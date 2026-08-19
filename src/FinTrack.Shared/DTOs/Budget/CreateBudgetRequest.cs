using System.ComponentModel.DataAnnotations;

namespace FinTrack.Shared.DTOs.Budget;

public class CreateBudgetRequest
{
    [Required(ErrorMessage = "CategoryId is required.")]
    public Guid? CategoryId { get; set; }

    [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "Limit must be greater than zero.")]
    public decimal Limit { get; set; }

    [Required, RegularExpression(@"^\d{4}-\d{2}$", ErrorMessage = "Month must be in YYYY-MM format.")]
    public string Month { get; set; } = string.Empty;
}
