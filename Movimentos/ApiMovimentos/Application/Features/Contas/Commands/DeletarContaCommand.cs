using MediatR;

namespace Application.Features.Contas.Commands;

public record DeletarContaCommand(int Id) : IRequest;
