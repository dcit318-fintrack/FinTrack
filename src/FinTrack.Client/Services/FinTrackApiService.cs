using System.Net.Http.Json;
using FinTrack.Shared.DTOs.Auth;
using FinTrack.Shared.DTOs.Budget;
using FinTrack.Shared.DTOs.Category;
using FinTrack.Shared.DTOs.Dashboard;
using FinTrack.Shared.DTOs.Report;
using FinTrack.Shared.DTOs.Savings;
using FinTrack.Shared.DTOs.Transaction;

namespace FinTrack.Client.Services;

public class FinTrackApiService : IFinTrackApiService
{
    private readonly HttpClient _http;
    public event Action? OnDataChanged;

    // Seeded In-Memory State for Instant Interactive UI
    private readonly List<CategoryDto> _categories = new();
    private readonly List<TransactionDto> _transactions = new();
    private readonly List<BudgetDto> _budgets = new();
    private readonly List<SavingsGoalDto> _savingsGoals = new();

    public FinTrackApiService(HttpClient http)
    {
        _http = http;
        SeedInitialData();
    }

    public void NotifyDataChanged()
    {
        OnDataChanged?.Invoke();
    }

    private void SeedInitialData()
    {
        var foodCatId = Guid.NewGuid();
        var transportCatId = Guid.NewGuid();
        var rentCatId = Guid.NewGuid();
        var entertainmentCatId = Guid.NewGuid();
        var salaryCatId = Guid.NewGuid();
        var utilitiesCatId = Guid.NewGuid();

        _categories.AddRange(new[]
        {
            new CategoryDto { Id = foodCatId, Name = "Food & Groceries", Type = "Expense" },
            new CategoryDto { Id = transportCatId, Name = "Transport", Type = "Expense" },
            new CategoryDto { Id = rentCatId, Name = "Rent", Type = "Expense" },
            new CategoryDto { Id = entertainmentCatId, Name = "Entertainment", Type = "Expense" },
            new CategoryDto { Id = utilitiesCatId, Name = "Utilities", Type = "Expense" },
            new CategoryDto { Id = salaryCatId, Name = "Salary & Income", Type = "Income" }
        });

        // Transactions matching Figma mockups
        _transactions.AddRange(new[]
        {
            new TransactionDto
            {
                Id = Guid.NewGuid(),
                Description = "Whole Foods Market",
                Amount = 84.50m,
                Type = "Expense",
                CategoryId = foodCatId,
                CategoryName = "Groceries",
                Date = DateTime.Today.AddHours(10).AddMinutes(42)
            },
            new TransactionDto
            {
                Id = Guid.NewGuid(),
                Description = "Starbucks",
                Amount = 4.75m,
                Type = "Expense",
                CategoryId = foodCatId,
                CategoryName = "Coffee",
                Date = DateTime.Today.AddHours(8).AddMinutes(15)
            },
            new TransactionDto
            {
                Id = Guid.NewGuid(),
                Description = "Grocery",
                Amount = 50.00m,
                Type = "Expense",
                CategoryId = foodCatId,
                CategoryName = "Groceries",
                Date = DateTime.Today.AddHours(10).AddMinutes(24)
            },
            new TransactionDto
            {
                Id = Guid.NewGuid(),
                Description = "Tech Corp Inc.",
                Amount = 3250.00m,
                Type = "Income",
                CategoryId = salaryCatId,
                CategoryName = "Salary",
                Date = DateTime.Today.AddDays(-1).AddHours(9)
            },
            new TransactionDto
            {
                Id = Guid.NewGuid(),
                Description = "Salary",
                Amount = 3000.00m,
                Type = "Income",
                CategoryId = salaryCatId,
                CategoryName = "Salary",
                Date = DateTime.Today.AddDays(-1).AddHours(9)
            },
            new TransactionDto
            {
                Id = Guid.NewGuid(),
                Description = "Uber",
                Amount = 24.20m,
                Type = "Expense",
                CategoryId = transportCatId,
                CategoryName = "Transport",
                Date = DateTime.Today.AddDays(-1).AddHours(18).AddMinutes(30)
            },
            new TransactionDto
            {
                Id = Guid.NewGuid(),
                Description = "Rent",
                Amount = 600.00m,
                Type = "Expense",
                CategoryId = rentCatId,
                CategoryName = "Rent",
                Date = DateTime.Today.AddDays(-5).AddHours(12)
            }
        });

        // Budgets matching Figma
        _budgets.AddRange(new[]
        {
            new BudgetDto
            {
                Id = Guid.NewGuid(),
                CategoryId = foodCatId,
                CategoryName = "Food",
                Limit = 500.00m,
                Spent = 400.00m,
                Remaining = 100.00m,
                Month = DateTime.Today.ToString("yyyy-MM")
            },
            new BudgetDto
            {
                Id = Guid.NewGuid(),
                CategoryId = transportCatId,
                CategoryName = "Transport",
                Limit = 150.00m,
                Spent = 120.00m,
                Remaining = 30.00m,
                Month = DateTime.Today.ToString("yyyy-MM")
            },
            new BudgetDto
            {
                Id = Guid.NewGuid(),
                CategoryId = entertainmentCatId,
                CategoryName = "Entertainment",
                Limit = 100.00m,
                Spent = 80.00m,
                Remaining = 20.00m,
                Month = DateTime.Today.ToString("yyyy-MM")
            }
        });

        // Savings Goals matching Figma
        _savingsGoals.AddRange(new[]
        {
            new SavingsGoalDto
            {
                Id = Guid.NewGuid(),
                Name = "New Laptop",
                TargetAmount = 2000.00m,
                CurrentAmount = 1500.00m,
                ProgressPercent = 75.0,
                TargetDate = DateTime.Today.AddMonths(4),
                IsAchieved = false
            },
            new SavingsGoalDto
            {
                Id = Guid.NewGuid(),
                Name = "Emergency Fund",
                TargetAmount = 5000.00m,
                CurrentAmount = 1200.00m,
                ProgressPercent = 24.0,
                TargetDate = DateTime.Today.AddYears(1),
                IsAchieved = false
            },
            new SavingsGoalDto
            {
                Id = Guid.NewGuid(),
                Name = "Vacation",
                TargetAmount = 2000.00m,
                CurrentAmount = 450.00m,
                ProgressPercent = 22.5,
                TargetDate = DateTime.Today.AddMonths(8),
                IsAchieved = false
            }
        });
    }

