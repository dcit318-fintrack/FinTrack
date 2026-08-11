using FinTrack.Server.Data;
using FinTrack.Server.Models;
using FinTrack.Shared.DTOs.Budget;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Server.Services.Budgets;

public class BudgetService : IBudgetService
{
    private readonly FinTrackDbContext _dbContext;

    public BudgetService(FinTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<BudgetDto>> GetBudgetsAsync(Guid userId, string month)
    {
        if (string.IsNullOrWhiteSpace(month))
        {
            month = DateTime.UtcNow.ToString("yyyy-MM");
        }

        var budgets = await _dbContext.Budgets
            .AsNoTracking()
            .Where(b => b.UserId == userId && b.Month == month)
            .Include(b => b.Category)
            .ToListAsync();

        if (!budgets.Any())
        {
            return new List<BudgetDto>();
        }

        // Calculate start and end dates for the month
        if (!DateTime.TryParseExact(month + "-01", "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var startDate))
        {
            startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        }
        var endDate = startDate.AddMonths(1).AddTicks(-1);

        // Fetch all expense transactions for the user within that month
        var expenseSums = await _dbContext.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId &&
                        t.Type.ToLower() == "expense" &&
                        t.Date >= startDate &&
                        t.Date <= endDate)
            .GroupBy(t => t.CategoryId)
            .Select(g => new
            {
                CategoryId = g.Key,
                TotalSpent = g.Sum(t => t.Amount)
            })
            .ToDictionaryAsync(x => x.CategoryId, x => x.TotalSpent);

        return budgets.Select(b =>
        {
            var spent = expenseSums.TryGetValue(b.CategoryId, out var total) ? total : 0m;
            return new BudgetDto
            {
                Id = b.Id,
                CategoryId = b.CategoryId,
                CategoryName = b.Category.Name,
                Limit = b.Limit,
                Spent = spent,
                Remaining = b.Limit - spent,
                Month = b.Month
            };
        }).ToList();
    }

    public async Task<(bool success, BudgetDto? dto, string? errorMessage, Dictionary<string, string[]>? errors, bool isConflict)> CreateAsync(
        Guid userId,
        CreateBudgetRequest request)
    {
        var category = await _dbContext.Categories.FindAsync(request.CategoryId);
        if (category == null)
        {
            var errors = new Dictionary<string, string[]>
            {
                { "categoryId", new[] { "Category does not exist." } }
            };
            return (false, null, "Validation failed.", errors, false);
        }

        var existingBudget = await _dbContext.Budgets
            .AnyAsync(b => b.UserId == userId && b.CategoryId == request.CategoryId && b.Month == request.Month);

        if (existingBudget)
        {
            return (false, null, "A budget for this category and month already exists.", null, true);
        }

        var budget = new Budget
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CategoryId = request.CategoryId,
            Limit = Math.Round(request.Limit, 2),
            Month = request.Month,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Budgets.Add(budget);
        await _dbContext.SaveChangesAsync();

        var dto = new BudgetDto
        {
            Id = budget.Id,
            CategoryId = budget.CategoryId,
            CategoryName = category.Name,
            Limit = budget.Limit,
            Spent = 0m,
            Remaining = budget.Limit,
            Month = budget.Month
        };

        return (true, dto, null, null, false);
    }

    public async Task<(bool success, BudgetDto? dto, string? errorMessage)> UpdateAsync(
        Guid userId,
        Guid id,
        UpdateBudgetRequest request)
    {
        var budget = await _dbContext.Budgets
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.UserId == userId && b.Id == id);

        if (budget == null)
        {
            return (false, null, "Budget not found.");
        }

        budget.Limit = Math.Round(request.Limit, 2);
        await _dbContext.SaveChangesAsync();

        var dto = new BudgetDto
        {
            Id = budget.Id,
            CategoryId = budget.CategoryId,
            CategoryName = budget.Category.Name,
            Limit = budget.Limit,
            Spent = 0m,
            Remaining = budget.Limit,
            Month = budget.Month
        };

        return (true, dto, null);
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid id)
    {
        var budget = await _dbContext.Budgets
            .FirstOrDefaultAsync(b => b.UserId == userId && b.Id == id);

        if (budget == null)
        {
            return false;
        }

        _dbContext.Budgets.Remove(budget);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
