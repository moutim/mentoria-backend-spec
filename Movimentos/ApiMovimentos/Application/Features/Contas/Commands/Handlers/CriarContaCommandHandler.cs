using Domain.Entities;
using Infrastructure.Repositories;
using MediatR;

namespace Application.Features.Contas.Commands.Handlers;

public class CriarContaCommandHandler : IRequestHandler<CriarContaCommand, int>
{
    private readonly IContaRepository _contaRepository;

    public CriarContaCommandHandler(IContaRepository contaRepository)
    {
        _contaRepository = contaRepository;
    }

    public async Task<int> Handle(CriarContaCommand request, CancellationToken cancellationToken)
    {
        var conta = new Conta
        {
            Numero = request.Numero,
            Saldo = request.SaldoInicial
        };

        await _contaRepository.CreateAsync(conta);
        return conta.Id;
    }
}