    #region Dashboard
    public Task<DashboardSummaryDto> GetDashboardSummaryAsync(string? month = null)
    {
        var currentMonth = month ?? DateTime.Today.ToString("yyyy-MM");
        var income = _transactions.Where(t => t.Type == "Income").Sum(t => t.Amount);
        var expense = _transactions.Where(t => t.Type == "Expense").Sum(t => t.Amount);
        var balance = income - expense;

        if (balance == 0 && income == 0)
        {
            // Default matching Figma
            income = 3200m;
            expense = 750m;
            balance = 2450m;
        }

        var result = new DashboardSummaryDto
        {
            TotalIncome = income > 0 ? income : 3200m,
            TotalExpenses = expense > 0 ? expense : 750m,
            Balance = balance > 0 ? balance : 2450m,
            Month = currentMonth,
            RecentTransactions = _transactions.OrderByDescending(t => t.Date).Take(5).ToList(),
            BudgetsAtRisk = _budgets
                .Where(b => b.Limit > 0 && ((double)(b.Spent / b.Limit) >= 0.8))
                .Select(b => new BudgetRiskDto
                {
                    CategoryName = b.CategoryName,
                    Limit = b.Limit,
                    Spent = b.Spent,
                    PercentUsed = (double)(b.Spent / b.Limit) * 100.0
                }).ToList()
        };

        return Task.FromResult(result);
    }
    #endregion

    #region Transactions
    public Task<PagedResult<TransactionDto>> GetTransactionsAsync(string? search = null, string? type = null, Guid? categoryId = null, int page = 1, int pageSize = 50)
    {
        var query = _transactions.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(t => 
                t.Description.Contains(search, StringComparison.OrdinalIgnoreCase) || 
                t.CategoryName.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(type) && type != "All")
        {
            query = query.Where(t => t.Type.Equals(type, StringComparison.OrdinalIgnoreCase));
        }

        if (categoryId.HasValue && categoryId.Value != Guid.Empty)
        {
            query = query.Where(t => t.CategoryId == categoryId.Value);
        }

        var list = query.OrderByDescending(t => t.Date).ToList();
        var paged = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return Task.FromResult(new PagedResult<TransactionDto>
        {
            Items = paged,
            Page = page,
            PageSize = pageSize,
            TotalCount = list.Count
        });
    }

