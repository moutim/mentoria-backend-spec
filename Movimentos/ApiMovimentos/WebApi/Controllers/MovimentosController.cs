using Application.DTOs.Requests;
using Application.Features.Movimentos.Commands;
using Application.Features.Movimentos.Queries;
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
    public async Task<IActionResult> GetAll()
    {
        var movimentos = await _mediator.Send(new ObterTodosMovimentosQuery());
        return Ok(movimentos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var movimento = await _mediator.Send(new ObterMovimentoPorIdQuery(id));
        if (movimento == null)
            return NotFound();

        return Ok(movimento);
    }

    [HttpGet("conta/{contaId}")]
    public async Task<IActionResult> GetByContaId(int contaId)
    {
        var movimentos = await _mediator.Send(new ObterMovimentosPorContaQuery(contaId));
        return Ok(movimentos);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CriarMovimentoRequest request)
    {
        try
        {
            var command = new CriarMovimentoCommand
            {
                ContaId = request.ContaId,
                Valor = request.Valor,
                Tipo = request.Tipo
            };
            
            var id = await _mediator.Send(command);
            var movimento = await _mediator.Send(new ObterMovimentoPorIdQuery(id));
            return CreatedAtAction(nameof(GetById), new { id }, movimento);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
