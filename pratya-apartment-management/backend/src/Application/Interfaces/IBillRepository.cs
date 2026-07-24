using Domain.Entities;

namespace Application.Interfaces;

public interface IBillRepository
{
    Task<List<Bill>> GetAllAsync();

    Task<Bill?> GetByIdAsync(Guid id);

    Task AddAsync(Bill bill);

    Task UpdateAsync(Bill bill);

    Task DeleteAsync(Guid id);
}
