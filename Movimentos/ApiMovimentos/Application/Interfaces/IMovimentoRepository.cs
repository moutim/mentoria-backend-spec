using Domain.Entities;

namespace Application.Interfaces;

public interface IMovimentoRepository
{
    Task<IEnumerable<Movimento>> GetAllAsync();
    Task<IEnumerable<Movimento>> GetByUsuarioIdAsync(string usuarioId);
    Task<Movimento?> GetByIdAsync(int id);
    Task<Movimento> CreateAsync(Movimento movimento);
    Task UpdateAsync(Movimento movimento);
    Task DeleteAsync(int id);
}

