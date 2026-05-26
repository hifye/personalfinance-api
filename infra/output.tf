output "rds_endpoint" {
  value = module.rds.db_instance_endpoint
}

output "ecr_repository_api_url" {
  value = module.ecr.api_repository_url
}

output "ecr_repository_migrations_url" {
  value = module.ecr.migrations_repository_url
}

output "ecs_cluster_name" {
  value = module.ecs.cluster_name
}
