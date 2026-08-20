using FinTrack.Shared.DTOs.Auth;
using FinTrack.Shared.DTOs.Budget;
using FinTrack.Shared.DTOs.Category;
using FinTrack.Shared.DTOs.Dashboard;
using FinTrack.Shared.DTOs.Report;
using FinTrack.Shared.DTOs.Savings;
using FinTrack.Shared.DTOs.Transaction;

namespace FinTrack.Client.Services;

public interface IFinTrackApiService
{
    // Dashboard
    Task<DashboardSummaryDto> GetDashboardSummaryAsync(string? month = null);

    // Transactions
    Task<PagedResult<TransactionDto>> GetTransactionsAsync(string? search = null, string? type = null, Guid? categoryId = null, int page = 1, int pageSize = 50);
    Task<TransactionDto?> GetTransactionByIdAsync(Guid id);
    Task<TransactionDto> CreateTransactionAsync(CreateTransactionRequest request);
    Task<TransactionDto> UpdateTransactionAsync(Guid id, UpdateTransactionRequest request);
    Task<bool> DeleteTransactionAsync(Guid id);

    // Categories
    Task<List<CategoryDto>> GetCategoriesAsync();
    Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request);

    // Budgets
    Task<List<BudgetDto>> GetBudgetsAsync(string? month = null);
    Task<BudgetDto> CreateBudgetAsync(CreateBudgetRequest request);
    Task<BudgetDto> UpdateBudgetAsync(Guid id, UpdateBudgetRequest request);
    Task<bool> DeleteBudgetAsync(Guid id);

    // Savings Goals
    Task<List<SavingsGoalDto>> GetSavingsGoalsAsync();
    Task<SavingsGoalDto> CreateSavingsGoalAsync(CreateSavingsGoalRequest request);
    Task<SavingsGoalDto> UpdateSavingsGoalAsync(Guid id, UpdateSavingsGoalRequest request);
    Task<SavingsGoalDto> ContributeToSavingsGoalAsync(Guid id, ContributeRequest request);
    Task<bool> DeleteSavingsGoalAsync(Guid id);

    // Reports
    Task<SpendingByCategoryReportDto> GetSpendingByCategoryReportAsync(DateTime from, DateTime to);
    Task<IncomeVsExpenseReportDto> GetIncomeVsExpenseReportAsync(DateTime from, DateTime to, string granularity = "monthly");

    // Event notification when data changes so all screens can auto-refresh
    event Action? OnDataChanged;
    void NotifyDataChanged();
}
