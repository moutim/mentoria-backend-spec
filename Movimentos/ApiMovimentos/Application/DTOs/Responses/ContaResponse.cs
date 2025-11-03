namespace Application.DTOs.Responses;

public class ContaResponse
{
    public int Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public decimal Saldo { get; set; }
    public ICollection<MovimentoResponse> Movimentos { get; set; } = new List<MovimentoResponse>();
}
