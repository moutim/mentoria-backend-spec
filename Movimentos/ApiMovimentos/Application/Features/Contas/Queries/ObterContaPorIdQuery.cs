using Application.DTOs.Responses;
using MediatR;

namespace Application.Features.Contas.Queries;

public record ObterContaPorIdQuery(int Id) : IRequest<ContaResponse?>;
