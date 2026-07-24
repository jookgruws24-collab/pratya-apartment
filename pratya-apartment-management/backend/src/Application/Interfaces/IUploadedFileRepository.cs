using Domain.Entities;

namespace Application.Interfaces;

public interface IUploadedFileRepository
{
    Task<List<UploadedFile>> GetAllAsync();

    Task AddAsync(UploadedFile file);
}
