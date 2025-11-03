using Application.DTOs.Requests;
using Application.Features.Contas.Commands;
using Application.Features.Contas.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContasController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retorna todas as contas cadastradas
    /// </summary>
    /// <returns>Lista de contas com seus respectivos movimentos</returns>
    /// <response code="200">Retorna a lista de contas</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var contas = await _mediator.Send(new ObterTodasContasQuery());
        return Ok(contas);
    }

    /// <summary>
    /// Obtém uma conta pelo seu ID
    /// </summary>
    /// <param name="id">ID da conta</param>
    /// <returns>Dados da conta solicitada</returns>
    /// <response code="200">Retorna a conta solicitada</response>
    /// <response code="404">Conta não encontrada</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var conta = await _mediator.Send(new ObterContaPorIdQuery(id));
        if (conta == null)
            return NotFound();

        return Ok(conta);
    }

    /// <summary>
    /// Cria uma nova conta
    /// </summary>
    /// <param name="request">Dados da conta a ser criada</param>
    /// <returns>Conta recém-criada</returns>
    /// <response code="201">Conta criada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CriarContaRequest request)
    {
        var command = new CriarContaCommand
        {
            Numero = request.Numero,
            SaldoInicial = request.SaldoInicial
        };
        
        var id = await _mediator.Send(command);
        var conta = await _mediator.Send(new ObterContaPorIdQuery(id));
        return CreatedAtAction(nameof(GetById), new { id }, conta);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeletarContaCommand(id));
        return NoContent();
    }
}
