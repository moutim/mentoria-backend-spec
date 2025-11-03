using Application.DTOs.Responses;
using Infrastructure.Repositories;
using MediatR;

namespace Application.Features.Contas.Queries.Handlers;

public class ObterContaPorIdQueryHandler : IRequestHandler<ObterContaPorIdQuery, ContaResponse?>
{
    private readonly IContaRepository _contaRepository;

    public ObterContaPorIdQueryHandler(IContaRepository contaRepository)
    {
        _contaRepository = contaRepository;
    }

    public async Task<ContaResponse?> Handle(ObterContaPorIdQuery request, CancellationToken cancellationToken)
    {
        var conta = await _contaRepository.GetByIdAsync(request.Id);
        if (conta == null)
            return null;

        return new ContaResponse
        {
            Id = conta.Id,
            Numero = conta.Numero,
            Saldo = conta.Saldo,
            Movimentos = conta.Movimentos.Select(m => new MovimentoResponse
            {
                Id = m.Id,
                ContaId = m.ContaId,
                Valor = m.Valor,
                Tipo = m.Tipo,
                DataMovimento = m.DataMovimento
            }).ToList()
        };
    }
}
