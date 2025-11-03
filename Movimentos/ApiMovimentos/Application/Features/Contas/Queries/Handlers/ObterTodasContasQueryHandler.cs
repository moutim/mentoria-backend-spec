using Application.DTOs.Responses;
using Infrastructure.Repositories;
using MediatR;

namespace Application.Features.Contas.Queries.Handlers;

public class ObterTodasContasQueryHandler : IRequestHandler<ObterTodasContasQuery, IEnumerable<ContaResponse>>
{
    private readonly IContaRepository _contaRepository;

    public ObterTodasContasQueryHandler(IContaRepository contaRepository)
    {
        _contaRepository = contaRepository;
    }

    public async Task<IEnumerable<ContaResponse>> Handle(ObterTodasContasQuery request, CancellationToken cancellationToken)
    {
        var contas = await _contaRepository.GetAllAsync();
        return contas.Select(c => new ContaResponse
        {
            Id = c.Id,
            Numero = c.Numero,
            Saldo = c.Saldo,
            Movimentos = c.Movimentos.Select(m => new MovimentoResponse
            {
                Id = m.Id,
                ContaId = m.ContaId,
                Valor = m.Valor,
                Tipo = m.Tipo,
                DataMovimento = m.DataMovimento
            }).ToList()
        });
    }
}
