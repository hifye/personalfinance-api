variable "aws_region" {
  type    = string
  default = "sa-east-1"
}
variable "project_name" {
  type    = string
  default = "personal-finance"
}
variable "db_password" {
  type      = string
  sensitive = true
}
variable "jwt_key" {
  type      = string
  sensitive = true
}