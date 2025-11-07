using Domain.Entities;

namespace Infrastructure.Repositories;

public interface IContaRepository
{
    Task<IEnumerable<Conta>> GetAllAsync();
    Task<Conta?> GetByIdAsync(int id);
    Task<Conta> CreateAsync(Conta conta);
    Task UpdateAsync(Conta conta);
    Task DeleteAsync(int id);
}
