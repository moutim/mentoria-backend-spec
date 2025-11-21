using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using NotificacoesWorker.Models.Interfaces;

namespace NotificacoesWorker.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IConfiguration _configuration;

    public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task EnviarEmailMovimentacaoAsync(string destinatario, string tipoMovimentacao, 
        decimal valor, string descricao, DateTime dataMovimentacao, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                _configuration["Email:FromName"],
                _configuration["Email:FromEmail"]));
            message.To.Add(new MailboxAddress("Vitor Moutim", "moutimg@hotmail.com"));
            message.Subject = $"Movimentação Bancária - {tipoMovimentacao}";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <h2>Nova Movimentação Bancária</h2>
                        <p><strong>Tipo:</strong> {tipoMovimentacao}</p>
                        <p><strong>Valor:</strong> R$ {valor:N2}</p>
                        <p><strong>Descrição:</strong> {descricao}</p>
                        <p><strong>Data/Hora:</strong> {dataMovimentacao:dd/MM/yyyy HH:mm:ss}</p>
                    </body>
                    </html>"
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(
                _configuration["Email:SmtpHost"], 
                int.Parse(_configuration["Email:SmtpPort"]!), 
                SecureSocketOptions.StartTls, 
                cancellationToken);

            await client.AuthenticateAsync(
                _configuration["Email:Username"], 
                _configuration["Email:Password"], 
                cancellationToken);

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("Email enviado com sucesso para {Destinatario}", destinatario);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar email para {Destinatario}", destinatario);
            throw;
        }
    }
}

