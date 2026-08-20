namespace FinTrack.Server.Models;

public class SavingsGoal
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public DateTime TargetDate { get; set; }
    public bool IsAchieved { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
