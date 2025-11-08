using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class MovimentoRepository : IMovimentoRepository
{
    private readonly AppDbContext _context;

    public MovimentoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Movimento>> GetAllAsync()
    {
        return await _context.Movimentos.Include(m => m.Categoria).ToListAsync();
    }

    public async Task<IEnumerable<Movimento>> GetByUsuarioIdAsync(string usuarioId)
    {
        return await _context.Movimentos
            .Include(m => m.Categoria)
            .Where(m => m.UsuarioId == usuarioId)
            .ToListAsync();
    }

    public async Task<Movimento?> GetByIdAsync(int id)
    {
        return await _context.Movimentos
            .Include(m => m.Categoria)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Movimento> CreateAsync(Movimento movimento)
    {
        _context.Movimentos.Add(movimento);
        await _context.SaveChangesAsync();
        return movimento;
    }

    public async Task UpdateAsync(Movimento movimento)
    {
        _context.Entry(movimento).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var movimento = await _context.Movimentos.FindAsync(id);
        if (movimento != null)
        {
            _context.Movimentos.Remove(movimento);
            await _context.SaveChangesAsync();
        }
    }
}
