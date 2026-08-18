namespace FinTrack.Shared.DTOs.Budget;

public class BudgetDto
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Limit { get; set; }
    public decimal Spent { get; set; }
    public decimal Remaining { get; set; }
    public string Month { get; set; } = string.Empty; // "YYYY-MM"
}
