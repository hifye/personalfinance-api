variable "project_name"          { type = string }
variable "vpc_id"                { type = string }
variable "subnet_ids"            { type = list(string) }
variable "db_password" { 
  type = string
  sensitive = true 
}
variable "ecs_security_group_id" { type = string }