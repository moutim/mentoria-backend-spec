using Application.DTOs.Responses;
using Application.Interfaces;
using Domain.Entities;
using Domain.Events;
using MediatR;

namespace Application.Commands.CriarMovimento;

public class CriarMovimentoCommandHandler : IRequestHandler<CriarMovimentoCommand, MovimentoResponse>
{
    private readonly IMovimentoRepository _movimentoRepository;
    private readonly IEventBus _eventBus;

    public CriarMovimentoCommandHandler(
        IMovimentoRepository movimentoRepository,
        IEventBus eventBus)
    {
        _movimentoRepository = movimentoRepository;
        _eventBus = eventBus;
    }

    public async Task<MovimentoResponse> Handle(CriarMovimentoCommand request, CancellationToken cancellationToken)
    {
        var movimento = new Movimento
        {
            UsuarioId = request.UsuarioId,
            Remetente = request.Remetente,
            Destinatario = request.Destinatario,
            Tipo = request.Tipo,
            CategoriaId = request.CategoriaId,
            Descricao = request.Descricao,
            Valor = request.Valor,
            CriadoEm = DateTime.UtcNow
        };

        var movimentoCriado = await _movimentoRepository.CreateAsync(movimento);
        
        await _eventBus.PublishAsync(new MovimentoCriadoEvent
        {
            MovimentoId = movimentoCriado.Id,
            UsuarioId = movimentoCriado.UsuarioId,
            Remetente = movimentoCriado.Remetente,
            Destinatario = movimentoCriado.Destinatario,
            Tipo = movimentoCriado.Tipo,
            CategoriaId = movimentoCriado.CategoriaId,
            Descricao = movimentoCriado.Descricao ?? "Descrição não fornecida",
            Valor = movimentoCriado.Valor,
            CriadoEm = movimentoCriado.CriadoEm
        });

        return new MovimentoResponse
        {
            Id = movimentoCriado.Id,
            UsuarioId = movimentoCriado.UsuarioId,
            Remetente = movimentoCriado.Remetente,
            Destinatario = movimentoCriado.Destinatario,
            Tipo = movimentoCriado.Tipo,
            CategoriaId = movimentoCriado.CategoriaId,
            CategoriaNome = movimentoCriado.Categoria?.Nome,
            Descricao = movimentoCriado.Descricao,
            Valor = movimentoCriado.Valor,
            CriadoEm = movimentoCriado.CriadoEm
        };
    }
}