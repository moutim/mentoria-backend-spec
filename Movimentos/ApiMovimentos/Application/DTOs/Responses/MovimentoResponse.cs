using Domain.Enums;

namespace Application.DTOs.Responses;

public class MovimentoResponse
{
    public int Id { get; set; }
    public int ContaId { get; set; }
    public decimal Valor { get; set; }
    public TipoMovimento Tipo { get; set; }
    public DateTime DataMovimento { get; set; }
}
