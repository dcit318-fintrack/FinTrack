using System.ComponentModel.DataAnnotations;

namespace FinTrack.Shared.DTOs.Category;

public class CreateCategoryRequest
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(Income|Expense)$", ErrorMessage = "Category type must be 'Income' or 'Expense'.")]
    public string Type { get; set; } = string.Empty;
}
