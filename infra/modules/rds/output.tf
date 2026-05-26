output "db_instance_endpoint" {
  description = "O endereço de conexão do banco de dados"
  # O split é para remover a porta (:5432) do final, deixando apenas o host
  value = split(":", aws_db_instance.postgres.endpoint)[0]
}

output "db_instance_id" {
  value = aws_db_instance.postgres.id
}
