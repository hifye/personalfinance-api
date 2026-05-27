variable "project_name" { type = string }
variable "db_password"  {
  type      = string
  sensitive = true
}
variable "jwt_key" {
  type      = string
  sensitive = true
}
variable "db_host" { type = string }