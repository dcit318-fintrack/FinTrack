using System.ComponentModel.DataAnnotations;

namespace FinTrack.Shared.DTOs.Savings;

public class CreateSavingsGoalRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "Target amount must be greater than zero")]
    public decimal TargetAmount { get; set; }

    [Required]
    public DateTime TargetDate { get; set; }
}
