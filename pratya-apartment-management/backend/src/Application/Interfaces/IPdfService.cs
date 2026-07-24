using Domain.Entities;

namespace Application.Interfaces;

public interface IPdfService
{
    // สร้างไฟล์ PDF รายงานบิล คืนเป็น byte[] เพื่อส่งกลับให้ผู้ใช้ดาวน์โหลด
    byte[] GenerateBillsReport(List<Bill> bills);
}
