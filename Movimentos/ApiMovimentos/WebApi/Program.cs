using System.Reflection;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configuração do Swagger/OpenAPI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API de Movimentos Bancários",
        Version = "v1",
        Description = "API para gerenciamento de contas e movimentações bancárias",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Suporte",
            Email = "suporte@apimovement.com"
        }
    });

    // Adiciona comentários XML se você quiser documentar com /// comments
    var xmlFile = "WebApi.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Configure DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped<IContaRepository, ContaRepository>();
builder.Services.AddScoped<IMovimentoRepository, MovimentoRepository>();

// Register MediatR
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(Assembly.Load("Application")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Create database and apply migrations
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
