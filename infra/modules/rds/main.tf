# 1. Grupo de Subnets (Onde o banco pode circular)
resource "aws_db_subnet_group" "main" {
  name       = "${var.project_name}-db-subnet-group"
  subnet_ids = var.subnet_ids

  tags = { Name = "${var.project_name}-db-subnet-group" }
}

# 2. Security Group (Quem pode falar com o banco)
resource "aws_security_group" "rds_sg" {
  name   = "${var.project_name}-rds-sg"
  vpc_id = var.vpc_id

  # Permitir entrada na porta 5432 (Postgres)
  ingress {
    from_port   = 5432
    to_port     = 5432
    protocol    = "tcp"
    cidr_blocks = []
    security_groups = [var.ecs_security_group_id]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

# 3. A Instância do RDS
resource "aws_db_instance" "postgres" {
  identifier           = "${var.project_name}-db"
  engine               = "postgres"
  engine_version       = "17.2"
  instance_class       = "db.t4g.micro" # Econômica e moderna
  allocated_storage    = 20
  db_name              = "personalfinance"
  username             = "postgres"
  password             = var.db_password
  db_subnet_group_name = aws_db_subnet_group.main.name
  vpc_security_group_ids = [aws_security_group.rds_sg.id]
  publicly_accessible  = false
  skip_final_snapshot  = true

  tags = { Name = "${var.project_name}-db" }
}
