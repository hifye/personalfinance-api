variable "project_name" { type = string }
variable "vpc_id"       { type = string }
variable "subnet_ids"   { type = list(string) }
variable "region"       { type = string }

# URLs dos repositórios no módulo ECR
variable "api_repository_url"        { type = string }
variable "migrations_repository_url" { type = string }

# Informações do banco para passar para os containers
variable "db_host"     { type = string }
variable "db_password" { type = string }
