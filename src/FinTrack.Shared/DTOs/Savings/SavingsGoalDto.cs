namespace FinTrack.Shared.DTOs.Savings;

public class SavingsGoalDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public double ProgressPercent { get; set; }
    public DateTime TargetDate { get; set; }
    public bool IsAchieved { get; set; }
}
