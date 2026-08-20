namespace FinTrack.Server.Models;

public class Budget
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public decimal Limit { get; set; }
    public string Month { get; set; } = string.Empty; // "YYYY-MM"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
