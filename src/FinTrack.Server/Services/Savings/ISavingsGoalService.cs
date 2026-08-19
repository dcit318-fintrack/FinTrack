using FinTrack.Shared.DTOs.Common;
using FinTrack.Shared.DTOs.Savings;

namespace FinTrack.Server.Services.Savings;

public interface ISavingsGoalService
{
    Task<List<SavingsGoalDto>> GetSavingsGoalsAsync(Guid userId);

    Task<Result<SavingsGoalDto>> CreateAsync(
        Guid userId,
        CreateSavingsGoalRequest request);

    Task<Result<SavingsGoalDto>> UpdateAsync(
        Guid userId,
        Guid id,
        UpdateSavingsGoalRequest request);

    Task<Result<SavingsGoalDto>> ContributeAsync(
        Guid userId,
        Guid id,
        ContributeRequest request);

    Task<bool> DeleteAsync(Guid userId, Guid id);
}
