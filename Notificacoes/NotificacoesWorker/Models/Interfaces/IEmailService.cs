namespace NotificacoesWorker.Models.Interfaces;

public interface IEmailService
{
        Task EnviarEmailMovimentacaoAsync(string destinatario, string tipoMovimentacao, decimal valor, 
            string descricao, DateTime dataMovimentacao, CancellationToken cancellationToken = default);
}