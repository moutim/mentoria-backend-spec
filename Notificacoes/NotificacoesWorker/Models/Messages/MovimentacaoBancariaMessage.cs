namespace NotificacoesWorker.Models.Messages;

public class MovimentacaoBancariaMessage
{
    public string TransacaoId { get; set; } = string.Empty;
    public string TipoMovimentacao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string EmailDestinatario { get; set; } = string.Empty;
    public DateTime DataMovimentacao { get; set; }
}