    public Task<TransactionDto?> GetTransactionByIdAsync(Guid id)
    {
        var item = _transactions.FirstOrDefault(t => t.Id == id);
        return Task.FromResult(item);
    }

    public Task<TransactionDto> CreateTransactionAsync(CreateTransactionRequest request)
    {
        var category = _categories.FirstOrDefault(c => c.Id == request.CategoryId);
        var item = new TransactionDto
        {
            Id = Guid.NewGuid(),
            Amount = request.Amount,
            Type = request.Type,
            CategoryId = request.CategoryId,
            CategoryName = category?.Name ?? "Other",
            Description = request.Description,
            Date = request.Date
        };

        _transactions.Insert(0, item);
        NotifyDataChanged();
        return Task.FromResult(item);
    }

    public Task<TransactionDto> UpdateTransactionAsync(Guid id, UpdateTransactionRequest request)
    {
        var existing = _transactions.FirstOrDefault(t => t.Id == id);
        if (existing == null) throw new KeyNotFoundException("Transaction not found");

        var category = _categories.FirstOrDefault(c => c.Id == request.CategoryId);
        existing.Amount = request.Amount;
        existing.Type = request.Type;
        existing.CategoryId = request.CategoryId;
        existing.CategoryName = category?.Name ?? existing.CategoryName;
        existing.Description = request.Description;
        existing.Date = request.Date;

        NotifyDataChanged();
        return Task.FromResult(existing);
    }

    public Task<bool> DeleteTransactionAsync(Guid id)
    {
        var item = _transactions.FirstOrDefault(t => t.Id == id);
        if (item != null)
        {
            _transactions.Remove(item);
            NotifyDataChanged();
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
    #endregion

    #region Categories
    public Task<List<CategoryDto>> GetCategoriesAsync()
    {
        return Task.FromResult(_categories.ToList());
    }

    public Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request)
    {
        var cat = new CategoryDto
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Type = request.Type
        };
        _categories.Add(cat);
        NotifyDataChanged();
        return Task.FromResult(cat);
    }
    #endregion

    #region Budgets
    public Task<List<BudgetDto>> GetBudgetsAsync(string? month = null)
    {
        return Task.FromResult(_budgets.ToList());
    }

    public Task<BudgetDto> CreateBudgetAsync(CreateBudgetRequest request)
    {
        var category = _categories.FirstOrDefault(c => c.Id == request.CategoryId);
        var budget = new BudgetDto
        {
            Id = Guid.NewGuid(),
            CategoryId = request.CategoryId,
            CategoryName = category?.Name ?? "General",
            Limit = request.Limit,
            Spent = 0m,
            Remaining = request.Limit,
            Month = request.Month
        };

        _budgets.Add(budget);
        NotifyDataChanged();
        return Task.FromResult(budget);
    }

    public Task<BudgetDto> UpdateBudgetAsync(Guid id, UpdateBudgetRequest request)
    {
        var budget = _budgets.FirstOrDefault(b => b.Id == id);
        if (budget == null) throw new KeyNotFoundException("Budget not found");

        budget.Limit = request.Limit;
        budget.Remaining = Math.Max(0, budget.Limit - budget.Spent);

        NotifyDataChanged();
        return Task.FromResult(budget);
    }

