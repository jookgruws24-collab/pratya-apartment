using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UploadedFileRepository : IUploadedFileRepository
{
    private readonly ApplicationDbContext _context;

    public UploadedFileRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<UploadedFile>> GetAllAsync()
    {
        return await _context.UploadedFiles
            .OrderByDescending(f => f.UploadedAt)
            .ToListAsync();
    }

    public async Task AddAsync(UploadedFile file)
    {
        await _context.UploadedFiles.AddAsync(file);

        await _context.SaveChangesAsync();
    }
}
