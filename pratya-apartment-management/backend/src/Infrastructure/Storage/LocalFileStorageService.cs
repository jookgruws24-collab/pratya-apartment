using Application.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace Infrastructure.Storage;

// เก็บไฟล์ลงโฟลเดอร์ wwwroot/uploads แล้วคืน url แบบ /uploads/ชื่อไฟล์
// ใช้ตอน dev จะได้ไม่ต้องมี Azure account
public class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _env;

    public LocalFileStorageService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> SaveAsync(
        Stream content, string fileName, string contentType)
    {
        // ใช้ ContentRootPath เสมอเพื่อให้ path ตรงกับที่ static files เสิร์ฟไฟล์
        // (WebRootPath อาจเป็น null ถ้าโฟลเดอร์ wwwroot ยังไม่ถูกสร้างตอนเริ่มโปรแกรม)
        var webRoot = Path.Combine(_env.ContentRootPath, "wwwroot");

        var uploadsDir = Path.Combine(webRoot, "uploads");
        Directory.CreateDirectory(uploadsDir);

        // ตั้งชื่อไฟล์ใหม่กันชนกัน: <guid>_<ชื่อเดิม>
        var safeName = $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}";
        var fullPath = Path.Combine(uploadsDir, safeName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await content.CopyToAsync(stream);
        }

        return $"/uploads/{safeName}";
    }
}
