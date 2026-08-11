using System.ComponentModel.DataAnnotations;

namespace FinTrack.Shared.DTOs.Budget;

public class UpdateBudgetRequest
{
    [Range(0.01, double.MaxValue, ErrorMessage = "Limit must be greater than zero")]
    public decimal Limit { get; set; }
}
