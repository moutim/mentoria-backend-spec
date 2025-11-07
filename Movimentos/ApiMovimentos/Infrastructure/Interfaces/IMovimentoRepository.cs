using Domain.Entities;

namespace Infrastructure.Repositories;

public interface IMovimentoRepository
{
    Task<IEnumerable<Movimento>> GetAllAsync();
    Task<IEnumerable<Movimento>> GetByContaIdAsync(int contaId);
    Task<Movimento?> GetByIdAsync(int id);
    Task<Movimento> CreateAsync(Movimento movimento);
    Task UpdateAsync(Movimento movimento);
    Task DeleteAsync(int id);
}
