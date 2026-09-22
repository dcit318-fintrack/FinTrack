using FinTrack.Server.Data;
using FinTrack.Server.Services.Budgets;
using FinTrack.Shared.DTOs.Dashboard;
using FinTrack.Shared.DTOs.Transaction;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Server.Services.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly FinTrackDbContext _dbContext;
    private readonly IBudgetService _budgetService;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(FinTrackDbContext dbContext, IBudgetService budgetService, ILogger<DashboardService> logger)
    {
        _dbContext = dbContext;
        _budgetService = budgetService;
        _logger = logger;
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(Guid userId, string? month)
    {
        if (string.IsNullOrWhiteSpace(month))
        {
            month = DateTime.UtcNow.ToString("yyyy-MM");
        }

        if (!DateTime.TryParseExact(month + "-01", "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var startDate))
        {
            startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        }
        var endDate = startDate.AddMonths(1).AddTicks(-1);

        var totalIncome = await _dbContext.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId && t.Type == "Income" && t.Date >= startDate && t.Date <= endDate)
            .SumAsync(t => (decimal?)t.Amount) ?? 0m;

        var totalExpenses = await _dbContext.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId && t.Type == "Expense" && t.Date >= startDate && t.Date <= endDate)
            .SumAsync(t => (decimal?)t.Amount) ?? 0m;

        var balance = totalIncome - totalExpenses;

        var recentTransactions = await _dbContext.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.CreatedAt)
            .Take(5)
            .Select(t => new TransactionDto
            {
                Id = t.Id,
                Amount = t.Amount,
                Type = t.Type,
                CategoryId = t.CategoryId,
                CategoryName = t.Category.Name,
                Description = t.Description,
                Date = t.Date
            })
            .ToListAsync();

        var budgets = await _budgetService.GetBudgetsAsync(userId, month);
        var budgetsAtRisk = budgets
            .Where(b => b.Limit > 0 && (b.Spent / b.Limit) >= 0.8m)
            .Select(b => new BudgetRiskDto
            {
                CategoryName = b.CategoryName,
                Limit = b.Limit,
                Spent = b.Spent,
                PercentUsed = Math.Round((double)(b.Spent / b.Limit) * 100, 1)
            })
            .ToList();

        _logger.LogInformation("Dashboard summary loaded for user {UserId}, month {Month}: income={Income}, expenses={Expenses}", userId, month, totalIncome, totalExpenses);

        return new DashboardSummaryDto
        {
            TotalIncome = totalIncome,
            TotalExpenses = totalExpenses,
            Balance = balance,
            Month = month,
            RecentTransactions = recentTransactions,
            BudgetsAtRisk = budgetsAtRisk
        };
    }
}
