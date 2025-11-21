using System.Text.Json;
using Microsoft.Extensions.Logging;
using NotificacoesWorker.Models.Interfaces;
using NotificacoesWorker.Models.Messages;

namespace NotificacoesWorker.Handlers
{
    public interface IMessageHandler
    {
        Task ProcessarMensagemAsync(string messageBody, CancellationToken cancellationToken = default);
    }

    public class MovimentacaoBancariaHandler : IMessageHandler
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<MovimentacaoBancariaHandler> _logger;

        public MovimentacaoBancariaHandler(IEmailService emailService, ILogger<MovimentacaoBancariaHandler> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        public async Task ProcessarMensagemAsync(string messageBody, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Processando mensagem: {MessageBody}", messageBody);

                var movimentacao = JsonSerializer.Deserialize<MovimentacaoBancariaMessage>(messageBody);

                if (movimentacao == null)
                {
                    _logger.LogWarning("Mensagem deserializada como null");
                    return;
                }

                _logger.LogInformation(
                    "Movimentação ID: {TransacaoId}, Tipo: {Tipo}, Valor: {Valor}", 
                    movimentacao.TransacaoId, 
                    movimentacao.TipoMovimentacao, 
                    movimentacao.Valor);

                await _emailService.EnviarEmailMovimentacaoAsync(
                    movimentacao.EmailDestinatario,
                    movimentacao.TipoMovimentacao,
                    movimentacao.Valor,
                    movimentacao.Descricao,
                    movimentacao.DataMovimentacao,
                    cancellationToken);

                _logger.LogInformation("Mensagem processada com sucesso: {TransacaoId}", movimentacao.TransacaoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem");
                throw;
            }
        }
    }
}


