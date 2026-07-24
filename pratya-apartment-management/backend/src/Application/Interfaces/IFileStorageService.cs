namespace Application.Interfaces;

// สลับที่เก็บไฟล์ได้ระหว่าง disk (ตอน dev) กับ Azure Blob (ตอน deploy)
// โดยที่ Controller เรียกใช้เหมือนเดิม
public interface IFileStorageService
{
    // บันทึกไฟล์ แล้วคืน url/path สำหรับเปิดไฟล์
    Task<string> SaveAsync(Stream content, string fileName, string contentType);
}
