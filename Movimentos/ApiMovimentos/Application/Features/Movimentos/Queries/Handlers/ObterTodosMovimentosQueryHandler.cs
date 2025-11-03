using Application.DTOs.Responses;
using Infrastructure.Repositories;
using MediatR;

namespace Application.Features.Movimentos.Queries.Handlers;

public class ObterTodosMovimentosQueryHandler : IRequestHandler<ObterTodosMovimentosQuery, IEnumerable<MovimentoResponse>>
{
    private readonly IMovimentoRepository _movimentoRepository;

    public ObterTodosMovimentosQueryHandler(IMovimentoRepository movimentoRepository)
    {
        _movimentoRepository = movimentoRepository;
    }

    public async Task<IEnumerable<MovimentoResponse>> Handle(ObterTodosMovimentosQuery request, CancellationToken cancellationToken)
    {
        var movimentos = await _movimentoRepository.GetAllAsync();
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
