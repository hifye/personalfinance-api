output "connection_string_arn" {
  value = aws_secretsmanager_secret.connection_string.arn
}

output "jwt_key_arn" {
  value = aws_secretsmanager_secret.jwt_key.arn
}

output "db_password_arn" {
  value = aws_secretsmanager_secret.db_password.arn
}