output "api_repository_url" {
  value = aws_ecr_repository.api.repository_url
}

output "migrations_repository_url" {
  value = aws_ecr_repository.migrations.repository_url
}
