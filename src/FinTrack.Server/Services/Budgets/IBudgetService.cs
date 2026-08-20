using FinTrack.Shared.DTOs.Budget;

namespace FinTrack.Server.Services.Budgets;

public interface IBudgetService
{
    Task<List<BudgetDto>> GetBudgetsAsync(Guid userId, string month);

    Task<(bool success, BudgetDto? dto, string? errorMessage, Dictionary<string, string[]>? errors, bool isConflict)> CreateAsync(
        Guid userId,
        CreateBudgetRequest request);

    Task<(bool success, BudgetDto? dto, string? errorMessage)> UpdateAsync(
        Guid userId,
        Guid id,
        UpdateBudgetRequest request);

    Task<bool> DeleteAsync(Guid userId, Guid id);
}
