# Como Usar Parameter Store com AppSettings

## 1. No appsettings.json - Define valores locais (fallback)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=movimentos;Username=postgres;Password=secret"
  },
  "AppSettings": {
    "JwtSecret": "local-secret-key",
    "ApiKey": "local-api-key",
    "Environment": "Development"
  }
}
```

## 2. No Program.cs - Carrega do Parameter Store

```csharp
using Infrastructure.DependencyInjection;
using Infrastructure.Services;
using Infrastructure.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddParameterStore();

// Obter valores do Parameter Store
var parameterStore = builder.Services.BuildServiceProvider().GetRequiredService<IParameterStoreService>();

// Atualizar appsettings com valores do AWS (opcional, para valores sensíveis)
try
{
    var jwtSecret = await ParameterStoreHelper.GetParameter(
        parameterStore,
        ParameterStoreHelper.ParameterNames.JwtSecret
    );
    builder.Configuration["AppSettings:JwtSecret"] = jwtSecret;
    
    var connection = await ParameterStoreHelper.GetParameter(
        parameterStore,
        ParameterStoreHelper.ParameterNames.DatabaseConnection
    );
    builder.Configuration["ConnectionStrings:DefaultConnection"] = connection;
}
catch (Exception ex)
{
    Console.WriteLine($"Warning: Could not load from Parameter Store: {ex.Message}");
    // Usa valores do appsettings.json como fallback
}

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ... resto da configuração
```

## 3. Em um Service - Usar configuração

```csharp
public class MeuService
{
    private readonly IConfiguration _configuration;
    private readonly IParameterStoreService _parameterStore;
    
    public MeuService(IConfiguration configuration, IParameterStoreService parameterStore)
    {
        _configuration = configuration;
        _parameterStore = parameterStore;
    }
    
    public async Task MeuMetodo()
    {
        // Opção 1: Usar do appsettings (carregado no Program.cs)
        var jwtSecret = _configuration["AppSettings:JwtSecret"];
        
        // Opção 2: Buscar diretamente do Parameter Store
        var apiKey = await ParameterStoreHelper.GetParameter(
            _parameterStore,
            ParameterStoreHelper.ParameterNames.ApiKey
        );
    }
}
```

## 4. Criar classe Options Pattern

```csharp
// AppOptions.cs
public class AppSettings
{
    public string JwtSecret { get; set; }
    public string ApiKey { get; set; }
    public string Environment { get; set; }
}
```

## 5. No Program.cs - Usar Options Pattern

```csharp
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
```

## 6. Em um Controller - Usar AppSettings

```csharp
[ApiController]
[Route("api/[controller]")]
public class MyController : ControllerBase
{
    private readonly IOptionsMonitor<AppSettings> _appSettings;
    
    public MyController(IOptionsMonitor<AppSettings> appSettings)
    {
        _appSettings = appSettings;
    }
    
    [HttpGet]
    public IActionResult Get()
    {
        var settings = _appSettings.CurrentValue;
        return Ok(new { settings.Environment });
    }
}
```

## Fluxo Resumido

```
appsettings.json (valores locais)
        ↓
Program.cs carrega Parameter Store
        ↓
Sobrescreve configuration com valores do AWS (se disponível)
        ↓
Services usam via IConfiguration ou IOptionsMonitor
```

## No Terraform - Definir parâmetros

```hcl
# Infrastructure/terraform/parameters.tf

resource "aws_ssm_parameter" "jwt_secret" {
  name  = "/movimentos/security/jwt-secret"
  type  = "SecureString"
  value = var.movimentos_jwt_secret
}

resource "aws_ssm_parameter" "db_connection" {
  name  = "/movimentos/database/connection"
  type  = "String"
  value = "Host=${var.movimentos_db_host};Port=5432;Database=movimentos;Username=${var.movimentos_db_username};Password=${var.movimentos_db_password}"
}
```

