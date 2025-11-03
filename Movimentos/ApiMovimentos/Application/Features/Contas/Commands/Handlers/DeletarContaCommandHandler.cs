using Infrastructure.Repositories;
using MediatR;

namespace Application.Features.Contas.Commands.Handlers;

public class DeletarContaCommandHandler : IRequestHandler<DeletarContaCommand>
{
    private readonly IContaRepository _contaRepository;

    public DeletarContaCommandHandler(IContaRepository contaRepository)
    {
        _contaRepository = contaRepository;
    }

    public async Task Handle(DeletarContaCommand request, CancellationToken cancellationToken)
    {
        await _contaRepository.DeleteAsync(request.Id);
    }
}
