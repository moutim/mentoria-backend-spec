using Application.DTOs.Responses;
using MediatR;

namespace Application.Features.Movimentos.Queries;

public record ObterTodosMovimentosQuery : IRequest<IEnumerable<MovimentoResponse>>;
