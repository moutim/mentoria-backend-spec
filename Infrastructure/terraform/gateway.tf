# ========================================
# API Gateway - REST API
# ========================================
# Cria a API Gateway principal
resource "aws_api_gateway_rest_api" "movimentos_api" {
  name        = "movimentos-api"
  description = "API Gateway para gerenciamento de movimentos financeiros"
}

# ========================================
# Recursos (Paths)
# ========================================
# Cria o recurso /api
resource "aws_api_gateway_resource" "api" {
  rest_api_id = aws_api_gateway_rest_api.movimentos_api.id
  parent_id   = aws_api_gateway_rest_api.movimentos_api.root_resource_id
  path_part   = "api"
}

# Cria o recurso /api/movimentos
resource "aws_api_gateway_resource" "movimentos" {
  rest_api_id = aws_api_gateway_rest_api.movimentos_api.id
  parent_id   = aws_api_gateway_resource.api.id
  path_part   = "movimentos"
}

# ========================================
# Método GET /api/movimentos
# ========================================
# Define o método HTTP GET
resource "aws_api_gateway_method" "get_movimentos" {
  rest_api_id   = aws_api_gateway_rest_api.movimentos_api.id
  resource_id   = aws_api_gateway_resource.movimentos.id
  http_method   = "GET"
  authorization = "NONE"

  request_parameters = {
    "method.request.querystring.usuarioId" = false
  }
}

# Integração HTTP do GET - aponta para sua API real
resource "aws_api_gateway_integration" "get_movimentos" {
  rest_api_id             = aws_api_gateway_rest_api.movimentos_api.id
  resource_id             = aws_api_gateway_resource.movimentos.id
  http_method             = aws_api_gateway_method.get_movimentos.http_method
  type                    = "HTTP_PROXY"
  integration_http_method = "GET"
  uri                     = "http://host.docker.internal:5001/api/movimentos"

  request_parameters = {
    "integration.request.querystring.usuarioId" = "method.request.querystring.usuarioId"
  }
}

# ========================================
# Método POST /api/movimentos
# ========================================
# Define o método HTTP POST
resource "aws_api_gateway_method" "post_movimentos" {
  rest_api_id   = aws_api_gateway_rest_api.movimentos_api.id
  resource_id   = aws_api_gateway_resource.movimentos.id
  http_method   = "POST"
  authorization = "NONE"
}

# Integração HTTP do POST - aponta para sua API real
resource "aws_api_gateway_integration" "post_movimentos" {
  rest_api_id             = aws_api_gateway_rest_api.movimentos_api.id
  resource_id             = aws_api_gateway_resource.movimentos.id
  http_method             = aws_api_gateway_method.post_movimentos.http_method
  type                    = "HTTP_PROXY"
  integration_http_method = "POST"
  uri                     = "http://host.docker.internal:5001/api/movimentos"
}

# ========================================
# Deploy e Stage
# ========================================
# Deploy da API
resource "aws_api_gateway_deployment" "movimentos_deployment" {
  rest_api_id = aws_api_gateway_rest_api.movimentos_api.id

  triggers = {
    redeployment = sha1(jsonencode([
      aws_api_gateway_resource.movimentos.id,
      aws_api_gateway_method.get_movimentos.id,
      aws_api_gateway_method.post_movimentos.id,
      aws_api_gateway_integration.get_movimentos.id,
      aws_api_gateway_integration.post_movimentos.id,
    ]))
  }

  lifecycle {
    create_before_destroy = true
  }

  depends_on = [
    aws_api_gateway_integration.get_movimentos,
    aws_api_gateway_integration.post_movimentos
  ]
}

# Stage de desenvolvimento
resource "aws_api_gateway_stage" "dev" {
  deployment_id = aws_api_gateway_deployment.movimentos_deployment.id
  rest_api_id   = aws_api_gateway_rest_api.movimentos_api.id
  stage_name    = "dev"
}

# ========================================
# Outputs
# ========================================
# URL base da API Gateway
output "api_gateway_url" {
  value       = "http://localhost:4566/restapis/${aws_api_gateway_rest_api.movimentos_api.id}/dev/_user_request_"
  description = "URL base do API Gateway no LocalStack"
}

# ID da API
output "api_gateway_id" {
  value       = aws_api_gateway_rest_api.movimentos_api.id
  description = "ID da API Gateway"
}