    public Task<bool> DeleteBudgetAsync(Guid id)
    {
        var budget = _budgets.FirstOrDefault(b => b.Id == id);
        if (budget != null)
        {
            _budgets.Remove(budget);
            NotifyDataChanged();
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
    #endregion

    #region Savings Goals
    public Task<List<SavingsGoalDto>> GetSavingsGoalsAsync()
    {
        return Task.FromResult(_savingsGoals.ToList());
    }

    public Task<SavingsGoalDto> CreateSavingsGoalAsync(CreateSavingsGoalRequest request)
    {
        var goal = new SavingsGoalDto
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            TargetAmount = request.TargetAmount,
            CurrentAmount = 0m,
            ProgressPercent = 0.0,
            TargetDate = request.TargetDate,
            IsAchieved = false
        };

        _savingsGoals.Add(goal);
        NotifyDataChanged();
        return Task.FromResult(goal);
    }

    public Task<SavingsGoalDto> UpdateSavingsGoalAsync(Guid id, UpdateSavingsGoalRequest request)
    {
        var goal = _savingsGoals.FirstOrDefault(g => g.Id == id);
        if (goal == null) throw new KeyNotFoundException("Goal not found");

        goal.Name = request.Name;
        goal.TargetAmount = request.TargetAmount;
        if (request.CurrentAmount.HasValue)
        {
            goal.CurrentAmount = request.CurrentAmount.Value;
        }
        goal.ProgressPercent = goal.TargetAmount > 0 
            ? Math.Round((double)(goal.CurrentAmount / goal.TargetAmount) * 100.0, 1) 
            : 0.0;
        goal.TargetDate = request.TargetDate;
        goal.IsAchieved = goal.CurrentAmount >= goal.TargetAmount;

        NotifyDataChanged();
        return Task.FromResult(goal);
    }

    public Task<SavingsGoalDto> ContributeToSavingsGoalAsync(Guid id, ContributeRequest request)
    {
        var goal = _savingsGoals.FirstOrDefault(g => g.Id == id);
        if (goal == null) throw new KeyNotFoundException("Goal not found");

        goal.CurrentAmount += request.Amount;
        goal.ProgressPercent = goal.TargetAmount > 0 
            ? Math.Round((double)(goal.CurrentAmount / goal.TargetAmount) * 100.0, 1) 
            : 0.0;
        goal.IsAchieved = goal.CurrentAmount >= goal.TargetAmount;

        NotifyDataChanged();
        return Task.FromResult(goal);
    }

    public Task<bool> DeleteSavingsGoalAsync(Guid id)
    {
        var goal = _savingsGoals.FirstOrDefault(g => g.Id == id);
        if (goal != null)
        {
            _savingsGoals.Remove(goal);
            NotifyDataChanged();
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
    #endregion

    #region Reports
    public Task<SpendingByCategoryReportDto> GetSpendingByCategoryReportAsync(DateTime from, DateTime to)
    {
        var result = new SpendingByCategoryReportDto
        {
            From = from,
            To = to,
            TotalSpent = 4250.00m,
            Categories = new List<CategorySpendingItemDto>
            {
                new() { CategoryId = Guid.NewGuid(), CategoryName = "Rent", Amount = 1912.50m, PercentOfTotal = 45.0 },
                new() { CategoryId = Guid.NewGuid(), CategoryName = "Food", Amount = 1062.50m, PercentOfTotal = 25.0 },
                new() { CategoryId = Guid.NewGuid(), CategoryName = "Utilities", Amount = 637.50m, PercentOfTotal = 15.0 },
                new() { CategoryId = Guid.NewGuid(), CategoryName = "Fun", Amount = 425.00m, PercentOfTotal = 10.0 }
            }
        };

        return Task.FromResult(result);
    }

    public Task<IncomeVsExpenseReportDto> GetIncomeVsExpenseReportAsync(DateTime from, DateTime to, string granularity = "monthly")
    {
        var result = new IncomeVsExpenseReportDto
        {
            Granularity = granularity,
            Points = new List<IncomeVsExpensePointDto>
            {
                new() { Period = "Jan", Income = 3200m, Expense = 1800m, Net = 1400m },
                new() { Period = "Feb", Income = 3400m, Expense = 2100m, Net = 1300m },
                new() { Period = "Mar", Income = 3100m, Expense = 1950m, Net = 1150m },
                new() { Period = "Apr", Income = 4500m, Expense = 2400m, Net = 2100m },
                new() { Period = "May", Income = 3600m, Expense = 2200m, Net = 1400m },
                new() { Period = "Jun", Income = 3200m, Expense = 750m, Net = 2450m }
            }
        };

        return Task.FromResult(result);
    }
    #endregion
}
