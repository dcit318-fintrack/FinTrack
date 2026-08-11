using FinTrack.Shared.DTOs.Savings;

namespace FinTrack.Server.Services.Savings;

public interface ISavingsGoalService
{
    Task<List<SavingsGoalDto>> GetSavingsGoalsAsync(Guid userId);

    Task<(bool success, SavingsGoalDto? dto, string? errorMessage, Dictionary<string, string[]>? errors)> CreateAsync(
        Guid userId,
        CreateSavingsGoalRequest request);

    Task<(bool success, SavingsGoalDto? dto, string? errorMessage)> UpdateAsync(
        Guid userId,
        Guid id,
        UpdateSavingsGoalRequest request);

    Task<(bool success, SavingsGoalDto? dto, string? errorMessage)> ContributeAsync(
        Guid userId,
        Guid id,
        ContributeRequest request);

    Task<bool> DeleteAsync(Guid userId, Guid id);
}
