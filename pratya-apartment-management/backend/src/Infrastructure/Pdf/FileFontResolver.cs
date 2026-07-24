using PdfSharp.Fonts;

namespace Infrastructure.Pdf;

// PDFsharp บน Linux ไม่รู้จักฟอนต์ในเครื่องเอง เราจึงต้องบอกที่อยู่ไฟล์ฟอนต์ให้มันเอง
// - Windows (ตอน dev): ใช้ Arial จาก C:\Windows\Fonts
// - Linux (ใน Docker): ใช้ DejaVu Sans (ติดตั้งผ่าน Dockerfile)
public class FileFontResolver : IFontResolver
{
    private readonly byte[] _fontData;

    public FileFontResolver()
    {
        _fontData = File.ReadAllBytes(FindFontFile());
    }

    private static string FindFontFile()
    {
        string[] candidates =
        {
            // Linux (DejaVu มากับ package fonts-dejavu)
            "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf",
            "/usr/share/fonts/dejavu/DejaVuSans.ttf",
            // Windows
            @"C:\Windows\Fonts\arial.ttf",
        };

        foreach (var path in candidates)
        {
            if (File.Exists(path))
            {
                return path;
            }
        }

        throw new FileNotFoundException(
            "ไม่พบไฟล์ฟอนต์สำหรับสร้าง PDF (ลองติดตั้ง fonts-dejavu ใน container)");
    }

    public byte[]? GetFont(string faceName) => _fontData;

    public FontResolverInfo? ResolveTypeface(
        string familyName, bool isBold, bool isItalic)
    {
        // ใช้ฟอนต์เดียวกับทุกกรณี (regular/bold/italic) เพื่อความง่าย
        return new FontResolverInfo("AppFont");
    }
}
