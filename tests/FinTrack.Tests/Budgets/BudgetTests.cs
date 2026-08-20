// Budget tests for /api/budgets
// Activate tests (remove Skip) once budget endpoints are implemented (#19).

using Microsoft.AspNetCore.Mvc.Testing;

namespace FinTrack.Tests.Budgets;

public class BudgetTests(WebApplicationFactory<Program> factory)
    : IntegrationTestBase(factory)
{
    // -------------------------------------------------------------------------
    // POST /api/budgets
    // -------------------------------------------------------------------------

    [Fact(Skip = "Pending: POST /api/budgets (#19)")]
    public async Task CreateBudget_ValidData_Returns201()
    {
        // Arrange: authenticate, get a valid categoryId
        // Act: POST { categoryId, limit: 500.00, month: "2026-08" }
        // Assert: 201
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: POST /api/budgets (#19)")]
    public async Task CreateBudget_DuplicateCategoryAndMonth_Returns409()
    {
        // Create same budget twice
        // Assert: second POST returns 409 Conflict
        throw new NotImplementedException();
    }

    [Theory(Skip = "Pending: POST /api/budgets (#19)")]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-500)]
    public async Task CreateBudget_LimitNotPositive_Returns400(decimal limit)
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: POST /api/budgets (#19)")]
    public async Task CreateBudget_NonExistentCategoryId_Returns400()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: POST /api/budgets (#19)")]
    public async Task CreateBudget_MalformedMonth_Returns400()
    {
        // month = "08-2026" instead of "2026-08"
        throw new NotImplementedException();
    }

    // -------------------------------------------------------------------------
    // GET /api/budgets
    // -------------------------------------------------------------------------

    [Fact(Skip = "Pending: GET /api/budgets (#19)")]
    public async Task GetBudgets_DefaultsToCurrentMonth_ReturnsCorrectShape()
    {
        // Assert each item has: id, categoryId, categoryName, limit, spent, remaining, month
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: GET /api/budgets (#19)")]
    public async Task GetBudgets_SpentPlusRemainingEqualsLimit()
    {
        // Create a budget, add a transaction for that category and month
        // Assert: spent + remaining == limit exactly (decimal precision)
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: GET /api/budgets (#19)")]
    public async Task GetBudgets_SpentReflectsRealTransactions()
    {
        // Budget: Food, limit=500
        // Add Food expense of 100
        // Assert: spent=100.00, remaining=400.00
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: GET /api/budgets (#19)")]
    public async Task GetBudgets_FilterByMonth_ReturnsOnlyThatMonth()
    {
        // Create budgets for two different months
        // Request ?month=2026-07 and assert only July budgets returned
        throw new NotImplementedException();
    }

    // -------------------------------------------------------------------------
    // PUT /api/budgets/{id}
    // -------------------------------------------------------------------------

    [Fact(Skip = "Pending: PUT /api/budgets/{id} (#19)")]
    public async Task UpdateBudgetLimit_ValidData_Returns200WithNewLimit()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: PUT /api/budgets/{id} (#19)")]
    public async Task UpdateBudget_BelongsToAnotherUser_Returns404()
    {
        throw new NotImplementedException();
    }

    // -------------------------------------------------------------------------
    // DELETE /api/budgets/{id}
    // -------------------------------------------------------------------------

    [Fact(Skip = "Pending: DELETE /api/budgets/{id} (#19)")]
    public async Task DeleteBudget_Exists_Returns204()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: DELETE /api/budgets/{id} (#19)")]
    public async Task DeleteBudget_BelongsToAnotherUser_Returns404()
    {
        throw new NotImplementedException();
    }
}
