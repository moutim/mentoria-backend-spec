# ============================================
# Variáveis Gerais
# ============================================

variable "environment" {
  description = "Ambiente de deployment (prod, staging, dev)"
  type        = string
  default     = "dev"
}

# ============================================
# Variáveis do Projeto Movimentos
# ============================================

# Database Variables
variable "movimentos_db_host" {
  description = "Host do banco de dados do projeto Movimentos"
  type        = string
  sensitive   = false
}

variable "movimentos_db_port" {
  description = "Porta do banco de dados do projeto Movimentos"
  type        = string
  default     = "5432"
}

variable "movimentos_db_name" {
  description = "Nome do banco de dados do projeto Movimentos"
  type        = string
  default     = "movimentos"
}

variable "movimentos_db_username" {
  description = "Usuário do banco de dados do projeto Movimentos"
  type        = string
  sensitive   = false
}

variable "movimentos_db_password" {
  description = "Senha do banco de dados do projeto Movimentos"
  type        = string
  sensitive   = true
}

# Security Variables
variable "movimentos_jwt_secret" {
  description = "JWT Secret para autenticação do projeto Movimentos"
  type        = string
  sensitive   = true
}

variable "movimentos_api_key" {
  description = "API Key do projeto Movimentos"
  type        = string
  sensitive   = true
}

# Application Variables
variable "movimentos_log_level" {
  description = "Nível de log da aplicação Movimentos (Debug, Info, Warning, Error)"
  type        = string
  default     = "Information"
}

