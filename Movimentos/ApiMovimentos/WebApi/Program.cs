using Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMemoryCache();

// Configuração de injeção de dependência
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddMediatorHandlers();

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
});


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


app.Run();
