using Application.DTOs.Responses;
using MediatR;

namespace Application.Queries.MovimentosByConta;

public class MovimentosByContaQuery : IRequest<MovimentoResponse[]>
{
    public string UsuarioId { get; set; } = string.Empty;
}