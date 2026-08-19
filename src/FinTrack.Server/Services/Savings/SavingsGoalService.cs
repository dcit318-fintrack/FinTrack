using FinTrack.Server.Data;
using FinTrack.Server.Models;
using FinTrack.Shared.DTOs.Savings;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Server.Services.Savings;

public class SavingsGoalService : ISavingsGoalService
{
    private readonly FinTrackDbContext _dbContext;

    public SavingsGoalService(FinTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<SavingsGoalDto>> GetSavingsGoalsAsync(Guid userId)
    {
        var goals = await _dbContext.SavingsGoals
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .OrderBy(s => s.TargetDate)
            .ToListAsync();

        return goals.Select(MapToDto).ToList();
    }

    public async Task<(bool success, SavingsGoalDto? dto, string? errorMessage, Dictionary<string, string[]>? errors)> CreateAsync(
        Guid userId,
        CreateSavingsGoalRequest request)
    {
        if (request.TargetDate <= DateTime.UtcNow)
        {
            var errors = new Dictionary<string, string[]>
            {
                { "targetDate", new[] { "Target date must be in the future." } }
            };
            return (false, null, "Validation failed.", errors);
        }

        var goal = new SavingsGoal
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = request.Name,
            TargetAmount = Math.Round(request.TargetAmount, 2),
            CurrentAmount = 0m,
            TargetDate = request.TargetDate,
            IsAchieved = false,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.SavingsGoals.Add(goal);
        await _dbContext.SaveChangesAsync();

        return (true, MapToDto(goal), null, null);
    }

    public async Task<(bool success, SavingsGoalDto? dto, string? errorMessage)> UpdateAsync(
        Guid userId,
        Guid id,
        UpdateSavingsGoalRequest request)
    {
        var goal = await _dbContext.SavingsGoals
            .FirstOrDefaultAsync(s => s.UserId == userId && s.Id == id);

        if (goal == null)
        {
            return (false, null, "Savings goal not found.");
        }

        goal.Name = request.Name;
        goal.TargetAmount = Math.Round(request.TargetAmount, 2);
        if (request.CurrentAmount.HasValue)
        {
            goal.CurrentAmount = Math.Round(request.CurrentAmount.Value, 2);
        }
        goal.TargetDate = request.TargetDate;
        goal.IsAchieved = goal.CurrentAmount >= goal.TargetAmount;

        await _dbContext.SaveChangesAsync();

        return (true, MapToDto(goal), null);
    }

    public async Task<(bool success, SavingsGoalDto? dto, string? errorMessage)> ContributeAsync(
        Guid userId,
        Guid id,
        ContributeRequest request)
    {
        var goal = await _dbContext.SavingsGoals
            .FirstOrDefaultAsync(s => s.UserId == userId && s.Id == id);

        if (goal == null)
        {
            return (false, null, "Savings goal not found.");
        }

        goal.CurrentAmount += Math.Round(request.Amount, 2);
        goal.IsAchieved = goal.CurrentAmount >= goal.TargetAmount;

        await _dbContext.SaveChangesAsync();

        return (true, MapToDto(goal), null);
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid id)
    {
        var goal = await _dbContext.SavingsGoals
            .FirstOrDefaultAsync(s => s.UserId == userId && s.Id == id);

        if (goal == null)
        {
            return false;
        }

        _dbContext.SavingsGoals.Remove(goal);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private static SavingsGoalDto MapToDto(SavingsGoal goal)
    {
        var progressPercent = goal.TargetAmount > 0
            ? Math.Min(100.0, Math.Round((double)(goal.CurrentAmount / goal.TargetAmount) * 100, 1))
            : 0.0;

        return new SavingsGoalDto
        {
            Id = goal.Id,
            Name = goal.Name,
            TargetAmount = goal.TargetAmount,
            CurrentAmount = goal.CurrentAmount,
            ProgressPercent = progressPercent,
            TargetDate = goal.TargetDate,
            IsAchieved = goal.IsAchieved || goal.CurrentAmount >= goal.TargetAmount
        };
    }
}
