using FinTrack.Shared.DTOs.Report;

namespace FinTrack.Server.Services.Reports;

public interface IReportService
{
    Task<SpendingByCategoryReportDto> GetSpendingByCategoryReportAsync(
        Guid userId,
        DateTime from,
        DateTime to);

    Task<IncomeVsExpenseReportDto> GetIncomeVsExpenseReportAsync(
        Guid userId,
        DateTime from,
        DateTime to,
        string granularity = "monthly");
}
