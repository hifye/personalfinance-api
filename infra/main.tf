terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    }
  }
}

provider "aws" {
  region = var.aws_region
}

module "networking" {
  source       = "./modules/networking"
  project_name = var.project_name
}

module "ecr" {
  source       = "./modules/ecr"
  project_name = var.project_name
}

module "rds" {
  source                = "./modules/rds"
  project_name          = var.project_name
  vpc_id                = module.networking.vpc_id
  subnet_ids            = module.networking.subnet_ids
  db_password           = var.db_password
  ecs_security_group_id = module.ecs.ecs_security_group_id
}

module "secrets" {
  source       = "./modules/secrets"
  project_name = var.project_name
  db_password  = var.db_password
  jwt_key      = var.jwt_key
  db_host      = module.rds.db_instance_endpoint
}

module "ecs" {
  source                       = "./modules/ecs"
  project_name                 = var.project_name
  vpc_id                       = module.networking.vpc_id
  subnet_ids                   = module.networking.subnet_ids
  region                       = var.aws_region
  api_repository_url           = module.ecr.api_repository_url
  migrations_repository_url    = module.ecr.migrations_repository_url
  db_host                      = module.rds.db_instance_endpoint
  connection_string_secret_arn = module.secrets.connection_string_arn
  jwt_key_secret_arn           = module.secrets.jwt_key_arn
  db_password_secret_arn       = module.secrets.db_password_arn
  jwt_issuer                   = "https://your-domain.com"
  jwt_audience                 = "https://your-domain.com"
  jwt_access_expiry            = 15
  jwt_refresh_expiry           = 7
}