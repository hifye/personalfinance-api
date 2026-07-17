# PersonalFinance API

API REST para gerenciamento de finanças pessoais, construída com ASP.NET Core e arquitetura de monólito modular. O projeto serve como base de aprendizado prático de arquitetura de software e infraestrutura AWS.

---

## Sumário

- [Visão Geral](#visão-geral)
- [Arquitetura](#arquitetura)
- [Módulos](#módulos)
- [Decisões Técnicas](#decisões-técnicas)
- [Infraestrutura AWS](#infraestrutura-aws)
- [CI/CD](#cicd)
- [Segurança](#segurança)
- [Como Executar Localmente](#como-executar-localmente)
- [Testes](#testes)
- [Variáveis de Ambiente](#variáveis-de-ambiente)
- [Migrations](#migrations)
- [Endpoints](#endpoints)

---

## Visão Geral

A API permite que usuários gerenciem contas bancárias, categorias de transações e transações recorrentes. Cada usuário tem isolamento completo dos seus dados via autenticação JWT.

**Stack principal:**
- .NET 10 / ASP.NET Core
- PostgreSQL + Dapper
- MediatR (CQRS)
- FluentValidation
- Serilog
- Flyway (migrations)
- Docker
- Terraform (infraestrutura AWS)

---

## Arquitetura

O projeto segue a arquitetura de **Modular Monolith** — cada módulo de negócio é independente internamente (Domain, Application, Infrastructure, Api), mas todos rodam no mesmo processo. Essa abordagem foi escolhida intencionalmente como passo intermediário antes de uma eventual extração para microsserviços.

```
PersonalFinance/
├── Host/                          # Entry point — composição de módulos
├── src/
│   ├── BuildingBlocks/            # Infraestrutura transversal compartilhada
│   ├── SharedKernel/              # Primitivos de domínio (Result, Guard, ValueObjects)
│   └── Modules/
│       ├── Auth/                  # Autenticação e autorização
│       │   ├── Auth.Domain
│       │   ├── Auth.Application
│       │   ├── Auth.Infrastructure
│       │   └── Auth.Api
│       ├── Finance/               # Contas e transações
│       │   ├── Finance.Domain
│       │   ├── Finance.Application
│       │   ├── Finance.Infrastructure
│       │   └── Finance.Api
│       └── Catalog/               # Categorias de transações
│           ├── Catalog.Domain
│           ├── Catalog.Application
│           ├── Catalog.Infrastructure
│           └── Catalog.Api
├── tests/
│   ├── Modules/
│   │   ├── Auth/Auth.UnitTests
│   │   ├── Finance/Finance.UnitTests
│   │   └── Catalog/Catalog.UnitTests
│   └── IntegrationTests/
├── database/
│   ├── Dockerfile                 # Imagem do Flyway
│   └── migrations/                # Scripts SQL versionados
└── infra/                         # Terraform — infraestrutura AWS
    └── modules/
        ├── networking/
        ├── rds/
        ├── ecs/
        ├── ecr/
        └── secrets/
```

### Camadas por módulo

Cada módulo segue Clean Architecture com dependências apontando para dentro:

```
Api → Application → Domain
Infrastructure → Application
```

- **Domain** — entidades, value objects, regras de negócio. Sem dependências externas.
- **Application** — casos de uso (Commands/Queries via MediatR), interfaces de repositório e serviços.
- **Infrastructure** — implementações concretas: repositórios com Dapper, queries SQL, providers de JWT e hash.
- **Api** — endpoints Minimal API, mapeamento de rotas.

### Comunicação entre módulos

Módulos não se referenciam diretamente. A única exceção controlada é via interface `ICatalogModule` definida no `BuildingBlocks`, que Finance usa para verificar se uma categoria existe sem criar acoplamento em tempo de compilação com o Catalog.

---

## Módulos

### Auth

Responsável por registro, login, logout e renovação de tokens.

- Senha armazenada com **Argon2id** (via Konscious.Security.Cryptography)
- Rehash automático de senhas com parâmetros desatualizados no login
- Refresh token armazenado como hash SHA-256 no banco
- JWT com expiração configurável via appsettings

**Entidades:** `User`, `RefreshToken`

### Finance

Gerenciamento de contas bancárias e transações.

- Contas com saldo inicial e corrente
- Transações simples (receita/despesa)
- Transações recorrentes com frequência configurável (diária, semanal, mensal, anual)
- Resumo financeiro por período.

**Entidades:** `Account`, `Transaction`, `RecurringTransaction`

### Catalog

Categorias de transações por usuário

- Categorias do tipo Receita ou Despesa
- Expõe `ICatalogModule` (via BuildingBlocks) para outros módulos verificarem existência de categoria

**Entidades:** `Catalog`

---

## Decisões Técnicas

### Result Pattern

Todos os casos de uso retornam `Result<T>` ou `Result` ao invés de lançar exceções para fluxos de negócio. Exceções são reservadas para falhas inesperadas de infraestrutura.

```csharp
public static Result<Account> Create(Guid userId, string name, AccountType type, decimal initialBalance)
{
    return Guard.AgainstOutOfRange(userId == Guid.Empty, "The field User id is mandatory.")
        .Bind(() => Guard.AgainstNullOrWhiteSpace(name, "The field name is mandatory."))
        .Bind(() => Price.Create(initialBalance))
        .Map(validPrice => new Account(userId, name, type, validPrice));
}
```

### CQRS com MediatR

Commands (escrita) e Queries (leitura) são separados. Queries usam Dapper diretamente para leitura otimizada sem passar pelo modelo de domínio. Commands passam pelo domínio e usam Unit of Work para transações.

### Unit of Work com Dapper

`UnitOfWork` mantém uma conexão e transação abertas durante o request (lifetime Scoped). Repositórios de escrita participam da transação via `unitOfWork.Transaction`. Repositórios de leitura abrem conexões próprias independentes, sem transação.

### Validação em dois níveis

1. **FluentValidation** no pipeline do MediatR (`ValidationBehavior`) — valida formato e regras de input antes de chegar no handler
2. **Domain validation** via Result pattern dentro das entidades — valida regras de negócio

### Logging com mascaramento

`LoggingBehavior` intercepta todos os requests/responses e mascara automaticamente campos sensíveis (`Password`, `Token`, `RefreshToken`, etc.) antes de logar.

---

## Infraestrutura AWS

```
Internet
    │
    ▼
ECS Fargate (API)
    │
    ├──► RDS PostgreSQL (subnet privada, sem acesso público)
    │
    └──► AWS Secrets Manager (connection string, JWT key)

ECR ──► ECS (imagem da API)
ECR ──► ECS Task (imagem das migrations — roda antes da API subir)
```

### Componentes

| Componente | Serviço AWS | Detalhes |
|---|---|---|
| Aplicação | ECS Fargate | 256 CPU, 512MB RAM |
| Banco de dados | RDS PostgreSQL 17 | db.t4g.micro, 20GB |
| Imagens Docker | ECR | API + Migrations |
| Secrets | Secrets Manager | Connection string, JWT Key |
| Logs | CloudWatch | Retenção de 7 dias |
| Rede | VPC | 2 subnets públicas (sa-east-1a, sa-east-1b) |

### Segurança de rede

- RDS com `publicly_accessible = false`
- Security group do RDS aceita conexões apenas do security group do ECS na porta 5432
- Secrets nunca em variáveis de ambiente — injetados via campo `secrets` da task definition do ECS

### Migrations

As migrations rodam como uma ECS Task separada antes da API subir. A task usa a imagem Flyway com os scripts SQL versionados. O serviço da API (`depends_on: flyway: condition: service_completed_successfully`) só sobe após as migrations concluírem com sucesso.

---

## CI/CD

Três pipelines no GitHub Actions:

### CI (`.github/workflows/ci.yml`)

Dispara em todo pull request e push para `master`.

```
Checkout → Setup .NET 10 → Restore → Build → Test → Upload resultados
```

### Deploy (`.github/workflows/deploy.yml`)

Dispara automaticamente após CI passar em `master`, ou manualmente via `workflow_dispatch`.

```
Checkout → Configure AWS credentials (OIDC) → Login ECR →
Build e push imagem → Download task definition atual →
Render nova task definition → Deploy no ECS → Aguarda estabilização
```

Usa OIDC para autenticação com a AWS — sem access keys armazenadas nos secrets do GitHub.

### Migrations Image (`.github/workflows/migrations-image.yml`)

Dispara apenas quando há mudanças em `database/migrations/**` ou `database/Dockerfile`.

```
Checkout → Configure AWS → Login ECR → Build e push imagem Flyway
```

---

## Segurança

### Autenticação

- JWT Bearer com validação de issuer, audience, lifetime e assinatura
- Access token com expiração curta (padrão: 15 minutos)
- Refresh token com expiração longa (padrão: 7 dias), armazenado como hash
- Rotação de refresh token a cada uso — token antigo é revogado imediatamente
- Logout revoga todos os tokens do usuário

### Senhas

- Argon2id com parâmetros: 4 iterações, 64MB de memória, paralelismo 8
- Rehash automático se os parâmetros do hash armazenado estiverem desatualizados

### Rate Limiting

Rate limiting por IP (endpoints públicos) e por `userId` (endpoints autenticados), usando `X-Forwarded-For` para funcionar corretamente atrás do ALB.

| Política | Limite | Janela | Usado em |
|---|---|---|---|
| `auth-strict` | 5 req | 1 min | register, login, update-password |
| `auth-token` | 5 req | 1 min | refresh-token |
| `writes` | 30 req | 1 min | POST, PATCH, DELETE autenticados |
| `heavy-reads` | 60 req | 1 min | GET autenticados |
| `health` | 30 req | 1 min | /health |

---

## Como Executar Localmente

**Pré-requisitos:** Docker, Docker Compose, .NET 10 SDK

**1. Copia o arquivo de variáveis:**
```bash
cp .env.example .env
```

**2. Preenche as variáveis no `.env`:**
```env
POSTGRES_PASSWORD=sua_senha
ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=personalfinance;Username=postgres;Password=sua_senha
JWT__KEY=sua_chave_secreta_minimo_32_caracteres
JWT__ISSUER=personalfinance
JWT__AUDIENCE=personalfinance-api
JWT_ACCESS_EXPIRY=15
JWT_REFRESH_EXPIRY=7
TAG=latest
```

**3. Sobe o ambiente:**
```bash
docker compose up --build
```

O Docker Compose sobe o PostgreSQL, roda as migrations via Flyway e depois sobe a API. A documentação interativa estará disponível em:

```
http://localhost:8080/scalar
```

---

## Testes

O projeto tem três tipos de teste:

### Testes Unitários

Cobrem handlers de Commands e Queries, validators e regras de domínio. Usam NSubstitute para mocks e FluentAssertions para assertivas.

```bash
dotnet test tests/Modules/Auth/Auth.UnitTests
dotnet test tests/Modules/Finance/Finance.UnitTests
dotnet test tests/Modules/Catalog/Catalog.UnitTests
```

### Testes de Integração

Sobem um PostgreSQL real via Testcontainers, executam as migrations e testam os endpoints HTTP end-to-end. Usam Respawn para limpar o banco entre testes.

```bash
dotnet test tests/IntegrationTests
```

### Todos os testes

```bash
dotnet test PersonalFinance.sln
```

---

## Variáveis de Ambiente

| Variável | Descrição | Obrigatório |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | Connection string do PostgreSQL | Sim |
| `JWT__Key` | Chave de assinatura do JWT (mínimo 32 caracteres) | Sim |
| `JWT__Issuer` | Issuer do JWT | Sim |
| `JWT__Audience` | Audience do JWT | Sim |
| `JWT__AccessTokenExpirationMinutes` | Expiração do access token em minutos | Não (padrão: 15) |
| `JWT__RefreshTokenExpirationDays` | Expiração do refresh token em dias | Não (padrão: 7) |

Em produção (AWS), `ConnectionStrings__DefaultConnection` e `JWT__Key` são injetados via AWS Secrets Manager — nunca ficam expostos como variáveis de ambiente na task definition.

---

## Migrations

As migrations usam Flyway com versionamento sequencial:

| Versão | Descrição |
|---|---|
| V1 | Criação dos schemas (auth, finance, catalog) |
| V2 | Tabelas de autenticação (users, refresh_tokens) |
| V3 | Tabela de contas (finance.accounts) |
| V4 | Tabela de categorias (catalog.categories) |
| V5 | Tabela de transações recorrentes |
| V6 | Tabela de transações |

Para rodar as migrations manualmente contra o RDS:

```bash
cp .env.rds.example .env.rds
# preenche com os dados do RDS
docker compose -f docker-compose.rds.yml --env-file .env.rds up
```

---

## Endpoints

### Auth

| Método | Rota | Autenticação | Descrição |
|---|---|---|---|
| POST | `/api/auth/register` | Não | Registro de novo usuário |
| POST | `/api/auth/login` | Não | Login — retorna access e refresh token |
| POST | `/api/auth/refresh-token` | Não | Renova o access token |
| PATCH | `/api/auth/update-password` | Sim | Atualiza a senha |
| POST | `/api/auth/logout` | Sim | Revoga todos os tokens do usuário |

### Catalog

| Método | Rota | Descrição |
|---|---|---|
| POST | `/api/catalog/create-category` | Cria uma categoria |
| GET | `/api/catalog/get-categories-user` | Lista categorias do usuário |
| GET | `/api/catalog/get-category/{id}` | Detalhes de uma categoria |
| PATCH | `/api/catalog/patch-category/{id}` | Atualiza uma categoria |
| DELETE | `/api/catalog/delete-category/{id}` | Remove uma categoria |

### Finance — Contas

| Método | Rota | Descrição |
|---|---|---|
| POST | `/api/finance/create-account` | Cria uma conta |
| GET | `/api/finance/get-accounts-user` | Lista contas do usuário |
| GET | `/api/finance/get-account-details/{id}` | Detalhes de uma conta |
| PATCH | `/api/finance/patch-account/{id}` | Atualiza uma conta |
| DELETE | `/api/finance/delete-account/{id}` | Remove uma conta |

### Finance — Transações

| Método | Rota | Descrição |
|---|---|---|
| POST | `/api/finance/create-transaction` | Cria uma transação |
| GET | `/api/finance/get-transactions-user` | Lista transações do usuário |
| GET | `/api/finance/get-transaction-details/{id}` | Detalhes de uma transação |
| GET | `/api/finance/get-transactions-summary` | Resumo por período (`?startDate=&endDate=`) |
| PATCH | `/api/finance/patch-transaction/{id}` | Atualiza uma transação |
| DELETE | `/api/finance/delete-transaction/{id}` | Remove uma transação |

### Finance — Transações Recorrentes

| Método | Rota | Descrição |
|---|---|---|
| POST | `/api/finance/create-recurring-transaction` | Cria uma transação recorrente |
| GET | `/api/finance/get-recurring-transactions-user` | Lista transações recorrentes do usuário |
| GET | `/api/finance/get-recurring-transaction-details/{id}` | Detalhes de uma transação recorrente |
| PATCH | `/api/finance/patch-recurring-transaction/{id}` | Atualiza uma transação recorrente |
| DELETE | `/api/finance/delete-recurring-transaction/{id}` | Remove uma transação recorrente |

### Health Checks

| Rota | Descrição |
|---|---|
| `/health` | Status geral |
| `/health/live` | Liveness — processo está vivo |
| `/health/ready` | Readiness — banco de dados acessível |
