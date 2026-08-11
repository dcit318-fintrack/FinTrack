using System.ComponentModel.DataAnnotations;

namespace FinTrack.Shared.DTOs.Transaction;

public class UpdateTransactionRequest
{
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
    public decimal Amount { get; set; }

    [Required]
    public string Type { get; set; } = string.Empty;

    [Required]
    public Guid CategoryId { get; set; }

    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }
}
