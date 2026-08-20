// Reports tests for GET /api/reports/*
// Activate tests (remove Skip) once report endpoints are implemented (#21).
// Note from sprint plan: these are the slowest calls in the app — coordinate
// with the DB pair (#34) on query efficiency before these go live.

using Microsoft.AspNetCore.Mvc.Testing;

namespace FinTrack.Tests.Reports;

public class ReportsTests(WebApplicationFactory<Program> factory)
    : IntegrationTestBase(factory)
{
    // -------------------------------------------------------------------------
    // GET /api/reports/spending-by-category
    // -------------------------------------------------------------------------

    [Fact(Skip = "Pending: GET /api/reports/spending-by-category (#21)")]
    public async Task SpendingByCategory_ValidDateRange_Returns200WithCorrectShape()
    {
        // Assert: from, to, totalSpent, categories[] with categoryId/categoryName/amount/percentOfTotal
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: GET /api/reports/spending-by-category (#21)")]
    public async Task SpendingByCategory_CategoriesSortedByAmountDescending()
    {
        // Add expenses: Food=600, Transport=200, Entertainment=400
        // Assert order: Food(600), Entertainment(400), Transport(200)
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: GET /api/reports/spending-by-category (#21)")]
    public async Task SpendingByCategory_PercentOfTotalSumsToHundred()
    {
        // All category percents must add up to 100 (within 0.1% tolerance)
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: GET /api/reports/spending-by-category (#21)")]
    public async Task SpendingByCategory_MissingFrom_Returns400()
    {
        // Both `from` and `to` are required — omitting either should fail
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: GET /api/reports/spending-by-category (#21)")]
    public async Task SpendingByCategory_MissingTo_Returns400()
    {
        throw new NotImplementedException();
    }

    // -------------------------------------------------------------------------
    // GET /api/reports/income-vs-expense
    // -------------------------------------------------------------------------

    [Theory(Skip = "Pending: GET /api/reports/income-vs-expense (#21)")]
    [InlineData("daily")]
    [InlineData("weekly")]
    [InlineData("monthly")]
    public async Task IncomeVsExpense_ValidGranularity_Returns200(string granularity)
    {
        // Assert: granularity matches, points[] contains period/income/expense/net
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: GET /api/reports/income-vs-expense (#21)")]
    public async Task IncomeVsExpense_InvalidGranularity_Returns400()
    {
        // granularity = "annual" or any unsupported value
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: GET /api/reports/income-vs-expense (#21)")]
    public async Task IncomeVsExpense_DefaultGranularityIsMonthly()
    {
        // Omit granularity param; assert response.granularity == "monthly"
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: GET /api/reports/income-vs-expense (#21)")]
    public async Task IncomeVsExpense_MonthWithNoTransactions_StillAppearsWithZeros()
    {
        // Request a range that includes a month with no data
        // Assert: that month's point has income=0, expense=0, net=0
        // Critical: the contract explicitly requires this to avoid chart gaps
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: GET /api/reports/income-vs-expense (#21)")]
    public async Task IncomeVsExpense_NetEqualsIncomeMinusExpense()
    {
        // For each point: assert net == income - expense exactly
        throw new NotImplementedException();
    }
}
