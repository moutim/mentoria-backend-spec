# API Gateway - LocalStack

Este diretório contém a configuração do Terraform para provisionar um API Gateway no LocalStack que roteia requisições para a API de Movimentos.

## 📋 O que foi configurado

### Estrutura do API Gateway

```
API Gateway (movimentos-api)
└── /api
    └── /movimentos
        ├── GET  - Listar movimentos por conta
        └── POST - Criar novo movimento
```

### Componentes Principais

1. **aws_api_gateway_rest_api** - API principal chamada "movimentos-api"
2. **Resources** - Define os paths `/api` e `/api/movimentos`
3. **Methods** - Define os métodos HTTP (GET e POST)
4. **Integrations** - Conecta o API Gateway com sua API em `http://host.docker.internal:5000`
5. **Deployment** - Faz deploy das configurações
6. **Stage** - Cria o ambiente "dev"

## 🚀 Como usar

### 1. Subir a infraestrutura

```bash
# Na raiz do projeto
docker-compose up -d
```

Isso vai subir:
- PostgreSQL (porta 5432)
- LocalStack (porta 4566)
- Terraform (container)

### 2. Aplicar o Terraform

```bash
# Entrar no container do Terraform
docker exec -it mentoria_terraform sh

# Dentro do container
cd terraform
terraform init
terraform apply -auto-approve
```

### 3. Obter a URL do API Gateway

Após o `terraform apply`, você verá um output com a URL:

```
Outputs:
api_gateway_id = "xxxxxxxxxxxxx"
api_gateway_url = "http://localhost:4566/restapis/xxxxxxxxxxxxx/dev/_user_request_"
```

### 4. Subir sua API de Movimentos

Certifique-se que sua API .NET está rodando na porta 5000:

```bash
cd Movimentos/ApiMovimentos/WebApi
dotnet run
```

## 🧪 Testando as rotas

### GET - Listar movimentos

```bash
# Via API Gateway
curl "http://localhost:4566/restapis/{api_id}/dev/_user_request_/api/movimentos?usuarioId=123"

# Direto na API (para comparar)
curl "http://localhost:5000/api/movimentos?usuarioId=123"
```

### POST - Criar movimento

```bash
# Via API Gateway
curl -X POST "http://localhost:4566/restapis/{api_id}/dev/_user_request_/api/movimentos" \
  -H "Content-Type: application/json" \
  -d '{
    "usuarioId": "user123",
    "remetente": "João Silva",
    "destinatario": "Maria Santos",
    "tipo": 1,
    "categoriaId": "cat123",
    "descricao": "Pagamento de serviço",
    "valor": 150.00
  }'

# Direto na API (para comparar)
curl -X POST "http://localhost:5000/api/movimentos" \
  -H "Content-Type: application/json" \
  -d '{
    "usuarioId": "user123",
    "remetente": "João Silva",
    "destinatario": "Maria Santos",
    "tipo": 1,
    "categoriaId": "cat123",
    "descricao": "Pagamento de serviço",
    "valor": 150.00
  }'
```

## 📝 Explicação Técnica

### HTTP_PROXY vs MOCK

- **HTTP_PROXY**: Encaminha a requisição para um endpoint real (sua API)
- **MOCK**: Retorna uma resposta fixa (não foi usado aqui)

### host.docker.internal

Esta URL especial permite que containers Docker acessem serviços rodando no host (sua máquina). Como sua API .NET roda localmente na porta 5000, o API Gateway (dentro do LocalStack) usa `host.docker.internal:5000` para alcançá-la.

### Query Parameters

O parâmetro `usuarioId` é mapeado do API Gateway para sua API:
```
request_parameters = {
  "integration.request.querystring.usuarioId" = "method.request.querystring.usuarioId"
}
```

### Triggers de Redeployment

O deployment é automaticamente acionado quando qualquer recurso ou método muda:
```hcl
triggers = {
  redeployment = sha1(jsonencode([...]))
}
```

## 🔧 Comandos úteis

```bash
# Ver estado do Terraform
terraform show

# Ver outputs
terraform output

# Destruir recursos
terraform destroy -auto-approve

# Validar configuração
terraform validate

# Ver plano antes de aplicar
terraform plan
```

## 🐛 Troubleshooting

### API Gateway não acessa a API local

Verifique se:
1. Sua API está rodando na porta 5000
2. O Docker consegue acessar `host.docker.internal`
3. No Linux, pode ser necessário usar `host.docker.internal` ou o IP da sua máquina

### Terraform não conecta no LocalStack

1. Verifique se o LocalStack está rodando: `docker ps | grep localstack`
2. Teste o endpoint: `curl http://localhost:4566/_localstack/health`

### Mudanças não refletem

Execute `terraform apply` novamente. O trigger vai detectar mudanças e fazer redeploy automaticamente.

## 📚 Próximos passos

- [ ] Adicionar autenticação (API Key, JWT)
- [ ] Configurar CORS
- [ ] Adicionar rate limiting
- [ ] Implementar custom domain
- [ ] Adicionar logs e monitoring
