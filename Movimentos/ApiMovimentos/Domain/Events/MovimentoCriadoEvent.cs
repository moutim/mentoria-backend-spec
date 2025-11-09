namespace Domain.Events;

public class MovimentoCriadoEvent
{
    public int MovimentoId { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Remetente { get; set; } = string.Empty;
    public string Destinatario { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public int? CategoriaId { get; set; }
}
