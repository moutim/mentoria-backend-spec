using Application.DTOs.Responses;
using MediatR;

namespace Application.Features.Movimentos.Queries;

public record ObterMovimentoPorIdQuery(int Id) : IRequest<MovimentoResponse?>;
