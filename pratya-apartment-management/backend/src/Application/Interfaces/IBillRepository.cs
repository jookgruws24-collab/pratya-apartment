using Domain.Entities;

namespace Application.Interfaces;

public interface IBillRepository
{
    Task<List<Bill>> GetAllAsync();

    Task AddAsync(Bill bill);
}