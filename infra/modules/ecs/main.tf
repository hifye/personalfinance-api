# 1. O Cluster (O agrupamento lógico de tudo)
resource "aws_ecs_cluster" "main" {
  name = "${var.project_name}-cluster"
}

# 2. IAM Role (A identidade que o ECS assume para rodar as tarefas)
resource "aws_iam_role" "ecs_task_execution_role" {
  name = "${var.project_name}-task-execution-role"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [{
      Action = "sts:AssumeRole"
      Effect = "Allow"
      Principal = { Service = "ecs-tasks.amazonaws.com" }
    }]
  })
}

# 3. Anexar a política padrão que permite ler ECR e criar Logs
resource "aws_iam_role_policy_attachment" "ecs_task_execution_role_policy" {
  role       = aws_iam_role.ecs_task_execution_role.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy"
}

# 4. Grupo de Logs (Para ver os erros e sucessos no CloudWatch)
resource "aws_cloudwatch_log_group" "ecs_logs" {
  name              = "/ecs/${var.project_name}"
  retention_in_days = 7
}

# 5. Security Group para as Tasks do ECS
resource "aws_security_group" "ecs_tasks_sg" {
  name   = "${var.project_name}-ecs-tasks-sg"
  vpc_id = var.vpc_id

  ingress {
    from_port   = 8080 # Porta da API .NET
    to_port     = 8080
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

# 6. Task Definition para as Migrações (Flyway)
resource "aws_ecs_task_definition" "migrations" {
  family                   = "${var.project_name}-migrations"
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = "256"
  memory                   = "512"
  execution_role_arn       = aws_iam_role.ecs_task_execution_role.arn

  container_definitions = jsonencode([{
    name      = "migrations"
    image     = "${var.migrations_repository_url}:latest"
    essential = true
    environment = [
      { name = "FLYWAY_URL",      value = "jdbc:postgresql://${var.db_host}:5432/personalfinance" },
      { name = "FLYWAY_USER",     value = "postgres" },
      { name = "FLYWAY_PASSWORD", value = var.db_password }
    ]
    logConfiguration = {
      logDriver = "awslogs"
      options = {
        "awslogs-group"         = aws_cloudwatch_log_group.ecs_logs.name
        "awslogs-region"        = var.region
        "awslogs-stream-prefix" = "migrations"
      }
    }
  }])
}

# 7. Task Definition para a API
resource "aws_ecs_task_definition" "api" {
  family                   = "${var.project_name}-api"
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = "256"
  memory                   = "512"
  execution_role_arn       = aws_iam_role.ecs_task_execution_role.arn

  container_definitions = jsonencode([{
    name      = "api"
    image     = "${var.api_repository_url}:latest"
    essential = true
    portMappings = [{ containerPort = 8080, hostPort = 8080 }]
    environment = [
      { name = "ConnectionStrings__DefaultConnection", value = "Host=${var.db_host};Database=personalfinance;Username=postgres;Password=${var.db_password}" },
      { name = "ASPNETCORE_ENVIRONMENT",               value = "Production" }
    ]
    logConfiguration = {
      logDriver = "awslogs"
      options = {
        "awslogs-group"         = aws_cloudwatch_log_group.ecs_logs.name
        "awslogs-region"        = var.region
        "awslogs-stream-prefix" = "api"
      }
    }
  }])
}

# 8. Serviço para manter a API rodando sempre
resource "aws_ecs_service" "api" {
  name            = "${var.project_name}-api-service"
  cluster         = aws_ecs_cluster.main.id
  task_definition = aws_ecs_task_definition.api.arn
  launch_type     = "FARGATE"
  desired_count   = 1

  network_configuration {
    subnets          = var.subnet_ids
    security_groups  = [aws_security_group.ecs_tasks_sg.id]
    assign_public_ip = true
  }
}

