using System.ComponentModel.DataAnnotations;

namespace FinTrack.Shared.DTOs.Transaction;

public class UpdateTransactionRequest
{
    [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }

    [Required]
    public string Type { get; set; } = string.Empty;

    [Required(ErrorMessage = "CategoryId is required.")]
    public Guid? CategoryId { get; set; }

    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }
}
