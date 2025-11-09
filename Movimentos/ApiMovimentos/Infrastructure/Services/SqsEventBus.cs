using Amazon.SQS;
using Amazon.SQS.Model;
using Application.Interfaces;
using Domain.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Services;

public class SqsEventBus : IEventBus
{
    private readonly IAmazonSQS _sqsClient;
    private readonly ILogger<SqsEventBus> _logger;
    private readonly IConfiguration _configuration;
    private readonly Dictionary<Type, string> _queueMappings;

    public SqsEventBus(
        IAmazonSQS sqsClient,
        ILogger<SqsEventBus> logger,
        IConfiguration configuration)
    {
        _sqsClient = sqsClient ?? throw new ArgumentNullException(nameof(sqsClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        
        _queueMappings = new Dictionary<Type, string>
        {
            { 
                typeof(MovimentoCriadoEvent), 
                configuration["AWS:SQS:QueueUrls:NotificacoesQueue"] 
                ?? "http://sqs.us-east-1.localhost.localstack.cloud:4566/000000000000/notificacoes-queue"
            }
        };
    }

    public async Task PublishAsync<T>(T @event) where T : class
    {
        try
        {
            var eventType = typeof(T);
            
            _logger.LogInformation(
                "Iniciando publicação do evento {EventType}", 
                eventType.Name
            );

            if (!_queueMappings.TryGetValue(eventType, out var queueUrl))
            {
                var errorMessage = $"Mapeamento de fila não encontrado para o evento {eventType.Name}. " +
                                 $"Verifique o dicionário _queueMappings em SqsEventBus.";
                
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
            
            _logger.LogDebug(
                "Fila identificada: {QueueUrl}",  
                queueUrl
            );

            var messageBody = JsonSerializer.Serialize(@event, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });

            _logger.LogDebug(
                "Evento serializado para JSON: {MessageBody}", 
                messageBody
            );

            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = queueUrl,
                MessageBody = messageBody,
                MessageAttributes = new Dictionary<string, MessageAttributeValue>
                {
                    ["EventType"] = new MessageAttributeValue
                    {
                        DataType = "String",
                        StringValue = eventType.Name
                    },
                    ["PublishedAt"] = new MessageAttributeValue
                    {
                        DataType = "String",
                        StringValue = DateTime.UtcNow.ToString("O")
                    },
                    ["Source"] = new MessageAttributeValue
                    {
                        DataType = "String",
                        StringValue = "ApiMovimentos"
                    },
                    ["Version"] = new MessageAttributeValue
                    {
                        DataType = "String",
                        StringValue = "1.0"
                    }
                }
            };

            var response = await _sqsClient.SendMessageAsync(sendMessageRequest);

            _logger.LogInformation(
                "✅ Evento {EventType} publicado com sucesso na fila {QueueUrl}. " +
                "MessageId: {MessageId}, MD5: {MD5}",
                eventType.Name,
                queueUrl,
                response.MessageId,
                response.MD5OfMessageBody
            );
        }
        catch (AmazonSQSException sqsEx)
        {
            _logger.LogError(
                sqsEx,
                "❌ Erro SQS ao publicar evento {EventType}. " +
                "ErrorCode: {ErrorCode}, StatusCode: {StatusCode}",
                typeof(T).Name,
                sqsEx.ErrorCode,
                sqsEx.StatusCode
            );
            
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "❌ Erro inesperado ao publicar evento {EventType}",
                typeof(T).Name
            );
            
            throw;
        }
    }
}
