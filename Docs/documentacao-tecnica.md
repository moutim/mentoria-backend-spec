
Esta documentação contém todas as instruções detalhadas, exemplos de código e guias passo a passo para implementação do projeto de mentoria.

  

## 📚 Índice

  

- [Arquitetura do Sistema](#arquitetura-do-sistema)

- [Configuração do Ambiente](#configuração-do-ambiente)

- [Guia de Implementação](#guia-de-implementação)

- [Casos de Uso Detalhados](#casos-de-uso-detalhados)

- [Exemplos de Código](#exemplos-de-código)

- [Testes](#testes)

- [Troubleshooting](#troubleshooting)

  

---

  

## 🏗️ Arquitetura do Sistema

  

### Estrutura de Pastas

```

src/

├── movimentos/ # Bounded Context - Movimentações

│ ├── ApiMovimentos/ # Experience Layer

│ ├── BffMovimentos/ # Backend for Frontend

│ └── WorkerMovimentos/ # Background Processing

├── saldos/ # Bounded Context - Saldos

│ ├── ApiSaldos/ # Experience Layer

│ ├── BffSaldos/ # Backend for Frontend

│ └── WorkerSaldos/ # Background Processing

└── notificacoes/ # Bounded Context - Notificações

└── WorkerNotificacoes/ # Background Processing

```

  

### Fluxo de Dados

```

Cliente → BFF → API → Domain Services → Repository → Database

↓

Events → SQS → Worker → Domain Services → Repository

```

  

---

  

## ⚙️ Configuração do Ambiente

  

### Pré-requisitos

- Docker Desktop instalado

- .NET 8.0 SDK

- IDE (Visual Studio, VS Code ou Rider)

- Git

  

### Passo 1: Preparação Inicial

```bash

# Clone o repositório

git clone [URL_DO_REPOSITORIO]

cd mentoria-backend

  

# Verifique as ferramentas

docker --version

dotnet --version

```

  

### Passo 2: Configuração da Infraestrutura

```bash

# Subir todos os serviços

docker-compose up --build -d

  

# Verificar status dos containers

docker ps

  

# Containers esperados:

# - mentoria_db (PostgreSQL)

# - mentoria_pgadmin (Interface do banco)

# - mentoria_localstack (AWS Local)

```

  

### Passo 3: Validação do Ambiente

```bash

# Testar conectividade com PostgreSQL

docker exec mentoria_db psql -U postgres -d postgres -c "SELECT version();"

  

# Testar LocalStack

curl http://localhost:4566

  

# Acessar pgAdmin

# URL: http://localhost:3080

# Email: admin@mentoria.com

# Senha: admin

```

  

### Passo 4: Configuração do LocalStack (AWS Local)

```bash

# Criar filas SQS necessárias

aws --endpoint-url=http://localhost:4566 sqs create-queue --queue-name movimentos-queue

aws --endpoint-url=http://localhost:4566 sqs create-queue --queue-name saldos-queue

aws --endpoint-url=http://localhost:4566 sqs create-queue --queue-name notificacoes-queue

  

# Verificar filas criadas

aws --endpoint-url=http://localhost:4566 sqs list-queues

```

  

---

  

## 🚀 Guia de Implementação

  

### Fase 1: Configuração dos Projetos (Estagiários/Júniors)

  

#### 1.1 Configurar Entity Framework

```bash

# Instalar pacotes necessários em cada API

cd src/movimentos/ApiMovimentos

dotnet add package Microsoft.EntityFrameworkCore.Design

dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL

dotnet add package AWSSDK.SimpleSystemsManagement

dotnet add package AWSSDK.SecretsManager

dotnet add package AWSSDK.Extensions.NETCore.Setup

```

  

#### 1.2 Configurar Parâmetros via LocalStack (SSM/Secrets Manager)

  

**Passo 1: Criar infraestrutura com Terraform**

```hcl

# infra/parameters.tf

# Parâmetros SSM Parameter Store

resource "aws_ssm_parameter" "database_host" {

name = "/mentoria/database/host"

type = "String"

value = "db" # Nome do serviço no docker-compose

tags = {

Environment = "development"

Project = "mentoria-backend"

}

}

  

resource "aws_ssm_parameter" "database_port" {

name = "/mentoria/database/port"

type = "String"

value = "5432"

tags = {

Environment = "development"

Project = "mentoria-backend"

}

}

  

resource "aws_ssm_parameter" "database_name" {

name = "/mentoria/database/name"

type = "String"

value = "postgres"

tags = {

Environment = "development"

Project = "mentoria-backend"

}

}

  

resource "aws_ssm_parameter" "database_username" {

name = "/mentoria/database/username"

type = "String"

value = "postgres"

tags = {

Environment = "development"

Project = "mentoria-backend"

}

}

  

# Secret para senha do banco

resource "aws_secretsmanager_secret" "database_password" {

name = "mentoria/database/password"

description = "Senha do banco de dados PostgreSQL"

tags = {

Environment = "development"

Project = "mentoria-backend"

}

}

  

resource "aws_secretsmanager_secret_version" "database_password_value" {

secret_id = aws_secretsmanager_secret.database_password.id

secret_string = "postgres"

}

  

# Outputs para referência

output "ssm_parameters" {

value = {

database_host = aws_ssm_parameter.database_host.name

database_port = aws_ssm_parameter.database_port.name

database_name = aws_ssm_parameter.database_name.name

database_username = aws_ssm_parameter.database_username.name

}

}

  

output "secrets" {

value = {

database_password = aws_secretsmanager_secret.database_password.name

}

sensitive = true

}

```

  

**Passo 2: Aplicar infraestrutura Terraform**

```bash

# Navegar para pasta de infraestrutura

cd infra

  

# Inicializar Terraform

tflocal init

  

# Planejar mudanças

tflocal plan

  

# Aplicar infraestrutura

tflocal apply -auto-approve

  

# Verificar recursos criados

tflocal output

```

  

**Passo 3: Configurar appsettings.json**

```json

// appsettings.json

{

"AWS": {

"Region": "us-east-1",

"ServiceURL": "http://localhost:4566"

},

"ParameterStore": {

"DatabaseHost": "/mentoria/database/host",

"DatabasePort": "/mentoria/database/port",

"DatabaseName": "/mentoria/database/name",

"DatabaseUsername": "/mentoria/database/username"

},

"SecretsManager": {

"DatabasePassword": "mentoria/database/password"

},

"Logging": {

"LogLevel": {

"Default": "Information",

"Microsoft.AspNetCore": "Warning"

}

}

}

```

  

**Passo 4: Implementar Configuration Provider**

```csharp

// Configuration/AwsConfigurationProvider.cs

public class AwsConfigurationProvider

{

private readonly IAmazonSimpleSystemsManagement _ssmClient;

private readonly IAmazonSecretsManager _secretsClient;

private readonly ILogger<AwsConfigurationProvider> _logger;

  

public AwsConfigurationProvider(

IAmazonSimpleSystemsManagement ssmClient,

IAmazonSecretsManager secretsClient,

ILogger<AwsConfigurationProvider> logger)

{

_ssmClient = ssmClient;

_secretsClient = secretsClient;

_logger = logger;

}

  

public async Task<string> GetParameterAsync(string parameterName)

{

try

{

var request = new GetParameterRequest

{

Name = parameterName,

WithDecryption = true

};

  

var response = await _ssmClient.GetParameterAsync(request);

return response.Parameter.Value;

}

catch (Exception ex)

{

_logger.LogError(ex, "Erro ao buscar parâmetro {ParameterName}", parameterName);

throw;

}

}

  

public async Task<string> GetSecretAsync(string secretName)

{

try

{

var request = new GetSecretValueRequest

{

SecretId = secretName

};

  

var response = await _secretsClient.GetSecretValueAsync(request);

return response.SecretString;

}

catch (Exception ex)

{

_logger.LogError(ex, "Erro ao buscar secret {SecretName}", secretName);

throw;

}

}

  

public async Task<string> BuildConnectionStringAsync(IConfiguration configuration)

{

var host = await GetParameterAsync(configuration["ParameterStore:DatabaseHost"]);

var port = await GetParameterAsync(configuration["ParameterStore:DatabasePort"]);

var database = await GetParameterAsync(configuration["ParameterStore:DatabaseName"]);

var username = await GetParameterAsync(configuration["ParameterStore:DatabaseUsername"]);

var password = await GetSecretAsync(configuration["SecretsManager:DatabasePassword"]);

  

return $"Host={host};Port={port};Database={database};Username={username};Password={password}";

}

}

```

  

**Passo 5: Configurar no Program.cs**

```csharp

// Program.cs - Configuração AWS

builder.Services.AddAWSService<IAmazonSimpleSystemsManagement>(new AWSOptions

{

Region = RegionEndpoint.USEast1,

DefaultClientConfig = { ServiceURL = "http://localhost:4566" }

});

  

builder.Services.AddAWSService<IAmazonSecretsManager>(new AWSOptions

{

Region = RegionEndpoint.USEast1,

DefaultClientConfig = { ServiceURL = "http://localhost:4566" }

});

  

builder.Services.AddScoped<AwsConfigurationProvider>();

  

// Configurar connection string dinamicamente

builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>

{

var configProvider = serviceProvider.GetRequiredService<AwsConfigurationProvider>();

var configuration = serviceProvider.GetRequiredService<IConfiguration>();

var connectionString = configProvider.BuildConnectionStringAsync(configuration).Result;

options.UseNpgsql(connectionString);

});

```

  

**Benefícios desta abordagem:**

- ✅ **Infraestrutura como Código**: Recursos versionados e reproduzíveis

- ✅ **Segurança**: Senhas não ficam em arquivos de configuração

- ✅ **Flexibilidade**: Parâmetros podem ser alterados via Terraform

- ✅ **Simulação AWS**: Experiência próxima ao ambiente real

- ✅ **Separação de responsabilidades**: Configurações centralizadas

- ✅ **Boas práticas**: Seguindo padrões DevOps e AWS

  

**Comandos úteis para desenvolvimento:**

```bash

# Verificar parâmetros criados (após terraform apply)

terraform-local ssm get-parameters-by-path --path "/mentoria"

  

# Verificar secrets criados

terraform-local secretsmanager list-secrets

  

# Obter valor de um secret

terraform-local secretsmanager get-secret-value --secret-id "mentoria/database/password"

```

  

#### 1.3 Criar Modelos Básicos

```csharp

// Models/Entities/Conta.cs

public class Conta

{

public int Id { get; set; }

public string Numero { get; set; } = string.Empty;

public decimal Saldo { get; set; }

public DateTime CriadoEm { get; set; }

public DateTime AtualizadoEm { get; set; }

// Navigation property

public virtual ICollection<Movimento> Movimentos { get; set; } = new List<Movimento>();

}

  

// Models/Entities/Movimento.cs

public class Movimento

{

public int Id { get; set; }

public int ContaId { get; set; }

public decimal Valor { get; set; }

public TipoMovimento Tipo { get; set; }

public string Descricao { get; set; } = string.Empty;

public DateTime CriadoEm { get; set; }

// Navigation property

public virtual Conta Conta { get; set; } = null!;

}

  

// Enums/TipoMovimento.cs

public enum TipoMovimento

{

Debito = 1,

Credito = 2

}

```

  

#### 1.4 Configurar DbContext

```csharp

// Data/AppDbContext.cs

public class AppDbContext : DbContext

{

public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)

{

}

  

public DbSet<Conta> Contas { get; set; }

public DbSet<Movimento> Movimentos { get; set; }

  

protected override void OnModelCreating(ModelBuilder modelBuilder)

{

base.OnModelCreating(modelBuilder);

  

// Configuração de Conta

modelBuilder.Entity<Conta>(entity =>

{

entity.HasKey(e => e.Id);

entity.Property(e => e.Numero)

.IsRequired()

.HasMaxLength(20);

entity.Property(e => e.Saldo)

.HasPrecision(18, 2);

entity.HasIndex(e => e.Numero)

.IsUnique();

});

  

// Configuração de Movimento

modelBuilder.Entity<Movimento>(entity =>

{

entity.HasKey(e => e.Id);

entity.Property(e => e.Valor)

.HasPrecision(18, 2);

entity.Property(e => e.Descricao)

.HasMaxLength(500);

// Relacionamento com Conta

entity.HasOne(m => m.Conta)

.WithMany(c => c.Movimentos)

.HasForeignKey(m => m.ContaId)

.OnDelete(DeleteBehavior.Restrict);

});

}

}

```

  

### Fase 2: Implementação Básica (Todos os níveis)

  

#### 2.1 DTOs e Responses

```csharp

// DTOs/Requests/CriarMovimentoRequest.cs

public class CriarMovimentoRequest

{

[Required]

public int ContaId { get; set; }

[Required]

[Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]

public decimal Valor { get; set; }

[Required]

public TipoMovimento Tipo { get; set; }

[Required]

[StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]

public string Descricao { get; set; } = string.Empty;

}

  

// DTOs/Responses/MovimentoResponse.cs

public class MovimentoResponse

{

public int Id { get; set; }

public int ContaId { get; set; }

public decimal Valor { get; set; }

public TipoMovimento Tipo { get; set; }

public string Descricao { get; set; } = string.Empty;

public DateTime CriadoEm { get; set; }

public static MovimentoResponse FromEntity(Movimento movimento)

{

return new MovimentoResponse

{

Id = movimento.Id,

ContaId = movimento.ContaId,

Valor = movimento.Valor,

Tipo = movimento.Tipo,

Descricao = movimento.Descricao,

CriadoEm = movimento.CriadoEm

};

}

}

  

// DTOs/Responses/SaldoResponse.cs

public class SaldoResponse

{

public int ContaId { get; set; }

public string NumeroConta { get; set; } = string.Empty;

public decimal Valor { get; set; }

public DateTime UltimaAtualizacao { get; set; }

}

```

  

#### 2.2 Repositories (Padrão Repository)

```csharp

// Repositories/IContaRepository.cs

public interface IContaRepository

{

Task<Conta?> GetByIdAsync(int id);

Task<Conta?> GetByNumeroAsync(string numero);

Task<Conta> CreateAsync(Conta conta);

Task UpdateAsync(Conta conta);

Task<bool> ExistsAsync(int id);

}

  

// Repositories/ContaRepository.cs

public class ContaRepository : IContaRepository

{

private readonly AppDbContext _context;

  

public ContaRepository(AppDbContext context)

{

_context = context;

}

  

public async Task<Conta?> GetByIdAsync(int id)

{

return await _context.Contas

.Include(c => c.Movimentos)

.FirstOrDefaultAsync(c => c.Id == id);

}

  

public async Task<Conta?> GetByNumeroAsync(string numero)

{

return await _context.Contas

.FirstOrDefaultAsync(c => c.Numero == numero);

}

  

public async Task<Conta> CreateAsync(Conta conta)

{

conta.CriadoEm = DateTime.UtcNow;

conta.AtualizadoEm = DateTime.UtcNow;

_context.Contas.Add(conta);

await _context.SaveChangesAsync();

return conta;

}

  

public async Task UpdateAsync(Conta conta)

{

conta.AtualizadoEm = DateTime.UtcNow;

_context.Contas.Update(conta);

await _context.SaveChangesAsync();

}

  

public async Task<bool> ExistsAsync(int id)

{

return await _context.Contas.AnyAsync(c => c.Id == id);

}

}

```

  

#### 2.3 Services (Domain Services)

```csharp

// Services/ISaldoService.cs

public interface ISaldoService

{

Task<SaldoResponse> GetSaldoAsync(int contaId);

Task<SaldoResponse> GetSaldoByNumeroContaAsync(string numeroConta);

}

  

// Services/SaldoService.cs

public class SaldoService : ISaldoService

{

private readonly IContaRepository _contaRepository;

private readonly ILogger<SaldoService> _logger;

  

public SaldoService(IContaRepository contaRepository, ILogger<SaldoService> logger)

{

_contaRepository = contaRepository;

_logger = logger;

}

  

public async Task<SaldoResponse> GetSaldoAsync(int contaId)

{

_logger.LogInformation("Buscando saldo para conta {ContaId}", contaId);

  

var conta = await _contaRepository.GetByIdAsync(contaId);

if (conta == null)

{

throw new NotFoundException($"Conta com ID {contaId} não encontrada");

}

  

return new SaldoResponse

{

ContaId = conta.Id,

NumeroConta = conta.Numero,

Valor = conta.Saldo,

UltimaAtualizacao = conta.AtualizadoEm

};

}

  

public async Task<SaldoResponse> GetSaldoByNumeroContaAsync(string numeroConta)

{

_logger.LogInformation("Buscando saldo para conta número {NumeroConta}", numeroConta);

  

var conta = await _contaRepository.GetByNumeroAsync(numeroConta);

if (conta == null)

{

throw new NotFoundException($"Conta com número {numeroConta} não encontrada");

}

  

return new SaldoResponse

{

ContaId = conta.Id,

NumeroConta = conta.Numero,

Valor = conta.Saldo,

UltimaAtualizacao = conta.AtualizadoEm

};

}

}

```

  

#### 2.4 Controllers Básicos

```csharp

// Controllers/SaldosController.cs

[ApiController]

[Route("api/[controller]")]

public class SaldosController : ControllerBase

{

private readonly ISaldoService _saldoService;

private readonly ILogger<SaldosController> _logger;

  

public SaldosController(ISaldoService saldoService, ILogger<SaldosController> logger)

{

_saldoService = saldoService;

_logger = logger;

}

  

/// <summary>

/// Obtém o saldo de uma conta pelo ID

/// </summary>

/// <param name="contaId">ID da conta</param>

/// <returns>Saldo da conta</returns>

[HttpGet("{contaId:int}")]

[ProducesResponseType(typeof(SaldoResponse), StatusCodes.Status200OK)]

[ProducesResponseType(StatusCodes.Status404NotFound)]

[ProducesResponseType(StatusCodes.Status500InternalServerError)]

public async Task<ActionResult<SaldoResponse>> GetSaldo(int contaId)

{

try

{

var saldo = await _saldoService.GetSaldoAsync(contaId);

return Ok(saldo);

}

catch (NotFoundException ex)

{

_logger.LogWarning("Conta não encontrada: {Message}", ex.Message);

return NotFound(ex.Message);

}

catch (Exception ex)

{

_logger.LogError(ex, "Erro ao buscar saldo da conta {ContaId}", contaId);

return StatusCode(500, "Erro interno do servidor");

}

}

  

/// <summary>

/// Obtém o saldo de uma conta pelo número

/// </summary>

/// <param name="numeroConta">Número da conta</param>

/// <returns>Saldo da conta</returns>

[HttpGet("numero/{numeroConta}")]

[ProducesResponseType(typeof(SaldoResponse), StatusCodes.Status200OK)]

[ProducesResponseType(StatusCodes.Status404NotFound)]

public async Task<ActionResult<SaldoResponse>> GetSaldoByNumero(string numeroConta)

{

try

{

var saldo = await _saldoService.GetSaldoByNumeroContaAsync(numeroConta);

return Ok(saldo);

}

catch (NotFoundException ex)

{

return NotFound(ex.Message);

}

}

}

```

  

### Fase 3: Serviços de Negócio (Plenos/Seniores)

  

#### 3.1 Serviço de Movimentações

```csharp

// Services/IMovimentoService.cs

public interface IMovimentoService

{

Task<MovimentoResponse> CriarMovimentoAsync(CriarMovimentoRequest request);

Task<IEnumerable<MovimentoResponse>> GetMovimentosByContaAsync(int contaId);

Task<MovimentoResponse?> GetMovimentoByIdAsync(int movimentoId);

}

  

// Services/MovimentoService.cs

public class MovimentoService : IMovimentoService

{

private readonly AppDbContext _context;

private readonly IEventBus _eventBus;

private readonly ILogger<MovimentoService> _logger;

  

public MovimentoService(

AppDbContext context,

IEventBus eventBus,

ILogger<MovimentoService> logger)

{

_context = context;

_eventBus = eventBus;

_logger = logger;

}

  

public async Task<MovimentoResponse> CriarMovimentoAsync(CriarMovimentoRequest request)

{

using var transaction = await _context.Database.BeginTransactionAsync();

try

{

// Validar se a conta existe

var conta = await _context.Contas.FindAsync(request.ContaId);

if (conta == null)

{

throw new NotFoundException($"Conta {request.ContaId} não encontrada");

}

  

// Validar saldo para débito

if (request.Tipo == TipoMovimento.Debito && conta.Saldo < request.Valor)

{

throw new InvalidOperationException("Saldo insuficiente");

}

  

// Criar movimento

var movimento = new Movimento

{

ContaId = request.ContaId,

Valor = request.Valor,

Tipo = request.Tipo,

Descricao = request.Descricao,

CriadoEm = DateTime.UtcNow

};

  

_context.Movimentos.Add(movimento);

  

// Atualizar saldo da conta

if (request.Tipo == TipoMovimento.Credito)

conta.Saldo += request.Valor;

else

conta.Saldo -= request.Valor;

  

conta.AtualizadoEm = DateTime.UtcNow;

  

await _context.SaveChangesAsync();

  

// Publicar evento

var evento = new MovimentoCriadoEvent

{

MovimentoId = movimento.Id,

ContaId = movimento.ContaId,

Valor = movimento.Valor,

Tipo = movimento.Tipo,

NovoSaldo = conta.Saldo,

CriadoEm = movimento.CriadoEm

};

  

await _eventBus.PublishAsync(evento);

  

await transaction.CommitAsync();

  

_logger.LogInformation("Movimento {MovimentoId} criado com sucesso para conta {ContaId}",

movimento.Id, movimento.ContaId);

  

return MovimentoResponse.FromEntity(movimento);

}

catch

{

await transaction.RollbackAsync();

throw;

}

}

  

public async Task<IEnumerable<MovimentoResponse>> GetMovimentosByContaAsync(int contaId)

{

var movimentos = await _context.Movimentos

.Where(m => m.ContaId == contaId)

.OrderByDescending(m => m.CriadoEm)

.ToListAsync();

  

return movimentos.Select(MovimentoResponse.FromEntity);

}

  

public async Task<MovimentoResponse?> GetMovimentoByIdAsync(int movimentoId)

{

var movimento = await _context.Movimentos.FindAsync(movimentoId);

return movimento != null ? MovimentoResponse.FromEntity(movimento) : null;

}

}

```

  

#### 3.2 Controller de Movimentações

```csharp

// Controllers/MovimentosController.cs

[ApiController]

[Route("api/[controller]")]

public class MovimentosController : ControllerBase

{

private readonly IMovimentoService _movimentoService;

private readonly ILogger<MovimentosController> _logger;

  

public MovimentosController(

IMovimentoService movimentoService,

ILogger<MovimentosController> logger)

{

_movimentoService = movimentoService;

_logger = logger;

}

  

/// <summary>

/// Cria uma nova movimentação bancária

/// </summary>

[HttpPost]

[ProducesResponseType(typeof(MovimentoResponse), StatusCodes.Status201Created)]

[ProducesResponseType(StatusCodes.Status400BadRequest)]

[ProducesResponseType(StatusCodes.Status404NotFound)]

[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]

public async Task<ActionResult<MovimentoResponse>> CriarMovimento([FromBody] CriarMovimentoRequest request)

{

try

{

var movimento = await _movimentoService.CriarMovimentoAsync(request);

return CreatedAtAction(nameof(GetMovimento), new { id = movimento.Id }, movimento);

}

catch (NotFoundException ex)

{

return NotFound(ex.Message);

}

catch (InvalidOperationException ex)

{

return UnprocessableEntity(ex.Message);

}

}

  

/// <summary>

/// Obtém uma movimentação por ID

/// </summary>

[HttpGet("{id:int}")]

[ProducesResponseType(typeof(MovimentoResponse), StatusCodes.Status200OK)]

[ProducesResponseType(StatusCodes.Status404NotFound)]

public async Task<ActionResult<MovimentoResponse>> GetMovimento(int id)

{

var movimento = await _movimentoService.GetMovimentoByIdAsync(id);

return movimento != null ? Ok(movimento) : NotFound();

}

  

/// <summary>

/// Obtém todas as movimentações de uma conta

/// </summary>

[HttpGet("conta/{contaId:int}")]

[ProducesResponseType(typeof(IEnumerable<MovimentoResponse>), StatusCodes.Status200OK)]

public async Task<ActionResult<IEnumerable<MovimentoResponse>>> GetMovimentosByConta(int contaId)

{

var movimentos = await _movimentoService.GetMovimentosByContaAsync(contaId);

return Ok(movimentos);

}

}

```

  

### Fase 4: Event Bus e Comunicação Assíncrona

  

#### 4.1 Event Bus Implementation

```csharp

// Services/IEventBus.cs

public interface IEventBus

{

Task PublishAsync<T>(T @event) where T : class;

}

  

// Services/SqsEventBus.cs

public class SqsEventBus : IEventBus

{

private readonly IAmazonSQS _sqsClient;

private readonly ILogger<SqsEventBus> _logger;

private readonly Dictionary<Type, string> _queueMappings;

  

public SqsEventBus(IAmazonSQS sqsClient, ILogger<SqsEventBus> logger)

{

_sqsClient = sqsClient;

_logger = logger;

_queueMappings = new Dictionary<Type, string>

{

{ typeof(MovimentoCriadoEvent), "movimentos-queue" },

{ typeof(SaldoAtualizadoEvent), "saldos-queue" }

};

}

  

public async Task PublishAsync<T>(T @event) where T : class

{

try

{

var eventType = typeof(T);

if (!_queueMappings.TryGetValue(eventType, out var queueName))

{

throw new InvalidOperationException($"Queue mapping not found for event type {eventType.Name}");

}

  

var queueUrl = await GetQueueUrlAsync(queueName);

var message = JsonSerializer.Serialize(@event);

  

var request = new SendMessageRequest

{

QueueUrl = queueUrl,

MessageBody = message,

MessageAttributes = new Dictionary<string, MessageAttributeValue>

{

["EventType"] = new MessageAttributeValue

{

DataType = "String",

StringValue = eventType.Name

}

}

};

  

await _sqsClient.SendMessageAsync(request);

_logger.LogInformation("Event {EventType} published successfully to queue {QueueName}",

eventType.Name, queueName);

}

catch (Exception ex)

{

_logger.LogError(ex, "Failed to publish event {EventType}", typeof(T).Name);

throw;

}

}

  

private async Task<string> GetQueueUrlAsync(string queueName)

{

try

{

var response = await _sqsClient.GetQueueUrlAsync(queueName);

return response.QueueUrl;

}

catch (QueueDoesNotExistException)

{

// Auto-create queue if it doesn't exist

var createResponse = await _sqsClient.CreateQueueAsync(queueName);

return createResponse.QueueUrl;

}

}

}

```

  

#### 4.2 Background Workers

```csharp

// Workers/MovimentoWorker.cs

public class MovimentoWorker : BackgroundService

{

private readonly IServiceProvider _serviceProvider;

private readonly IAmazonSQS _sqsClient;

private readonly ILogger<MovimentoWorker> _logger;

private readonly string _queueUrl;

  

public MovimentoWorker(

IServiceProvider serviceProvider,

IAmazonSQS sqsClient,

ILogger<MovimentoWorker> logger)

{

_serviceProvider = serviceProvider;

_sqsClient = sqsClient;

_logger = logger;

_queueUrl = "http://localhost:4566/000000000000/movimentos-queue";

}

  

protected override async Task ExecuteAsync(CancellationToken stoppingToken)

{

while (!stoppingToken.IsCancellationRequested)

{

try

{

var request = new ReceiveMessageRequest

{

QueueUrl = _queueUrl,

MaxNumberOfMessages = 10,

WaitTimeSeconds = 20,

MessageAttributeNames = new List<string> { "All" }

};

  

var response = await _sqsClient.ReceiveMessageAsync(request, stoppingToken);

  

foreach (var message in response.Messages)

{

await ProcessMessageAsync(message, stoppingToken);

}

}

catch (Exception ex)

{

_logger.LogError(ex, "Error processing messages from queue");

await Task.Delay(5000, stoppingToken);

}

}

}

  

private async Task ProcessMessageAsync(Message message, CancellationToken cancellationToken)

{

try

{

using var scope = _serviceProvider.CreateScope();

var eventType = message.MessageAttributes["EventType"].StringValue;

switch (eventType)

{

case nameof(MovimentoCriadoEvent):

var movimentoEvent = JsonSerializer.Deserialize<MovimentoCriadoEvent>(message.Body);

await ProcessMovimentoCriadoAsync(movimentoEvent!, scope.ServiceProvider);

break;

default:

_logger.LogWarning("Unknown event type: {EventType}", eventType);

break;

}

  

// Delete message after successful processing

await _sqsClient.DeleteMessageAsync(_queueUrl, message.ReceiptHandle, cancellationToken);

_logger.LogInformation("Message processed successfully: {MessageId}", message.MessageId);

}

catch (Exception ex)

{

_logger.LogError(ex, "Error processing message {MessageId}", message.MessageId);

}

}

  

private async Task ProcessMovimentoCriadoAsync(MovimentoCriadoEvent evento, IServiceProvider serviceProvider)

{

var notificationService = serviceProvider.GetRequiredService<INotificationService>();

// Exemplo: Enviar notificação sobre o movimento

await notificationService.SendMovimentoNotificationAsync(evento);

_logger.LogInformation("Processed MovimentoCriado event for conta {ContaId}", evento.ContaId);

}

}

```

  

---

  

## 🧪 Casos de Uso Detalhados

  

### Caso de Uso 1: Recuperar Saldo

  

**Descrição**: Como usuário, quero consultar o saldo atual de uma conta bancária.

  

**Pré-condições**:

- Conta deve existir no sistema

- Conta deve estar ativa

  

**Fluxo Principal**:

1. Cliente faz requisição GET para `/api/saldos/{contaId}`

2. Sistema valida se a conta existe

3. Sistema retorna saldo atual e data da última atualização

  

**Fluxos Alternativos**:

- **2a.** Conta não existe: Retorna 404 Not Found

- **2b.** Erro de sistema: Retorna 500 Internal Server Error

  

**Pós-condições**:

- Nenhuma alteração de estado

- Log da consulta registrado

  

**Exemplo de Implementação**:

```csharp

[HttpGet("{contaId:int}")]

public async Task<ActionResult<SaldoResponse>> GetSaldo(int contaId)

{

var saldo = await _saldoService.GetSaldoAsync(contaId);

return Ok(saldo);

}

```

  

### Caso de Uso 2: Movimentar (Débito/Crédito)

  

**Descrição**: Como usuário, quero registrar uma movimentação bancária (débito ou crédito).

  

**Pré-condições**:

- Conta deve existir e estar ativa

- Para débitos: Saldo deve ser suficiente

- Valor deve ser maior que zero

  

**Fluxo Principal**:

1. Cliente envia POST para `/api/movimentos` com dados da movimentação

2. Sistema valida dados de entrada

3. Sistema verifica se conta existe

4. Sistema verifica saldo (se débito)

5. Sistema inicia transação de banco

6. Sistema cria registro de movimento

7. Sistema atualiza saldo da conta

8. Sistema confirma transação

9. Sistema publica evento MovimentoCriado

10. Sistema retorna dados do movimento criado

  

**Fluxos Alternativos**:

- **2a.** Dados inválidos: Retorna 400 Bad Request

- **3a.** Conta não existe: Retorna 404 Not Found

- **4a.** Saldo insuficiente: Retorna 422 Unprocessable Entity

- **6-8a.** Erro na transação: Rollback e retorna 500

  

**Pós-condições**:

- Movimento registrado no sistema

- Saldo da conta atualizado

- Evento publicado na fila

- Logs registrados

  

---

  

## 🧪 Testes

  

### Testes Unitários

  

#### Testando Services

```csharp

// Tests/Services/SaldoServiceTests.cs

public class SaldoServiceTests

{

private readonly Mock<IContaRepository> _contaRepositoryMock;

private readonly Mock<ILogger<SaldoService>> _loggerMock;

private readonly SaldoService _saldoService;

  

public SaldoServiceTests()

{

_contaRepositoryMock = new Mock<IContaRepository>();

_loggerMock = new Mock<ILogger<SaldoService>>();

_saldoService = new SaldoService(_contaRepositoryMock.Object, _loggerMock.Object);

}

  

[Fact]

public async Task GetSaldoAsync_ContaExiste_DeveRetornarSaldo()

{

// Arrange

var contaId = 1;

var conta = new Conta

{

Id = contaId,

Numero = "12345",

Saldo = 1000.00m,

AtualizadoEm = DateTime.UtcNow

};

  

_contaRepositoryMock.Setup(x => x.GetByIdAsync(contaId))

.ReturnsAsync(conta);

  

// Act

var resultado = await _saldoService.GetSaldoAsync(contaId);

  

// Assert

Assert.NotNull(resultado);

Assert.Equal(contaId, resultado.ContaId);

Assert.Equal(conta.Numero, resultado.NumeroConta);

Assert.Equal(conta.Saldo, resultado.Valor);

Assert.Equal(conta.AtualizadoEm, resultado.UltimaAtualizacao);

}

  

[Fact]

public async Task GetSaldoAsync_ContaNaoExiste_DeveLancarNotFoundException()

{

// Arrange

var contaId = 1;

_contaRepositoryMock.Setup(x => x.GetByIdAsync(contaId))

.ReturnsAsync((Conta?)null);

  

// Act & Assert

var exception = await Assert.ThrowsAsync<NotFoundException>(

() => _saldoService.GetSaldoAsync(contaId));

Assert.Contains($"Conta com ID {contaId} não encontrada", exception.Message);

}

}

```

  

#### Testando Services

```csharp

// Tests/Services/MovimentoServiceTests.cs

public class MovimentoServiceTests : IDisposable

{

private readonly AppDbContext _context;

private readonly Mock<IEventBus> _eventBusMock;

private readonly Mock<ILogger<MovimentoService>> _loggerMock;

private readonly MovimentoService _movimentoService;

  

public MovimentoServiceTests()

{

var options = new DbContextOptionsBuilder<AppDbContext>()

.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())

.Options;

  

_context = new AppDbContext(options);

_eventBusMock = new Mock<IEventBus>();

_loggerMock = new Mock<ILogger<MovimentoService>>();

_movimentoService = new MovimentoService(_context, _eventBusMock.Object, _loggerMock.Object);

  

SeedDatabase();

}

  

[Fact]

public async Task CriarMovimentoAsync_MovimentoCredito_DeveAtualizarSaldoCorretamente()

{

// Arrange

var request = new CriarMovimentoRequest

{

ContaId = 1,

Valor = 500.00m,

Tipo = TipoMovimento.Credito,

Descricao = "Depósito teste"

};

  

// Act

var resultado = await _movimentoService.CriarMovimentoAsync(request);

  

// Assert

Assert.NotNull(resultado);

Assert.Equal(request.ContaId, resultado.ContaId);

Assert.Equal(request.Valor, resultado.Valor);

  

// Verificar se saldo foi atualizado

var conta = await _context.Contas.FindAsync(1);

Assert.Equal(1500.00m, conta!.Saldo); // 1000 + 500

  

// Verificar se evento foi publicado

_eventBusMock.Verify(x => x.PublishAsync(It.IsAny<MovimentoCriadoEvent>()), Times.Once);

}

  

[Fact]

public async Task CriarMovimentoAsync_MovimentoDebitoSaldoInsuficiente_DeveLancarException()

{

// Arrange

var request = new CriarMovimentoRequest

{

ContaId = 1,

Valor = 2000.00m, // Maior que o saldo de 1000

Tipo = TipoMovimento.Debito,

Descricao = "Saque teste"

};

  

// Act & Assert

await Assert.ThrowsAsync<InvalidOperationException>(

() => _movimentoService.CriarMovimentoAsync(request));

  

// Verificar que nenhum evento foi publicado

_eventBusMock.Verify(x => x.PublishAsync(It.IsAny<MovimentoCriadoEvent>()), Times.Never);

}

  

private void SeedDatabase()

{

var conta = new Conta

{

Id = 1,

Numero = "12345",

Saldo = 1000.00m,

CriadoEm = DateTime.UtcNow,

AtualizadoEm = DateTime.UtcNow

};

  

_context.Contas.Add(conta);

_context.SaveChanges();

}

  

public void Dispose()

{

_context.Dispose();

}

}

```

  

### Testes de Integração

  

```csharp

// Tests/Integration/MovimentosControllerTests.cs

public class MovimentosControllerTests : IClassFixture<WebApplicationFactory<Program>>

{

private readonly WebApplicationFactory<Program> _factory;

private readonly HttpClient _client;

  

public MovimentosControllerTests(WebApplicationFactory<Program> factory)

{

_factory = factory.WithWebHostBuilder(builder =>

{

builder.ConfigureServices(services =>

{

// Remover o DbContext real

var descriptor = services.SingleOrDefault(

d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

if (descriptor != null) services.Remove(descriptor);

  

// Adicionar DbContext em memória

services.AddDbContext<AppDbContext>(options =>

{

options.UseInMemoryDatabase("TestDb");

});

  

// Mock do EventBus para testes

services.AddScoped<IEventBus, MockEventBus>();

});

});

_client = _factory.CreateClient();

}

  

[Fact]

public async Task POST_CriarMovimento_DeveRetornar201()

{

// Arrange

var request = new CriarMovimentoRequest

{

ContaId = 1,

Valor = 100.00m,

Tipo = TipoMovimento.Credito,

Descricao = "Teste de integração"

};

  

var json = JsonSerializer.Serialize(request);

var content = new StringContent(json, Encoding.UTF8, "application/json");

  

// Act

var response = await _client.PostAsync("/api/movimentos", content);

  

// Assert

response.StatusCode.Should().Be(HttpStatusCode.Created);

var responseContent = await response.Content.ReadAsStringAsync();

var movimento = JsonSerializer.Deserialize<MovimentoResponse>(responseContent);

movimento.Should().NotBeNull();

movimento.ContaId.Should().Be(request.ContaId);

movimento.Valor.Should().Be(request.Valor);

}

}

  

// Tests/Mocks/MockEventBus.cs

public class MockEventBus : IEventBus

{

public Task PublishAsync<T>(T @event) where T : class

{

// Mock implementation - doesn't actually publish

return Task.CompletedTask;

}

}

```

  

---

  

## 🔧 Configuração do Program.cs

  

```csharp

// Program.cs completo

using Amazon.SQS;

using Microsoft.EntityFrameworkCore;

using System.Reflection;

  

var builder = WebApplication.CreateBuilder(args);

  

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>

{

c.SwaggerDoc("v1", new() { Title = "Mentoria API", Version = "v1" });

// Include XML comments

var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

c.IncludeXmlComments(xmlPath);

});

  

// Database

builder.Services.AddDbContext<AppDbContext>(options =>

options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

  

// AWS SQS (LocalStack)

builder.Services.AddSingleton<IAmazonSQS>(provider =>

{

var config = new AmazonSQSConfig

{

ServiceURL = "http://localhost:4566",

UseHttp = true,

AuthenticationRegion = "us-east-1"

};

return new AmazonSQSClient("dummy", "dummy", config);

});

  

// Application Services

builder.Services.AddScoped<IContaRepository, ContaRepository>();

builder.Services.AddScoped<IMovimentoRepository, MovimentoRepository>();

builder.Services.AddScoped<ISaldoService, SaldoService>();

builder.Services.AddScoped<IMovimentoService, MovimentoService>();

builder.Services.AddScoped<IEventBus, SqsEventBus>();

  

// Background Services

builder.Services.AddHostedService<MovimentoWorker>();

  

// CORS

builder.Services.AddCors(options =>

{

options.AddPolicy("AllowAll",

builder => builder

.AllowAnyOrigin()

.AllowAnyMethod()

.AllowAnyHeader());

});

  

// Logging

builder.Services.AddLogging(logging =>

{

logging.AddConsole();

logging.AddDebug();

});

  

var app = builder.Build();

  

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())

{

app.UseSwagger();

app.UseSwaggerUI();

}

  

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

  

// Ensure database is created

using (var scope = app.Services.CreateScope())

{

var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

context.Database.EnsureCreated();

}

  

app.Run();

  

// Make the implicit Program class public so test projects can access it

public partial class Program { }

```

  

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

  

---

  

## 📖 Recursos Adicionais

  

### Documentação

- [.NET 8 Documentation](https://docs.microsoft.com/en-us/dotnet/)

- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

- [MediatR](https://github.com/jbogard/MediatR)

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