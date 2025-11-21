using Amazon.SQS.Model;

namespace NotificacoesWorker.Models.Interfaces;

public interface ISqsService
{
    Task<List<Message>> ReceberMensagensAsync(CancellationToken cancellationToken = default);
    Task DeletarMensagemAsync(string receiptHandle, CancellationToken cancellationToken = default);
}