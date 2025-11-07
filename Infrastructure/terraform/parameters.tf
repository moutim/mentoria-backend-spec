resource "aws_ssm_parameter" "foo" {
  name  = "foo"
  type  = "String"
  value = "bar"
}

# ============================================
# Parâmetros do Projeto Movimentos
# ============================================

# Database Configuration
resource "aws_ssm_parameter" "movimentos_db_connection" {
  name  = "/movimentos/database/connection"
  type  = "String"
  value = "Host=${var.movimentos_db_host};Port=${var.movimentos_db_port};Database=${var.movimentos_db_name};Username=${var.movimentos_db_username};Password=${var.movimentos_db_password}"
  
  tags = {
    Project = "Movimentos"
    Category = "Database"
  }
}

resource "aws_ssm_parameter" "movimentos_db_host" {
  name  = "/movimentos/database/host"
  type  = "String"
  value = var.movimentos_db_host
  
  tags = {
    Project = "Movimentos"
    Category = "Database"
  }
}

resource "aws_ssm_parameter" "movimentos_db_port" {
  name  = "/movimentos/database/port"
  type  = "String"
  value = var.movimentos_db_port
  
  tags = {
    Project = "Movimentos"
    Category = "Database"
  }
}

resource "aws_ssm_parameter" "movimentos_db_username" {
  name  = "/movimentos/database/username"
  type  = "String"
  value = var.movimentos_db_username
  
  tags = {
    Project = "Movimentos"
    Category = "Database"
  }
}

resource "aws_ssm_parameter" "movimentos_db_password" {
  name  = "/movimentos/database/password"
  type  = "SecureString"
  value = var.movimentos_db_password
  
  tags = {
    Project = "Movimentos"
    Category = "Database"
  }
}

# Security Configuration
resource "aws_ssm_parameter" "movimentos_jwt_secret" {
  name  = "/movimentos/security/jwt-secret"
  type  = "SecureString"
  value = var.movimentos_jwt_secret
  
  tags = {
    Project = "Movimentos"
    Category = "Security"
  }
}

resource "aws_ssm_parameter" "movimentos_api_key" {
  name  = "/movimentos/security/api-key"
  type  = "SecureString"
  value = var.movimentos_api_key
  
  tags = {
    Project = "Movimentos"
    Category = "Security"
  }
}

# Application Configuration
resource "aws_ssm_parameter" "movimentos_environment" {
  name  = "/movimentos/app/environment"
  type  = "String"
  value = var.environment
  
  tags = {
    Project = "Movimentos"
    Category = "Application"
  }
}

resource "aws_ssm_parameter" "movimentos_log_level" {
  name  = "/movimentos/app/log-level"
  type  = "String"
  value = var.movimentos_log_level
  
  tags = {
    Project = "Movimentos"
    Category = "Application"
  }
}

