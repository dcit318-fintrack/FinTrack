using System.ComponentModel.DataAnnotations;

namespace FinTrack.Shared.DTOs.Budget;

public class CreateBudgetRequest
{
    [Required]
    public Guid CategoryId { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Limit must be greater than zero")]
    public decimal Limit { get; set; }

    [Required, RegularExpression(@"^\d{4}-\d{2}$", ErrorMessage = "Month must be in YYYY-MM format")]
    public string Month { get; set; } = string.Empty;
}
