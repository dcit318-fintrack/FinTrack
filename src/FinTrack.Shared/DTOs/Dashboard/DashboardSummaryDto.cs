using FinTrack.Shared.DTOs.Transaction;

namespace FinTrack.Shared.DTOs.Dashboard;

public class DashboardSummaryDto
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal Balance { get; set; }
    public string Month { get; set; } = string.Empty;
    public List<TransactionDto> RecentTransactions { get; set; } = new();
    public List<BudgetRiskDto> BudgetsAtRisk { get; set; } = new();
}

public class BudgetRiskDto
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal Limit { get; set; }
    public decimal Spent { get; set; }
    public double PercentUsed { get; set; }
}
