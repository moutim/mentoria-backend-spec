using Amazon.SQS;
using Amazon.SQS.Model;
using NotificacoesWorker.Models.Interfaces;

namespace NotificacoesWorker.Services;

public class SqsService : ISqsService
{
    private readonly IAmazonSQS _sqsClient;
    private readonly ILogger<SqsService> _logger;
    private readonly string _queueUrl;
    private readonly int _maxNumberOfMessages;
    private readonly int _waitTimeSeconds;

    public SqsService(IAmazonSQS sqsClient, ILogger<SqsService> logger, IConfiguration configuration)
    {
        _sqsClient = sqsClient;
        _logger = logger;
        _queueUrl = configuration["AWS:QueueUrl"]!;
        _maxNumberOfMessages = int.Parse(configuration["Worker:MaxNumberOfMessages"]!);
        _waitTimeSeconds = int.Parse(configuration["Worker:WaitTimeSeconds"]!);
    }

    public async Task<List<Message>> ReceberMensagensAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new ReceiveMessageRequest
            {
                QueueUrl = _queueUrl,
                MaxNumberOfMessages = _maxNumberOfMessages,
                WaitTimeSeconds = _waitTimeSeconds,
                MessageAttributeNames = new List<string> { "All" },
                MessageSystemAttributeNames = new List<string> { "All" }
            };

            var response = await _sqsClient.ReceiveMessageAsync(request, cancellationToken);
            
            if (response.Messages.Any())
            {
                _logger.LogInformation("Recebidas {Count} mensagens da fila SQS", response.Messages.Count);
            }

            return response.Messages;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao receber mensagens da fila SQS");
            throw;
        }
    }

    public async Task DeletarMensagemAsync(string receiptHandle, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new DeleteMessageRequest
            {
                QueueUrl = _queueUrl,
                ReceiptHandle = receiptHandle
            };

            await _sqsClient.DeleteMessageAsync(request, cancellationToken);
            _logger.LogInformation("Mensagem deletada da fila SQS com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar mensagem da fila SQS");
            throw;
        }
    }
}

