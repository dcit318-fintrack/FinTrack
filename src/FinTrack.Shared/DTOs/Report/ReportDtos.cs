namespace FinTrack.Shared.DTOs.Report;

public class SpendingByCategoryReportDto
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public decimal TotalSpent { get; set; }
    public List<CategorySpendingItemDto> Categories { get; set; } = new();
}

public class CategorySpendingItemDto
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public double PercentOfTotal { get; set; }
}

public class IncomeVsExpenseReportDto
{
    public string Granularity { get; set; } = "monthly"; // "daily" | "weekly" | "monthly"
    public List<IncomeVsExpensePointDto> Points { get; set; } = new();
}

public class IncomeVsExpensePointDto
{
    public string Period { get; set; } = string.Empty;
    public decimal Income { get; set; }
    public decimal Expense { get; set; }
    public decimal Net { get; set; }
}
