using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BillRepository : IBillRepository
{
    private readonly ApplicationDbContext _context;

    public BillRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Bill>> GetAllAsync()
    {
        return await _context.Bills
            .Include(b => b.Room)
            .Include(b => b.Tenant)
            .Include(b => b.BillStatus)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<Bill?> GetByIdAsync(Guid id)
    {
        return await _context.Bills
            .Include(b => b.Room)
            .Include(b => b.Tenant)
            .Include(b => b.BillStatus)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task AddAsync(Bill bill)
    {
        await _context.Bills.AddAsync(bill);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Bill bill)
    {
        _context.Bills.Update(bill);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var bill = await _context.Bills.FindAsync(id);

        if (bill is null)
        {
            return;
        }

        _context.Bills.Remove(bill);

        await _context.SaveChangesAsync();
    }
}
