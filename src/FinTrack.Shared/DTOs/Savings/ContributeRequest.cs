using System.ComponentModel.DataAnnotations;

namespace FinTrack.Shared.DTOs.Savings;

public class ContributeRequest
{
    [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "Contribution amount must be greater than zero.")]
    public decimal Amount { get; set; }
}
