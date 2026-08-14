using System.ComponentModel.DataAnnotations;

namespace FinTrack.Shared.DTOs.Savings;

public class ContributeRequest
{
    [Range(0.01, double.MaxValue, ErrorMessage = "Contribution amount must be greater than zero")]
    public decimal Amount { get; set; }
}
