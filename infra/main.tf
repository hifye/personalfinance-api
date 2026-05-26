terraform {
  required_providers {
    aws = {
      source = "hashicorp/aws"
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
module "rds" {
  source       = "./modules/rds"
  project_name = var.project_name
  vpc_id       = module.networking.vpc_id
  subnet_ids   = module.networking.subnet_ids
  db_password  = var.db_password
}
module "ecr" {
  source       = "./modules/ecr"
  project_name = var.project_name
}
module "ecs" {
  source                    = "./modules/ecs"
  project_name              = var.project_name
  vpc_id                    = module.networking.vpc_id
  subnet_ids                = module.networking.subnet_ids
  region                    = var.aws_region

  api_repository_url        = module.ecr.api_repository_url
  migrations_repository_url = module.ecr.migrations_repository_url

  db_host                   = module.rds.db_instance_endpoint
  db_password               = var.db_password
}


