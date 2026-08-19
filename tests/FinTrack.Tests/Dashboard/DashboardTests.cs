// Dashboard tests for GET /api/dashboard
// Activate tests (remove Skip) once the dashboard endpoint is implemented.

using Microsoft.AspNetCore.Mvc.Testing;

namespace FinTrack.Tests.Dashboard;

public class DashboardTests(WebApplicationFactory<Program> factory)
    : IntegrationTestBase(factory)
{
    [Fact(Skip = "Pending: GET /api/dashboard (depends on #18 + #19)")]
    public async Task GetDashboard_Returns200WithAllExpectedFields()
    {
        // Assert: totalIncome, totalExpenses, balance, month, recentTransactions, budgetsAtRisk
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: GET /api/dashboard (depends on #18 + #19)")]
    public async Task GetDashboard_BalanceCalculation_IsCorrect()
    {
        // Create income=3000, expenses=1000
        // Assert: balance=2000.00, totalIncome=3000.00, totalExpenses=1000.00
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: GET /api/dashboard (depends on #18 + #19)")]
    public async Task GetDashboard_RecentTransactions_CappedAtFive()
    {
        // Add 10 transactions
        // Assert: recentTransactions.Length == 5
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: GET /api/dashboard (depends on #19)")]
    public async Task GetDashboard_BudgetsAtRisk_IncludesAt80Percent()
    {
        // Budget limit=500, spent=400 (exactly 80%) → must be in budgetsAtRisk
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: GET /api/dashboard (depends on #19)")]
    public async Task GetDashboard_BudgetsAtRisk_ExcludesBelow80Percent()
    {
        // Budget limit=500, spent=399 (79.8%) → must NOT be in budgetsAtRisk
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: GET /api/dashboard (depends on #19)")]
    public async Task GetDashboard_BudgetsAtRisk_HasCorrectShape()
    {
        // Assert each item has: categoryName, limit, spent, percentUsed
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: GET /api/dashboard (depends on #18 + #19)")]
    public async Task GetDashboard_FilterByMonth_ReturnsOnlyThatMonthData()
    {
        // Create data in two different months; request ?month=2026-07
        // Assert: only July income/expenses/budgets returned
        throw new NotImplementedException();
    }
}
