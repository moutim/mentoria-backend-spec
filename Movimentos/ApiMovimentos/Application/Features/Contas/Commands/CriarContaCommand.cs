using MediatR;

namespace Application.Features.Contas.Commands;

public record CriarContaCommand : IRequest<int>
{
    public string Numero { get; init; } = string.Empty;
    public decimal SaldoInicial { get; init; }
}
