namespace Domain.Entities;

// เก็บเฉพาะ "ข้อมูลของไฟล์" (metadata) ลงฐานข้อมูล
// ตัวไฟล์จริงเก็บไว้ที่ disk หรือ Azure Blob (ไม่เก็บไฟล์ลง database)
public class UploadedFile
{
    public Guid Id { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long SizeBytes { get; set; }

    // path หรือ url ที่ใช้เปิดไฟล์
    public string Url { get; set; } = string.Empty;

    public DateTime UploadedAt { get; set; }
}
