variable "backend_base_url" {
  description = "URL base do ApiMovimentos acessível a partir do LocalStack (ex: http://host.docker.internal:8080)"
  type        = string
  default     = "http://host.docker.internal:8080"
}

variable "jwt_issuer" {
  description = "Issuer do provedor de identidade (ex: https://auth.local or http://host.docker.internal:5001)"
  type        = string
  default     = "https://auth.local"
}

variable "jwt_audience" {
  description = "Audience esperada nos tokens JWT (ex: api_movimentos)"
  type        = string
  default     = "api_movimentos"
}

resource "aws_apigatewayv2_api" "apimovimentos" {
  name          = "apimovimentos-http"
  protocol_type = "HTTP"
}

# integração genérica para encaminhar para o backend; usamos HTTP_PROXY para preservar
# método e path. A integration_uri aponta para a URL base do serviço.
resource "aws_apigatewayv2_integration" "backend" {
  api_id           = aws_apigatewayv2_api.apimovimentos.id
  integration_type = "HTTP_PROXY"
  integration_uri  = var.backend_base_url
  connection_type  = "INTERNET"
  timeout_milliseconds = 30000
}

# Authorizer JWT para validar token antes de encaminhar a requisição ao backend
resource "aws_apigatewayv2_authorizer" "jwt" {
  api_id = aws_apigatewayv2_api.apimovimentos.id
  name   = "jwt-authorizer"
  authorizer_type = "JWT"

  # O API Gateway HTTP espera sources como $request.header.Authorization
  identity_sources = ["$request.header.Authorization"]

  jwt_configuration {
    issuer = var.jwt_issuer
    audience = [var.jwt_audience]
  }
}

# Rotas para Contas
resource "aws_apigatewayv2_route" "contas_get_all" {
  api_id   = aws_apigatewayv2_api.apimovimentos.id
  route_key = "GET /api/contas"
  target    = "integrations/${aws_apigatewayv2_integration.backend.id}"
  authorization_type = "JWT"
  authorizer_id = aws_apigatewayv2_authorizer.jwt.id
}

resource "aws_apigatewayv2_route" "contas_get_by_id" {
  api_id   = aws_apigatewayv2_api.apimovimentos.id
  route_key = "GET /api/contas/{id}"
  target    = "integrations/${aws_apigatewayv2_integration.backend.id}"
  authorization_type = "JWT"
  authorizer_id = aws_apigatewayv2_authorizer.jwt.id
}

resource "aws_apigatewayv2_route" "contas_post" {
  api_id   = aws_apigatewayv2_api.apimovimentos.id
  route_key = "POST /api/contas"
  target    = "integrations/${aws_apigatewayv2_integration.backend.id}"
  authorization_type = "JWT"
  authorizer_id = aws_apigatewayv2_authorizer.jwt.id
}

resource "aws_apigatewayv2_route" "contas_delete" {
  api_id   = aws_apigatewayv2_api.apimovimentos.id
  route_key = "DELETE /api/contas/{id}"
  target    = "integrations/${aws_apigatewayv2_integration.backend.id}"
  authorization_type = "JWT"
  authorizer_id = aws_apigatewayv2_authorizer.jwt.id
}

# Rotas para Movimentos
resource "aws_apigatewayv2_route" "movimentos_get_all" {
  api_id   = aws_apigatewayv2_api.apimovimentos.id
  route_key = "GET /api/movimentos"
  target    = "integrations/${aws_apigatewayv2_integration.backend.id}"
  authorization_type = "JWT"
  authorizer_id = aws_apigatewayv2_authorizer.jwt.id
}

resource "aws_apigatewayv2_route" "movimentos_get_by_id" {
  api_id   = aws_apigatewayv2_api.apimovimentos.id
  route_key = "GET /api/movimentos/{id}"
  target    = "integrations/${aws_apigatewayv2_integration.backend.id}"
  authorization_type = "JWT"
  authorizer_id = aws_apigatewayv2_authorizer.jwt.id
}

resource "aws_apigatewayv2_route" "movimentos_get_by_conta" {
  api_id   = aws_apigatewayv2_api.apimovimentos.id
  route_key = "GET /api/movimentos/conta/{contaId}"
  target    = "integrations/${aws_apigatewayv2_integration.backend.id}"
  authorization_type = "JWT"
  authorizer_id = aws_apigatewayv2_authorizer.jwt.id
}

resource "aws_apigatewayv2_route" "movimentos_post" {
  api_id   = aws_apigatewayv2_api.apimovimentos.id
  route_key = "POST /api/movimentos"
  target    = "integrations/${aws_apigatewayv2_integration.backend.id}"
  authorization_type = "JWT"
  authorizer_id = aws_apigatewayv2_authorizer.jwt.id
}

# Stage para deploy automático
resource "aws_apigatewayv2_stage" "dev" {
  api_id     = aws_apigatewayv2_api.apimovimentos.id
  name       = "dev"
  auto_deploy = true
}

output "apigateway_base_url" {
  description = "URL base do API Gateway (apigatewayv2)"
  value       = aws_apigatewayv2_api.apimovimentos.api_endpoint
}
