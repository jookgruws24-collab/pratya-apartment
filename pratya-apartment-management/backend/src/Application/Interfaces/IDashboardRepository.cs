using Application.DTOs;

namespace Application.Interfaces;

public interface IDashboardRepository
{
    Task<DashboardSummaryDto> GetSummaryAsync();
}
