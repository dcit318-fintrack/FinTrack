using System.ComponentModel.DataAnnotations;

namespace FinTrack.Shared.DTOs.Budget;

public class UpdateBudgetRequest
{
    [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "Limit must be greater than zero.")]
    public decimal Limit { get; set; }
}
