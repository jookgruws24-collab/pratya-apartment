using Domain.Entities;

namespace Application.Interfaces;

public interface IRoomRepository
{
    Task<List<Room>> GetAllAsync();

    Task AddAsync(Room room);
    
    Task DeleteAsync(Guid id);
    
    Task UpdateAsync(Room room);
}