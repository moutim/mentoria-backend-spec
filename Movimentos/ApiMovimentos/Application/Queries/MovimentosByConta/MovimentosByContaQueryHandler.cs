using Application.DTOs.Responses;
using Application.Interfaces;
using MediatR;

namespace Application.Queries.MovimentosByConta;

public class MovimentosByContaQueryHandler : IRequestHandler<MovimentosByContaQuery, MovimentoResponse[]>
{
    private readonly IMovimentoRepository _movimentoRepository;

    public MovimentosByContaQueryHandler(IMovimentoRepository movimentoRepository)
    {
        _movimentoRepository = movimentoRepository;
    }

    public async Task<MovimentoResponse[]> Handle(MovimentosByContaQuery request, CancellationToken cancellationToken)
    {
        var movimentos = string.IsNullOrWhiteSpace(request.UsuarioId)
            ? await _movimentoRepository.GetAllAsync()
            : await _movimentoRepository.GetByUsuarioIdAsync(request.UsuarioId);

        return movimentos.Select(m => new MovimentoResponse
        {
            Id = m.Id,
            UsuarioId = m.UsuarioId,
            Remetente = m.Remetente,
            Destinatario = m.Destinatario,
            Tipo = m.Tipo,
            CategoriaId = m.CategoriaId,
            CategoriaNome = m.Categoria?.Nome,
            Descricao = m.Descricao,
            Valor = m.Valor,
            CriadoEm = m.CriadoEm
        }).ToArray();
    }
}