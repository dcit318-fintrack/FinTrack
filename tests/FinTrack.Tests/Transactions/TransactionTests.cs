// Transaction tests for /api/transactions
// Activate tests (remove Skip) once CRUD endpoints are implemented (#18).

using Microsoft.AspNetCore.Mvc.Testing;

namespace FinTrack.Tests.Transactions;

public class TransactionTests(WebApplicationFactory<Program> factory)
    : IntegrationTestBase(factory)
{
    // -------------------------------------------------------------------------
    // POST /api/transactions
    // -------------------------------------------------------------------------

    [Fact(Skip = "Pending: POST /api/transactions (#18)")]
    public async Task CreateTransaction_Expense_Returns201WithCategoryName()
    {
        // Arrange: authenticate, get a valid categoryId for Food
        // Act: POST valid expense
        // Assert: 201, Location header, body.categoryName == "Food"
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: POST /api/transactions (#18)")]
    public async Task CreateTransaction_Income_Returns201()
    {
        throw new NotImplementedException();
    }

    [Theory(Skip = "Pending: POST /api/transactions (#18)")]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-999.99)]
    public async Task CreateTransaction_AmountNotPositive_Returns400(decimal amount)
    {
        // Act: POST with the given amount
        // Assert: 400, errors.amount present
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: POST /api/transactions (#18)")]
    public async Task CreateTransaction_TypeOmitted_Returns400()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: POST /api/transactions (#18)")]
    public async Task CreateTransaction_InvalidType_Returns400()
    {
        // type = "Cash" or any value other than "Income"/"Expense"
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: POST /api/transactions (#18)")]
    public async Task CreateTransaction_NonExistentCategoryId_Returns400()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: POST /api/transactions (#18)")]
    public async Task CreateTransaction_Description201Chars_Returns400()
    {
        // description is one char over the 200-char limit
        var description = new string('x', 201);
        // POST and assert 400
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: POST /api/transactions (#18)")]
    public async Task CreateTransaction_Description200Chars_Returns201()
    {
        // Boundary: exactly at the limit must succeed
        var description = new string('x', 200);
        // POST and assert 201
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: POST /api/transactions (#18)")]
    public async Task CreateTransaction_DateInFuture_Returns400()
    {
        // date = DateTime.UtcNow.AddDays(1)
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: POST /api/transactions (#18)")]
    public async Task CreateTransaction_DateToday_Returns201()
    {
        // Boundary: today must pass
        throw new NotImplementedException();
    }

    // -------------------------------------------------------------------------
    // GET /api/transactions
    // -------------------------------------------------------------------------

    [Fact(Skip = "Pending: GET /api/transactions (#18)")]
    public async Task GetTransactions_DefaultPagination_ReturnsPage1Size25()
    {
        // Assert: page=1, pageSize=25, totalCount>=0, items is array
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: GET /api/transactions (#18)")]
    public async Task GetTransactions_FilterByDateRange_ReturnsOnlyMatchingItems()
    {
        // Create two transactions with different dates, filter to include only one
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: GET /api/transactions (#18)")]
    public async Task GetTransactions_FilterByCategoryId_ReturnsOnlyThatCategory()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: GET /api/transactions (#18)")]
    public async Task GetTransactions_FilterByType_ReturnsOnlyThatType()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: GET /api/transactions (#18)")]
    public async Task GetTransactions_SecondPage_ReturnsCorrectItems()
    {
        // Create 30 transactions; request page=2&pageSize=10
        // Assert: 10 items, totalCount=30
        throw new NotImplementedException();
    }

    // -------------------------------------------------------------------------
    // GET /api/transactions/{id}
    // -------------------------------------------------------------------------

    [Fact(Skip = "Pending: GET /api/transactions/{id} (#18)")]
    public async Task GetTransaction_Exists_Returns200()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: GET /api/transactions/{id} (#18)")]
    public async Task GetTransaction_NotFound_Returns404()
    {
        var response = await Client.GetAsync($"/api/transactions/{Guid.NewGuid()}");
        // Once auth is in, use AuthenticatedClient here
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(Skip = "Pending: GET /api/transactions/{id} (#18)")]
    public async Task GetTransaction_BelongsToAnotherUser_Returns404NotForbidden()
    {
        // User A creates; User B requests — must get 404, not 403
        // Critical: do NOT leak resource existence
        throw new NotImplementedException();
    }

    // -------------------------------------------------------------------------
    // PUT /api/transactions/{id}
    // -------------------------------------------------------------------------

    [Fact(Skip = "Pending: PUT /api/transactions/{id} (#18)")]
    public async Task UpdateTransaction_ValidData_Returns200WithUpdatedFields()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: PUT /api/transactions/{id} (#18)")]
    public async Task UpdateTransaction_NotFound_Returns404()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: PUT /api/transactions/{id} (#18)")]
    public async Task UpdateTransaction_BelongsToAnotherUser_Returns404()
    {
        throw new NotImplementedException();
    }

    // -------------------------------------------------------------------------
    // DELETE /api/transactions/{id}
    // -------------------------------------------------------------------------

    [Fact(Skip = "Pending: DELETE /api/transactions/{id} (#18)")]
    public async Task DeleteTransaction_Exists_Returns204()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: DELETE /api/transactions/{id} (#18)")]
    public async Task DeleteTransaction_NotFound_Returns404()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: DELETE /api/transactions/{id} (#18)")]
    public async Task DeleteTransaction_BelongsToAnotherUser_Returns404()
    {
        throw new NotImplementedException();
    }
}
