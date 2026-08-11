using FinTrack.Server.Data;
using FinTrack.Shared.DTOs.Report;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Server.Services.Reports;

public class ReportService : IReportService
{
    private readonly FinTrackDbContext _dbContext;

    public ReportService(FinTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SpendingByCategoryReportDto> GetSpendingByCategoryReportAsync(
        Guid userId,
        DateTime from,
        DateTime to)
    {
        var expenseTransactions = await _dbContext.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId &&
                        t.Type.ToLower() == "expense" &&
                        t.Date >= from &&
                        t.Date <= to)
            .Include(t => t.Category)
            .ToListAsync();

        var totalSpent = expenseTransactions.Sum(t => t.Amount);

        var categoryGroup = expenseTransactions
            .GroupBy(t => new { t.CategoryId, t.Category.Name })
            .Select(g =>
            {
                var amount = g.Sum(t => t.Amount);
                var percent = totalSpent > 0
                    ? Math.Round((double)(amount / totalSpent) * 100, 1)
                    : 0.0;

                return new CategorySpendingItemDto
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.Name,
                    Amount = amount,
                    PercentOfTotal = percent
                };
            })
            .OrderByDescending(c => c.Amount)
            .ToList();

        return new SpendingByCategoryReportDto
        {
            From = from,
            To = to,
            TotalSpent = totalSpent,
            Categories = categoryGroup
        };
    }

    public async Task<IncomeVsExpenseReportDto> GetIncomeVsExpenseReportAsync(
        Guid userId,
        DateTime from,
        DateTime to,
        string granularity = "monthly")
    {
        granularity = string.IsNullOrWhiteSpace(granularity) ? "monthly" : granularity.ToLower();

        var transactions = await _dbContext.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId && t.Date >= from && t.Date <= to)
            .ToListAsync();

        var points = new List<IncomeVsExpensePointDto>();

        if (granularity == "daily")
        {
            for (var day = from.Date; day <= to.Date; day = day.AddDays(1))
            {
                var periodStr = day.ToString("yyyy-MM-dd");
                var dayTx = transactions.Where(t => t.Date.Date == day).ToList();

                var income = dayTx.Where(t => t.Type.Equals("Income", StringComparison.OrdinalIgnoreCase)).Sum(t => t.Amount);
                var expense = dayTx.Where(t => t.Type.Equals("Expense", StringComparison.OrdinalIgnoreCase)).Sum(t => t.Amount);

                points.Add(new IncomeVsExpensePointDto
                {
                    Period = periodStr,
                    Income = income,
                    Expense = expense,
                    Net = income - expense
                });
            }
        }
        else
        {
            // Monthly aggregation (default)
            var current = new DateTime(from.Year, from.Month, 1);
            var end = new DateTime(to.Year, to.Month, 1);

            while (current <= end)
            {
                var periodStr = current.ToString("yyyy-MM");
                var monthStart = current;
                var monthEnd = current.AddMonths(1).AddTicks(-1);

                var monthTx = transactions.Where(t => t.Date >= monthStart && t.Date <= monthEnd).ToList();

                var income = monthTx.Where(t => t.Type.Equals("Income", StringComparison.OrdinalIgnoreCase)).Sum(t => t.Amount);
                var expense = monthTx.Where(t => t.Type.Equals("Expense", StringComparison.OrdinalIgnoreCase)).Sum(t => t.Amount);

                points.Add(new IncomeVsExpensePointDto
                {
                    Period = periodStr,
                    Income = income,
                    Expense = expense,
                    Net = income - expense
                });

                current = current.AddMonths(1);
            }
        }

        return new IncomeVsExpenseReportDto
        {
            Granularity = granularity,
            Points = points
        };
    }
}
