-- =============================================================================
-- PersonalFinance — init-db.sql
-- Executado automaticamente pelo Postgres na primeira inicialização do container
-- =============================================================================

-- -----------------------------------------------------------------------------
-- SCHEMAS
-- -----------------------------------------------------------------------------

CREATE SCHEMA IF NOT EXISTS auth;
CREATE SCHEMA IF NOT EXISTS finance;
CREATE SCHEMA IF NOT EXISTS catalog;

-- =============================================================================
-- AUTH
-- =============================================================================

CREATE TABLE auth.users
(
    id            UUID PRIMARY KEY      DEFAULT gen_random_uuid(),
    name          VARCHAR(200) NOT NULL,
    email         VARCHAR(100) NOT NULL,
    password_hash VARCHAR      NOT NULL,
    created_at    TIMESTAMPTZ  NOT NULL DEFAULT now(),
    updated_at    TIMESTAMPTZ  NOT NULL DEFAULT now()
);

CREATE UNIQUE INDEX idx_users_email
    ON auth.users (email);

-- -----------------------------------------------------------------------------

CREATE TABLE auth.refresh_tokens
(
    id         UUID PRIMARY KEY     DEFAULT gen_random_uuid(),
    user_id    UUID        NOT NULL,
    token_hash VARCHAR     NOT NULL,
    expires_at TIMESTAMPTZ NOT NULL,
    is_revoked BOOLEAN     NOT NULL DEFAULT false,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),

    CONSTRAINT fk_refresh_tokens_users
        FOREIGN KEY (user_id)
            REFERENCES auth.users (id)
            ON DELETE CASCADE
);

CREATE UNIQUE INDEX idx_refresh_token_hash
    ON auth.refresh_tokens (token_hash);

CREATE INDEX idx_refresh_tokens_user_id
    ON auth.refresh_tokens (user_id);

-- =============================================================================
-- FINANCE
-- =============================================================================
-- AccountType enum (Finance.Domain.Enums.AccountType):
--   None = 0 | Checking = 1 | Savings = 2 | Credit = 3
-- =============================================================================

CREATE TABLE finance.accounts
(
    id              UUID PRIMARY KEY        DEFAULT gen_random_uuid(),
    user_id         UUID           NOT NULL,
    name            VARCHAR(50)    NOT NULL,
    type            SMALLINT       NOT NULL,
    initial_balance NUMERIC(18, 2) NOT NULL,
    current_balance NUMERIC(18, 2) NOT NULL,
    is_active       BOOLEAN        NOT NULL DEFAULT true,
    created_at      TIMESTAMPTZ    NOT NULL DEFAULT now(),
    updated_at      TIMESTAMPTZ    NOT NULL DEFAULT now(),

    CONSTRAINT accounts_type_check
        CHECK (type IN (1, 2, 3)),

    CONSTRAINT uq_accounts_user_name
        UNIQUE (user_id, name)
);

-- =============================================================================
-- CATALOG
-- =============================================================================
-- CatalogType enum (Catalog.Domain.Enums.CatalogType):
--   None = 0 | Income = 1 | Expense = 2
-- =============================================================================

CREATE TABLE catalog.categories
(
    id         UUID PRIMARY KEY      DEFAULT gen_random_uuid(),
    user_id    UUID         NOT NULL,
    name       VARCHAR(100) NOT NULL,
    type       SMALLINT     NOT NULL,
    is_active  BOOLEAN      NOT NULL DEFAULT true,
    created_at TIMESTAMPTZ  NOT NULL DEFAULT now(),
    updated_at TIMESTAMPTZ  NOT NULL DEFAULT now(),

    CONSTRAINT categories_type_check
        CHECK (type IN (1, 2)),

    CONSTRAINT uq_categories_user_name
        UNIQUE (user_id, name)
);

-- =============================================================================
-- RECURRING TRANSACTIONS
-- =============================================================================
-- TransactionType enum (Finance.Domain.Enums.TransactionType):
--   None = 0 | Income = 1 | Expense = 2
--
-- RecurringFrequency enum (Finance.Domain.Enums.RecurringFrequency):
--   None = 0 | Daily = 1 | Weekly = 2 | Monthly = 3 | Yearly = 4
-- =============================================================================

CREATE TABLE finance.recurring_transactions
(
    id              UUID PRIMARY KEY        DEFAULT gen_random_uuid(),
    user_id         UUID           NOT NULL,
    account_id      UUID           NOT NULL,
    category_id     UUID,
    amount          NUMERIC(18, 2) NOT NULL,
    type            SMALLINT       NOT NULL,
    description     VARCHAR(250),
    frequency       SMALLINT       NOT NULL,
    start_date      DATE           NOT NULL,
    end_date        DATE,
    next_occurrence DATE           NOT NULL,
    is_active       BOOLEAN        NOT NULL DEFAULT true,
    created_at      TIMESTAMPTZ    NOT NULL DEFAULT now(),
    updated_at      TIMESTAMPTZ,

    CONSTRAINT recurring_transactions_type_check
        CHECK (type IN (1, 2)),

    CONSTRAINT recurring_transactions_frequency_check
        CHECK (frequency IN (1, 2, 3, 4)),

    CONSTRAINT fk_recurring_accounts
        FOREIGN KEY (account_id)
            REFERENCES finance.accounts (id)
            ON DELETE RESTRICT,

    CONSTRAINT fk_recurring_categories
        FOREIGN KEY (category_id)
            REFERENCES catalog.categories (id)
            ON DELETE SET NULL
);

-- =============================================================================
-- TRANSACTIONS
-- =============================================================================
-- TransactionType enum (Finance.Domain.Enums.TransactionType):
--   None = 0 | Income = 1 | Expense = 2
-- =============================================================================

CREATE TABLE finance.transactions
(
    id                       UUID PRIMARY KEY        DEFAULT gen_random_uuid(),
    user_id                  UUID           NOT NULL,
    account_id               UUID           NOT NULL,
    category_id              UUID,
    recurring_transaction_id UUID,
    amount                   NUMERIC(18, 2) NOT NULL,
    type                     SMALLINT       NOT NULL,
    description              VARCHAR(250),
    transaction_date         DATE           NOT NULL,
    created_at               TIMESTAMPTZ    NOT NULL DEFAULT now(),
    updated_at               TIMESTAMPTZ,

    CONSTRAINT transactions_type_check
        CHECK (type IN (1, 2)),

    CONSTRAINT fk_transactions_accounts
        FOREIGN KEY (account_id)
            REFERENCES finance.accounts (id)
            ON DELETE RESTRICT,

    CONSTRAINT fk_transactions_categories
        FOREIGN KEY (category_id)
            REFERENCES catalog.categories (id)
            ON DELETE SET NULL,

    CONSTRAINT fk_transactions_recurring
        FOREIGN KEY (recurring_transaction_id)
            REFERENCES finance.recurring_transactions (id)
            ON DELETE SET NULL
);

-- -----------------------------------------------------------------------------
-- INDEXES — finance.transactions
-- -----------------------------------------------------------------------------

CREATE INDEX idx_transactions_user_id
    ON finance.transactions (user_id);

CREATE INDEX idx_transactions_account_id
    ON finance.transactions (account_id);

CREATE INDEX idx_transactions_category_id
    ON finance.transactions (category_id);

CREATE INDEX idx_transactions_date
    ON finance.transactions (transaction_date);

CREATE INDEX idx_transactions_user_date
    ON finance.transactions (user_id, transaction_date);