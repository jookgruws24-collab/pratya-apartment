using Application.Interfaces;
using Domain.Entities;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;

namespace Infrastructure.Pdf;

public class PdfService : IPdfService
{
    static PdfService()
    {
        // ตั้งค่า font resolver ครั้งเดียวตอนโปรแกรมเริ่ม (จำเป็นสำหรับ Linux)
        if (GlobalFontSettings.FontResolver is null)
        {
            GlobalFontSettings.FontResolver = new FileFontResolver();
        }
    }

    public byte[] GenerateBillsReport(List<Bill> bills)
    {
        var document = new PdfDocument();
        var page = document.AddPage();
        var gfx = XGraphics.FromPdfPage(page);

        var titleFont = new XFont("AppFont", 18, XFontStyleEx.Bold);
        var headerFont = new XFont("AppFont", 10, XFontStyleEx.Bold);
        var textFont = new XFont("AppFont", 10, XFontStyleEx.Regular);

        double margin = 40;
        double y = margin;

        gfx.DrawString("Bills Report", titleFont, XBrushes.Black,
            new XRect(0, y, page.Width.Point, 30), XStringFormats.TopCenter);
        y += 40;

        // หัวตาราง
        gfx.DrawString("Room", headerFont, XBrushes.Black, margin, y);
        gfx.DrawString("Tenant", headerFont, XBrushes.Black, margin + 90, y);
        gfx.DrawString("Month", headerFont, XBrushes.Black, margin + 240, y);
        gfx.DrawString("Status", headerFont, XBrushes.Black, margin + 330, y);
        gfx.DrawString("Total", headerFont, XBrushes.Black, margin + 430, y);
        y += 8;
        gfx.DrawLine(XPens.Black, margin, y, page.Width.Point - margin, y);
        y += 14;

        decimal grandTotal = 0;

        foreach (var bill in bills)
        {
            // ขึ้นหน้าใหม่ถ้าเนื้อหาล้นหน้า
            if (y > page.Height.Point - margin)
            {
                page = document.AddPage();
                gfx = XGraphics.FromPdfPage(page);
                y = margin;
            }

            var room = bill.Room?.RoomNumber ?? "-";
            var tenant = bill.Tenant is null
                ? "-"
                : $"{bill.Tenant.FirstName} {bill.Tenant.LastName}";
            var month = bill.BillingMonth.ToString("yyyy-MM");
            var status = bill.BillStatus?.Name ?? "-";

            gfx.DrawString(room, textFont, XBrushes.Black, margin, y);
            gfx.DrawString(tenant, textFont, XBrushes.Black, margin + 90, y);
            gfx.DrawString(month, textFont, XBrushes.Black, margin + 240, y);
            gfx.DrawString(status, textFont, XBrushes.Black, margin + 330, y);
            gfx.DrawString(bill.TotalAmount.ToString("N2"), textFont,
                XBrushes.Black, margin + 430, y);

            grandTotal += bill.TotalAmount;
            y += 18;
        }

        y += 6;
        gfx.DrawLine(XPens.Black, margin, y, page.Width.Point - margin, y);
        y += 16;
        gfx.DrawString($"Grand Total: {grandTotal:N2}", headerFont,
            XBrushes.Black, margin + 330, y);

        using var stream = new MemoryStream();
        document.Save(stream);
        return stream.ToArray();
    }
}
