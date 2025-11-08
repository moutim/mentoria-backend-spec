using Application.Commands.CriarMovimento;
using Application.DTOs.Requests;
using Application.Queries.MovimentosByConta;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MovimentosController : ControllerBase
{
    private readonly IMediator _mediator;

    public MovimentosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllByConta(
        [FromQuery(Name = "usuarioId")] string? usuarioId)
    {
        var query = new MovimentosByContaQuery
        {
            UsuarioId = usuarioId ?? string.Empty
        };
        var movimentos = await _mediator.Send(query);
        return Ok(movimentos);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CriarMovimentoRequest request)
    {
        var command = new CriarMovimentoCommand
        {
            UsuarioId = request.UsuarioId,
            Remetente = request.Remetente,
            Destinatario = request.Destinatario,
            Tipo = request.Tipo,
            CategoriaId = request.CategoriaId,
            Descricao = request.Descricao,
            Valor = request.Valor
        };

        var movimento = await _mediator.Send(command);
        return CreatedAtAction(nameof(Create), new { id = movimento.Id }, movimento);
    }
}
