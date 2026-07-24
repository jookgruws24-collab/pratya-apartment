using Application.DTOs;
using Application.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly ApplicationDbContext _context;

    public DashboardRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync()
    {
        var bills = await _context.Bills.ToListAsync();

        // BillStatusId = 2 คือ "Paid" (จ่ายแล้ว) ตาม seed data
        const int paidStatusId = 2;

        var summary = new DashboardSummaryDto
        {
            TotalRooms = await _context.Rooms.CountAsync(),
            TotalTenants = await _context.Tenants.CountAsync(),
            TotalBills = bills.Count,
            TotalRevenue = bills
                .Where(b => b.BillStatusId == paidStatusId)
                .Sum(b => b.TotalAmount),
            UnpaidAmount = bills
                .Where(b => b.BillStatusId != paidStatusId)
                .Sum(b => b.TotalAmount)
        };

        // จัดกลุ่มรายได้ (บิลที่จ่ายแล้ว) ตามเดือน เพื่อเอาไปวาดกราฟ
        summary.MonthlyRevenue = bills
            .Where(b => b.BillStatusId == paidStatusId)
            .GroupBy(b => new { b.BillingMonth.Year, b.BillingMonth.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new MonthlyRevenueDto
            {
                Month = $"{g.Key.Year:D4}-{g.Key.Month:D2}",
                Amount = g.Sum(b => b.TotalAmount)
            })
            .ToList();

        return summary;
    }
}
