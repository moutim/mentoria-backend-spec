using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ContaRepository : IContaRepository
{
    private readonly AppDbContext _context;

    public ContaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Conta>> GetAllAsync()
    {
        return await _context.Contas.Include(c => c.Movimentos).ToListAsync();
    }

    public async Task<Conta?> GetByIdAsync(int id)
    {
        return await _context.Contas
            .Include(c => c.Movimentos)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Conta> CreateAsync(Conta conta)
    {
        _context.Contas.Add(conta);
        await _context.SaveChangesAsync();
        return conta;
    }

    public async Task UpdateAsync(Conta conta)
    {
        _context.Entry(conta).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var conta = await _context.Contas.FindAsync(id);
        if (conta != null)
        {
            _context.Contas.Remove(conta);
            await _context.SaveChangesAsync();
        }
    }
}
