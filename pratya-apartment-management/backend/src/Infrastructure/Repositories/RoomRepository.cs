using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly ApplicationDbContext _context;

    public RoomRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Room>> GetAllAsync()
    {
        return await _context.Rooms
            .Include(r => r.RoomStatus)
            .ToListAsync();
    }

    public async Task AddAsync(Room room)
    {
        await _context.Rooms.AddAsync(room);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var room = await _context.Rooms.FindAsync(id);

        if (room is null)
        {
            return;
        }

        _context.Rooms.Remove(room);

        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(Room room)
    {
        _context.Rooms.Update(room);

        await _context.SaveChangesAsync();
    }
}