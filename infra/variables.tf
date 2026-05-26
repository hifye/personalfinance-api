variable "aws_region" {
  description = "Região da AWS"
  type = string
  default = "sa-east-1"
}
variable "project_name" {
  description = "Nome do projeto"
  type = string
  default = "personal-finance"
}
variable "db_password" {
  description = "Senha mestre do RDS"
  type = string
  sensitive = true
}