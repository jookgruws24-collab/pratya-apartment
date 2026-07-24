namespace Application.DTOs;

// ข้อมูลสรุปสำหรับหน้า Dashboard (การ์ดสรุป + กราฟ)
public class DashboardSummaryDto
{
    public int TotalRooms { get; set; }

    public int TotalTenants { get; set; }

    public int TotalBills { get; set; }

    public decimal TotalRevenue { get; set; }

    public decimal UnpaidAmount { get; set; }

    // รายได้แยกตามเดือน (ใช้วาดกราฟ)
    public List<MonthlyRevenueDto> MonthlyRevenue { get; set; } = new();
}

public class MonthlyRevenueDto
{
    // รูปแบบ "2026-07"
    public string Month { get; set; } = string.Empty;

    public decimal Amount { get; set; }
}
