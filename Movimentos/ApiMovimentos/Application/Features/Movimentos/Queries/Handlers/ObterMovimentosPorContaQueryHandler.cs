using Application.DTOs.Responses;
using Infrastructure.Repositories;
using MediatR;

namespace Application.Features.Movimentos.Queries.Handlers;

public class ObterMovimentosPorContaQueryHandler : IRequestHandler<ObterMovimentosPorContaQuery, IEnumerable<MovimentoResponse>>
{
    private readonly IMovimentoRepository _movimentoRepository;

    public ObterMovimentosPorContaQueryHandler(IMovimentoRepository movimentoRepository)
    {
        _movimentoRepository = movimentoRepository;
    }

    public async Task<IEnumerable<MovimentoResponse>> Handle(ObterMovimentosPorContaQuery request, CancellationToken cancellationToken)
    {
        var movimentos = await _movimentoRepository.GetByContaIdAsync(request.ContaId);
        return movimentos.Select(m => new MovimentoResponse
        {
            Id = m.Id,
            ContaId = m.ContaId,
            Valor = m.Valor,
            Tipo = m.Tipo,
            DataMovimento = m.DataMovimento
        });
    }
}
