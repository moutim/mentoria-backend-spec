using Application.DTOs.Responses;
using MediatR;

namespace Application.Commands.CriarMovimento;

public class CriarMovimentoCommand : IRequest<MovimentoResponse>
{
    public string UsuarioId { get; set; } = string.Empty;
    public string Remetente { get; set; } = string.Empty;
    public string Destinatario { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public int? CategoriaId { get; set; }
    public string? Descricao { get; set; }
    public decimal Valor { get; set; }
}