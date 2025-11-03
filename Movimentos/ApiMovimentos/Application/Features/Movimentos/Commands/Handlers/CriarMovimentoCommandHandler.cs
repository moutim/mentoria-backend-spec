using Domain.Entities;
using Domain.Enums;
using Infrastructure.Repositories;
using MediatR;

namespace Application.Features.Movimentos.Commands.Handlers;

public class CriarMovimentoCommandHandler : IRequestHandler<CriarMovimentoCommand, int>
{
    private readonly IMovimentoRepository _movimentoRepository;
    private readonly IContaRepository _contaRepository;

    public CriarMovimentoCommandHandler(
        IMovimentoRepository movimentoRepository,
        IContaRepository contaRepository)
    {
        _movimentoRepository = movimentoRepository;
        _contaRepository = contaRepository;
    }

    public async Task<int> Handle(CriarMovimentoCommand request, CancellationToken cancellationToken)
    {
        var conta = await _contaRepository.GetByIdAsync(request.ContaId);
        if (conta == null)
            throw new ArgumentException("Conta não encontrada", nameof(request.ContaId));

        if (request.Tipo == TipoMovimento.Debito && conta.Saldo < request.Valor)
            throw new InvalidOperationException("Saldo insuficiente para realizar o débito");

        var movimento = new Movimento
        {
            ContaId = request.ContaId,
            Valor = request.Valor,
            Tipo = request.Tipo,
            DataMovimento = DateTime.UtcNow
        };

        conta.Saldo += request.Tipo == TipoMovimento.Credito ? request.Valor : -request.Valor;
        await _contaRepository.UpdateAsync(conta);

        await _movimentoRepository.CreateAsync(movimento);
        return movimento.Id;
    }
}
