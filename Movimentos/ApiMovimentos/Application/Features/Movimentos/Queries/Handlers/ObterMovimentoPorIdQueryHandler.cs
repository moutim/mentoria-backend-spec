using Application.DTOs.Responses;
using Infrastructure.Repositories;
using MediatR;

namespace Application.Features.Movimentos.Queries.Handlers;

public class ObterMovimentoPorIdQueryHandler : IRequestHandler<ObterMovimentoPorIdQuery, MovimentoResponse?>
{
    private readonly IMovimentoRepository _movimentoRepository;

    public ObterMovimentoPorIdQueryHandler(IMovimentoRepository movimentoRepository)
    {
        _movimentoRepository = movimentoRepository;
    }

    public async Task<MovimentoResponse?> Handle(ObterMovimentoPorIdQuery request, CancellationToken cancellationToken)
    {
        var movimento = await _movimentoRepository.GetByIdAsync(request.Id);
        if (movimento == null)
            return null;

        return new MovimentoResponse
        {
            Id = movimento.Id,
            ContaId = movimento.ContaId,
            Valor = movimento.Valor,
            Tipo = movimento.Tipo,
            DataMovimento = movimento.DataMovimento
        };
    }
}
