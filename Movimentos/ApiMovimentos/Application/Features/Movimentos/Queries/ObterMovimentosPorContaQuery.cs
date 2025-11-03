using Application.DTOs.Responses;
using MediatR;

namespace Application.Features.Movimentos.Queries;

public record ObterMovimentosPorContaQuery(int ContaId) : IRequest<IEnumerable<MovimentoResponse>>;
