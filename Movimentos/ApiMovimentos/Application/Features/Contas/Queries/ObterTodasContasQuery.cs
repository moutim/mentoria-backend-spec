using Application.DTOs.Responses;
using MediatR;

namespace Application.Features.Contas.Queries;

public record ObterTodasContasQuery : IRequest<IEnumerable<ContaResponse>>;
