resource "aws_ssm_parameter" "database_connectionstring" {
  name  = "/movimentos/database/connectionstring"
  type  = "String"
  value = "Host=localhost;Port=5432;Database=movimentos;Username=postgres;Password=postgres"
  
  description = "Connection string para o banco de dados PostgreSQL da API de Movimentos"
  
  tags = {
    Application = "ApiMovimentos"
    Environment = "Development"
  }
}
