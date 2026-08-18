using System.ComponentModel.DataAnnotations;

namespace FinTrack.Shared.DTOs.Savings;

public class UpdateSavingsGoalRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "Target amount must be greater than zero.")]
    public decimal TargetAmount { get; set; }

    public decimal? CurrentAmount { get; set; }

    [Required]
    public DateTime TargetDate { get; set; }
}
