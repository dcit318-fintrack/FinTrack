using FinTrack.Shared.DTOs.Dashboard;

namespace FinTrack.Server.Services.Dashboard;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetDashboardSummaryAsync(Guid userId, string? month);
}
