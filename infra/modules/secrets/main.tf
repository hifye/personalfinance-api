resource "aws_secretsmanager_secret" "connection_string" {
  name                    = "${var.project_name}/connection-string"
  recovery_window_in_days = 0
}

resource "aws_secretsmanager_secret_version" "connection_string" {
  secret_id     = aws_secretsmanager_secret.connection_string.id
  secret_string = "Host=${var.db_host};Database=personalfinance;Username=postgres;Password=${var.db_password}"
}

resource "aws_secretsmanager_secret" "jwt_key" {
  name                    = "${var.project_name}/jwt-key"
  recovery_window_in_days = 0
}

resource "aws_secretsmanager_secret_version" "jwt_key" {
  secret_id     = aws_secretsmanager_secret.jwt_key.id
  secret_string = var.jwt_key
}

resource "aws_secretsmanager_secret" "db_password" {
  name                    = "${var.project_name}/db-password"
  recovery_window_in_days = 0
}

resource "aws_secretsmanager_secret_version" "db_password" {
  secret_id     = aws_secretsmanager_secret.db_password.id
  secret_string = var.db_password
}