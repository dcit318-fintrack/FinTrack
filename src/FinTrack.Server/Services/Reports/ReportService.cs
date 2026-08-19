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
        // Database-side GroupBy and Sum aggregation
        var categoryGroups = await _dbContext.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId &&
                        t.Type == "Expense" &&
                        t.Date >= from &&
                        t.Date <= to)
            .GroupBy(t => new { t.CategoryId, t.Category.Name })
            .Select(g => new
            {
                CategoryId = g.Key.CategoryId,
                CategoryName = g.Key.Name,
                Amount = g.Sum(t => t.Amount)
            })
            .OrderByDescending(c => c.Amount)
            .ToListAsync();

        var totalSpent = categoryGroups.Sum(c => c.Amount);

        var categories = categoryGroups.Select(c => new CategorySpendingItemDto
        {
            CategoryId = c.CategoryId,
            CategoryName = c.CategoryName,
            Amount = c.Amount,
            PercentOfTotal = totalSpent > 0
                ? Math.Round((double)(c.Amount / totalSpent) * 100, 1)
                : 0.0
        }).ToList();

        return new SpendingByCategoryReportDto
        {
            From = from,
            To = to,
            TotalSpent = totalSpent,
            Categories = categories
        };
    }

    public async Task<IncomeVsExpenseReportDto> GetIncomeVsExpenseReportAsync(
        Guid userId,
        DateTime from,
        DateTime to,
        string granularity = "monthly")
    {
        granularity = string.IsNullOrWhiteSpace(granularity) ? "monthly" : granularity.ToLower();

        // Database-side grouping by Date and Type
        var rawTransactions = await _dbContext.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId && t.Date >= from && t.Date <= to)
            .Select(t => new { t.Date, t.Type, t.Amount })
            .ToListAsync();

        var points = new List<IncomeVsExpensePointDto>();

        if (granularity == "daily")
        {
            for (var day = from.Date; day <= to.Date; day = day.AddDays(1))
            {
                var periodStr = day.ToString("yyyy-MM-dd");
                var dayTx = rawTransactions.Where(t => t.Date.Date == day).ToList();

                var income = dayTx.Where(t => t.Type == "Income").Sum(t => t.Amount);
                var expense = dayTx.Where(t => t.Type == "Expense").Sum(t => t.Amount);

                points.Add(new IncomeVsExpensePointDto
                {
                    Period = periodStr,
                    Income = income,
                    Expense = expense,
                    Net = income - expense
                });
            }
        }
        else if (granularity == "weekly")
        {
            var current = from.Date;
            while (current <= to.Date)
            {
                var weekEnd = current.AddDays(6) > to.Date ? to.Date : current.AddDays(6);
                var periodStr = $"{current:yyyy-MM-dd}..{weekEnd:yyyy-MM-dd}";
                var weekTx = rawTransactions.Where(t => t.Date.Date >= current && t.Date.Date <= weekEnd).ToList();

                var income = weekTx.Where(t => t.Type == "Income").Sum(t => t.Amount);
                var expense = weekTx.Where(t => t.Type == "Expense").Sum(t => t.Amount);

                points.Add(new IncomeVsExpensePointDto
                {
                    Period = periodStr,
                    Income = income,
                    Expense = expense,
                    Net = income - expense
                });

                current = current.AddDays(7);
            }
        }
        else
        {
            var current = new DateTime(from.Year, from.Month, 1);
            var end = new DateTime(to.Year, to.Month, 1);

            while (current <= end)
            {
                var periodStr = current.ToString("yyyy-MM");
                var monthStart = current;
                var monthEnd = current.AddMonths(1).AddTicks(-1);

                var monthTx = rawTransactions.Where(t => t.Date >= monthStart && t.Date <= monthEnd).ToList();

                var income = monthTx.Where(t => t.Type == "Income").Sum(t => t.Amount);
                var expense = monthTx.Where(t => t.Type == "Expense").Sum(t => t.Amount);

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
