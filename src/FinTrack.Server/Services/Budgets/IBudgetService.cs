using FinTrack.Shared.DTOs.Budget;
using FinTrack.Shared.DTOs.Common;

namespace FinTrack.Server.Services.Budgets;

public interface IBudgetService
{
    Task<List<BudgetDto>> GetBudgetsAsync(Guid userId, string month);

    Task<Result<BudgetDto>> CreateAsync(
        Guid userId,
        CreateBudgetRequest request);

    Task<Result<BudgetDto>> UpdateAsync(
        Guid userId,
        Guid id,
        UpdateBudgetRequest request);

    Task<bool> DeleteAsync(Guid userId, Guid id);
}
