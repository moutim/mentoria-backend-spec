using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using NotificacoesWorker.Handlers;
using NotificacoesWorker.Models.Interfaces;

namespace NotificacoesWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ISqsService _sqsService;
    private readonly IMessageHandler _messageHandler;
    private readonly IConfiguration _configuration;

    public Worker(
        ILogger<Worker> logger, 
        ISqsService sqsService, 
        IMessageHandler messageHandler,
        IConfiguration configuration)
    {
        _logger = logger;
        _sqsService = sqsService;
        _messageHandler = messageHandler;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker iniciado em: {time}", DateTimeOffset.Now);

        var pollingInterval = TimeSpan.FromSeconds(
            int.Parse(_configuration["Worker:PollingIntervalSeconds"]!));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Buscando mensagens na fila SQS...");

                var messages = await _sqsService.ReceberMensagensAsync(stoppingToken);

                if (messages.Any())
                {
                    foreach (var message in messages)
                    {
                        try
                        {
                            await _messageHandler.ProcessarMensagemAsync(message.Body, stoppingToken);
                            
                            await _sqsService.DeletarMensagemAsync(message.ReceiptHandle, stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Erro ao processar mensagem. MessageId: {MessageId}", message.MessageId);
                        }
                    }
                }
                else
                {
                    _logger.LogInformation("Nenhuma mensagem encontrada. Aguardando próximo polling...");
                }

                await Task.Delay(pollingInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no loop principal do Worker");
                await Task.Delay(pollingInterval, stoppingToken);
            }
        }

        _logger.LogInformation("Worker finalizado em: {time}", DateTimeOffset.Now);
    }
}
