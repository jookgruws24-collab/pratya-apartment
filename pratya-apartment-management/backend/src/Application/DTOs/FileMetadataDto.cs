namespace Application.DTOs;

public class FileMetadataDto
{
    public Guid Id { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long SizeBytes { get; set; }

    public string Url { get; set; } = string.Empty;

    public DateTime UploadedAt { get; set; }
}
