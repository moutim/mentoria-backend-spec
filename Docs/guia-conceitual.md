Esta documentação apresenta os conceitos arquiteturais e orientações para implementação do projeto de mentoria backend.
## 📚 Índice
- [Arquitetura do Sistema](#arquitetura-do-sistema)

- [Configuração do Ambiente](#configuração-do-ambiente)

- [Fases de Implementação](#fases-de-implementação)

- [Casos de Uso](#casos-de-uso)

- [Estratégias de Teste](#estratégias-de-teste)

- [Resolução de Problemas](#resolução-de-problemas)
---

## 🏗️ Arquitetura do Sistema

### Estrutura Organizacional
O projeto está organizado em **Bounded Contexts** seguindo princípios de Domain-Driven Design:

#### Responsável pelas movimentações

- **`src/movimentos/`** - Contexto responsável por movimentações bancárias

- `ApiMovimentos/` - API REST para operações de movimentação

- `BffMovimentos/` - Backend for Frontend (agregação de dados)

- `WorkerMovimentos/` - Processamento assíncrono de eventos

#### Responsável pelo saldo
- **`src/saldos/`** - Contexto responsável por consultas de saldo

- `ApiSaldos/` - API REST para consultas de saldo

- `BffSaldos/` - Backend for Frontend

- `WorkerSaldos/` - Processamento de atualizações de saldo 

#### Responsável pelas notificações

- **`src/notificacoes/`** - Contexto responsável por notificações

- `WorkerNotificacoes/` - Processamento de notificações

### Fluxo Arquitetural

A arquitetura segue o padrão **Event-Driven** com separação de responsabilidades:

**Fluxo Síncrono**: Cliente → BFF → API → Services → Repository → Database

**Fluxo Assíncrono**: Events → SQS → Workers → Services → Repository

  
---
## ⚙️ Configuração do Ambiente

### Pré-requisitos Tecnológicos

- **Docker Desktop** - Para orquestração dos serviços de infraestrutura

- **.NET 8.0 SDK** - Plataforma de desenvolvimento

- **IDE** - Visual Studio, VS Code ou JetBrains Rider (sugestão)

- **Git** - Controle de versão

### Infraestrutura Local

O projeto utiliza `docker-compose.yaml` para provisionar:

- **PostgreSQL** - Banco de dados principal

- **pgAdmin** - Interface de administração do banco

- **LocalStack** - Simulação de serviços AWS (SQS, SSM, Secrets Manager)


### Configuração de Parâmetros

Utiliza **Infrastructure as Code** através de Terraform (`infra/`) para:

- Parâmetros de configuração via **SSM Parameter Store**

- Secrets via **AWS Secrets Manager**

- Filas SQS para comunicação assíncrona

**Benefícios desta abordagem:**

- Infraestrutura versionada e reproduzível

- Separação de configurações sensíveis

- Simulação próxima ao ambiente AWS real

---

  

## 🚀 Fases de Implementação

### Fase 1: Configuração Base (Estagiários/Júniors)

#### Configuração Entity Framework

**Arquivos sugeridos para trabalhar:**

- `src/*/appsettings.json` - Configurações da aplicação

- `infra/parameters.tf` - Parâmetros de infraestrutura

- `Data/AppDbContext.cs` - Contexto do banco de dados

**Conceitos a implementar:**

- Configuração de conexão com PostgreSQL via Entity Framework

- Integração com AWS LocalStack (SSM Parameter Store e Secrets Manager)

- Configuração de providers de configuração dinâmica

- Criação do DbContext com mapeamento de entidades

- Inicialização do banco via scripts Docker (`init-db/`)

#### Modelagem de Dados

**Arquivos sugeridos para trabalhar:**

- `Models/Entities/Conta.cs` - Entidade principal

- `Models/Entities/Movimento.cs` - Entidade de movimentação

- `Enums/TipoMovimento.cs` - Enumerações do domínio

**Conceitos a implementar:**

- Entidades do domínio bancário (Conta, Movimento)

- Relacionamentos entre entidades (One-to-Many)

- Validações de dados e constraints

- Enumerações para tipos de movimentação

### Fase 2: Camada de Aplicação (Júnior/Pleno)

#### DTOs e Contratos

**Arquivos sugeridos para trabalhar:**

- `DTOs/Requests/CriarMovimentoRequest.cs` - Request para criação

- `DTOs/Responses/MovimentoResponse.cs` - Response de movimentação

- `DTOs/Responses/SaldoResponse.cs` - Response de saldo

**Conceitos a implementar:**

- Data Transfer Objects (DTOs) para requisições e respostas

- Validações de entrada usando Data Annotations

- Mapeamento entre entidades e DTOs

- Separação entre modelos de domínio e API

#### Padrão Repository

**Arquivos sugeridos para trabalhar:**

- `Repositories/IContaRepository.cs` - Interface do repositório

- `Repositories/ContaRepository.cs` - Implementação do repositório

- `Repositories/IMovimentoRepository.cs` - Interface de movimentação

**Conceitos a implementar:**

- Abstração de acesso a dados

- Operações CRUD assíncronas

- Queries otimizadas com Entity Framework

- Injeção de dependência

#### Domain Services

**Arquivos sugeridos para trabalhar:**

- `Services/ISaldoService.cs` - Interface do serviço

- `Services/SaldoService.cs` - Lógica de negócio de saldo

- `Services/IMovimentoService.cs` - Interface de movimentação

  

**Conceitos a implementar:**

- Regras de negócio centralizadas

- Tratamento de exceções de domínio

- Logging e observabilidade

- Validações de negócio

  

#### Controllers REST

**Arquivos sugeridos para trabalhar:**

- `Controllers/SaldosController.cs` - API de consulta de saldos

- `Controllers/MovimentosController.cs` - API de movimentações

  

**Conceitos a implementar:**

- APIs RESTful bem estruturadas

- Códigos de status HTTP apropriados

- Documentação automática com Swagger

- Tratamento global de exceções

  

### Fase 3: Arquitetura Avançada (Pleno/Senior)

  

#### Event-Driven Architecture

**Arquivos sugeridos para trabalhar:**

- `Services/IEventBus.cs` - Interface de publicação de eventos

- `Services/SqsEventBus.cs` - Implementação com SQS

- `Events/MovimentoCriadoEvent.cs` - Eventos de domínio

  

**Conceitos a implementar:**

- Comunicação assíncrona via eventos

- Integração com AWS SQS (LocalStack)

- Publicação e consumo de mensagens

- Padrão Publisher/Subscriber

  

#### Background Processing

**Arquivos sugeridos para trabalhar:**

- `Workers/MovimentoWorker.cs` - Worker de processamento

- `Workers/SaldoWorker.cs` - Worker de saldos

- `Workers/NotificacaoWorker.cs` - Worker de notificações

  

**Conceitos a implementar:**

- Background Services com .NET

- Processamento de filas SQS

- Retry policies e error handling

- Monitoramento de workers

  

#### Transações e Consistência

**Conceitos a implementar:**

- Transações de banco de dados

- Padrão Unit of Work

- Eventual consistency

- Compensating transactions

  

### Fase 4: Qualidade e Observabilidade (Senior)

  

#### Configuração Centralizada

**Arquivos sugeridos para trabalhar:**

- `Program.cs` - Configuração da aplicação

- `appsettings.json` - Configurações base

- `Configuration/` - Providers customizados

- `init-db/` - Scripts de inicialização do banco

  

**Conceitos a implementar:**

- Dependency Injection avançada

- Configuration providers

- Health checks

- Middleware customizado

- Inicialização automática do banco via Docker

  

---

  

## 🧪 Casos de Uso

  

### Caso de Uso 1: Consultar Saldo

  

**Objetivo**: Permitir consulta do saldo atual de uma conta bancária

  

**Arquivo sugerido**: `Controllers/SaldosController.cs`

  

**Conceitos envolvidos:**

- API REST com verbo GET

- Validação de existência da conta

- Retorno de dados formatados

- Tratamento de exceções (404, 500)

  

**Fluxo conceitual:**

1. Receber requisição HTTP GET

2. Validar parâmetros de entrada

3. Buscar conta no repositório

4. Retornar saldo com timestamp

  

### Caso de Uso 2: Registrar Movimentação

  

**Objetivo**: Processar débitos e créditos em contas bancárias

  

**Arquivo sugerido**: `Controllers/MovimentosController.cs`

  

**Conceitos envolvidos:**

- API REST com verbo POST

- Validação de regras de negócio

- Transações de banco de dados

- Publicação de eventos

- Atualização de saldo

  

**Fluxo conceitual:**

1. Receber dados da movimentação

2. Validar dados e regras de negócio

3. Iniciar transação

4. Criar movimento e atualizar saldo

5. Publicar evento para processamento assíncrono

6. Confirmar transação

  

### Caso de Uso 3: Processar Notificações

  

**Objetivo**: Enviar notificações sobre movimentações

  

**Arquivo sugerido**: `Workers/NotificacaoWorker.cs`

  

**Conceitos envolvidos:**

- Background processing

- Consumo de filas SQS

- Processamento assíncrono

- Integração com serviços externos

  

---

  

## 🧪 Estratégias de Teste

  

### Testes Unitários

  

**Pasta sugerida**: `Tests/Services/`

  

**Conceitos a implementar:**

- Testes isolados de services e repositories

- Mock de dependências externas

- Arrange-Act-Assert pattern

- Validação de comportamentos esperados

- Cobertura de cenários de erro

  

**Ferramentas sugeridas:**

- xUnit para framework de testes

- Moq para criação de mocks

- Entity Framework InMemory para testes de repositório

  

### Testes de Integração

  

**Pasta sugerida**: `Tests/Integration/`

  

**Conceitos a implementar:**

- Testes end-to-end de APIs

- WebApplicationFactory para ambiente de teste

- Banco de dados em memória

- Validação de fluxos completos

- Teste de middlewares e filtros

  

### Testes de Carga

  

**Conceitos a considerar:**

- Teste de performance das APIs

- Simulação de carga concorrente

- Monitoramento de recursos

- Identificação de gargalos

  
  

---

  

## 🔧 Resolução de Problemas

  

### Problemas Comuns e Soluções

  

#### Problemas de Conectividade

**Sintomas**: Erro de conexão com PostgreSQL ou LocalStack

  

**Arquivos para verificar:**

- `docker-compose.yaml` - Configuração dos containers

- `appsettings.json` - Strings de conexão

- `infra/main.tf` - Configuração da infraestrutura

  

**Conceitos de solução:**

- Verificar status dos containers Docker

- Validar configurações de rede

- Testar conectividade com ferramentas de linha de comando

  

#### Problemas de Configuração

**Sintomas**: Erro na inicialização da aplicação ou banco de dados

  

**Arquivos para verificar:**

- `Program.cs` - Configuração de serviços

- `Configuration/` - Providers customizados

- `docker-compose.yaml` - Configuração dos containers

- `init-db/` - Scripts de inicialização do banco

- Logs da aplicação

  

**Conceitos de solução:**

- Validar injeção de dependência

- Verificar configurações de ambiente

- Analisar logs de startup

- Verificar execução dos scripts de inicialização

- Validar ordem de execução dos containers Docker

  

#### Problemas de Performance

**Sintomas**: Respostas lentas ou timeouts

  

**Áreas para investigar:**

- Queries do Entity Framework

- Processamento de filas SQS

- Concorrência de workers

  

**Conceitos de solução:**

- Otimizar queries com includes

- Implementar retry policies

- Monitorar recursos do sistema

  

### Ferramentas de Monitoramento

  

**Sugestões de implementação:**

- Health checks para APIs

- Logging estruturado com Serilog

- Métricas de performance

- Alertas para falhas críticas

  

---

  

## 📚 Recursos Adicionais

  

### Documentação Técnica

- [.NET 8 Documentation](https://docs.microsoft.com/en-us/dotnet/)

- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

- [LocalStack](https://docs.localstack.cloud/)

- [Docker Compose](https://docs.docker.com/compose/)

  

### Padrões Arquiteturais

- **Clean Architecture**: Separação de responsabilidades

- **Domain-Driven Design**: Modelagem baseada no domínio

- **Event-Driven Architecture**: Comunicação assíncrona

- **CQRS**: Separação de comando e consulta

  

### Ferramentas Recomendadas

- **IDE**: Visual Studio, VS Code, ou JetBrains Rider

- **Database**: pgAdmin, DBeaver

- **API Testing**: Postman, Insomnia

- **Monitoring**: Application Insights, Seq

  

**Nota**: Todas as referências de pastas, arquivos e ferramentas são sugestões para orientação do desenvolvimento. Adapte conforme a necessidade do projeto e preferências da equipe.

  
  

---

  

## 🆘 Troubleshooting

  

### Problemas Comuns

  

#### 1. Erro de Conexão com PostgreSQL

```bash

# Verificar se container está rodando

docker ps | grep mentoria_db

  

# Verificar logs

docker logs mentoria_db

  

# Testar conectividade

docker exec mentoria_db psql -U postgres -d postgres -c "SELECT 1;"

```

  

#### 2. LocalStack não responde

```bash

# Verificar status

curl http://localhost:4566/health

  

# Verificar logs

docker logs mentoria_localstack

  

# Restart

docker-compose restart localstack

```

  

#### 4. Problemas com filas SQS

```bash

# Verificar filas existentes

aws --endpoint-url=http://localhost:4566 sqs list-queues

  

# Recriar filas

aws --endpoint-url=http://localhost:4566 sqs create-queue --queue-name movimentos-queue

```

  

### Comandos Úteis

  

#### Docker

```bash

# Rebuild completo

docker-compose down --volumes

docker-compose up --build -d

  

# Ver logs em tempo real

docker-compose logs -f

  

# Executar comando no container

docker exec -it mentoria_db bash

```

  

#### .NET

```bash

# Restaurar dependências

dotnet restore

  

# Build da solução

dotnet build

  

# Executar aplicação

dotnet run

  

# Executar testes com coverage

dotnet test --collect:"XPlat Code Coverage"

```

  

#### Banco de Dados (Docker)

```bash

# Verificar logs de inicialização do banco

docker logs mentoria_db

  

# Executar scripts manualmente (se necessário)

docker exec -it mentoria_db psql -U postgres -d postgres -f /docker-entrypoint-initdb.d/script.sql

  

# Conectar ao banco para verificar estrutura

docker exec -it mentoria_db psql -U postgres -d postgres

```

  

---

  

## 📖 Recursos Adicionais

  

### Documentação

- [.NET 8 Documentation](https://docs.microsoft.com/en-us/dotnet/)

- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

- [LocalStack](https://docs.localstack.cloud/)

  

### Padrões e Práticas

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

- [Domain-Driven Design](https://domainlanguage.com/ddd/)

- [CQRS Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)

- [Event Sourcing](https://martinfowler.com/eaaDev/EventSourcing.html)

  

### Ferramentas Recomendadas

- **IDE**: Visual Studio, VS Code, ou JetBrains Rider

- **Database**: pgAdmin, DBeaver

- **API Testing**: Postman, Insomnia

- **Monitoring**: Seq, Serilog