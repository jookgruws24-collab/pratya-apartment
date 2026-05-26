using Domain.Entities;

namespace Application.Interfaces;

public interface ITenantRepository
{
    Task<List<Tenant>> GetAllAsync();

    Task AddAsync(Tenant tenant);

    Task UpdateAsync(Tenant tenant);
}