# Repositório para a API (.NET)
resource "aws_ecr_repository" "api" {
  name                 = "${var.project_name}-api"
  image_tag_mutability = "MUTABLE" # Permite sobrescrever a tag 'latest'

  image_scanning_configuration {
    scan_on_push = true # AWS verifica vulnerabilidades toda vez que você faz push
  }
}

# Repositório para as Migrações (Flyway)
resource "aws_ecr_repository" "migrations" {
  name                 = "${var.project_name}-migrations"
  image_tag_mutability = "MUTABLE"

  image_scanning_configuration {
    scan_on_push = true
  }
}
