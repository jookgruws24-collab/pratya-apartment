using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // ต้องล็อกอินก่อนถึงจะอัปโหลด/ดูไฟล์ได้
public class FileController : ControllerBase
{
    private readonly IFileStorageService _fileStorage;
    private readonly IUploadedFileRepository _fileRepository;

    // ชนิดไฟล์ที่อนุญาต และขนาดสูงสุด (5 MB)
    private static readonly string[] AllowedContentTypes =
    {
        "image/jpeg", "image/png", "application/pdf"
    };
    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

    public FileController(
        IFileStorageService fileStorage,
        IUploadedFileRepository fileRepository)
    {
        _fileStorage = fileStorage;
        _fileRepository = fileRepository;
    }

    // อัปโหลดไฟล์ 1 ไฟล์ (form field ชื่อ "file")
    [HttpPost("upload")]
    public async Task<ActionResult<FileMetadataDto>> Upload(IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new { message = "กรุณาเลือกไฟล์" });
        }

        if (file.Length > MaxFileSizeBytes)
        {
            return BadRequest(new { message = "ไฟล์ใหญ่เกิน 5 MB" });
        }

        if (!AllowedContentTypes.Contains(file.ContentType))
        {
            return BadRequest(
                new { message = "รองรับเฉพาะไฟล์ JPG, PNG, PDF" });
        }

        // 1) เก็บตัวไฟล์ลง storage (disk หรือ Azure Blob) แล้วได้ url กลับมา
        await using var stream = file.OpenReadStream();
        var url = await _fileStorage.SaveAsync(
            stream, file.FileName, file.ContentType);

        // 2) เก็บ "ข้อมูลของไฟล์" ลงฐานข้อมูล
        var metadata = new UploadedFile
        {
            Id = Guid.NewGuid(),
            FileName = file.FileName,
            ContentType = file.ContentType,
            SizeBytes = file.Length,
            Url = url,
            UploadedAt = DateTime.UtcNow
        };

        await _fileRepository.AddAsync(metadata);

        return Ok(ToDto(metadata));
    }

    // ดูรายการไฟล์ที่เคยอัปโหลด
    [HttpGet]
    public async Task<ActionResult<List<FileMetadataDto>>> GetAll()
    {
        var files = await _fileRepository.GetAllAsync();

        return Ok(files.Select(ToDto).ToList());
    }

    private static FileMetadataDto ToDto(UploadedFile f) => new()
    {
        Id = f.Id,
        FileName = f.FileName,
        ContentType = f.ContentType,
        SizeBytes = f.SizeBytes,
        Url = f.Url,
        UploadedAt = f.UploadedAt
    };
}
