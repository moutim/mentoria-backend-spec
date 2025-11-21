using Amazon.SQS;
using NotificacoesWorker;
using NotificacoesWorker.Handlers;
using NotificacoesWorker.Models.Interfaces;
using NotificacoesWorker.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IAmazonSQS>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var clientConfig = new AmazonSQSConfig
    {
        ServiceURL = config["AWS:ServiceURL"],
        AuthenticationRegion = config["AWS:Region"]
    };
    return new AmazonSQSClient(clientConfig);
});

builder.Services.AddSingleton<ISqsService, SqsService>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<IMessageHandler, MovimentacaoBancariaHandler>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();