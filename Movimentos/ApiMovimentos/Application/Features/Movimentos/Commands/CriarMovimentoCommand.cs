using Domain.Enums;
using MediatR;

namespace Application.Features.Movimentos.Commands;

public record CriarMovimentoCommand : IRequest<int>
{
    public int ContaId { get; init; }
    public decimal Valor { get; init; }
    public TipoMovimento Tipo { get; init; }
}
