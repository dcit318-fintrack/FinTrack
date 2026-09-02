using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FinTrack.Tests.Transactions;

public class TransactionTests(WebApplicationFactory<Program> factory)
    : IntegrationTestBase(factory)
{
    private const string FoodCategoryId = "11111111-1111-1111-1111-111111111111";
    private const string SalaryCategoryId = "66666666-6666-6666-6666-666666666666";

    [Fact]
    public async Task CreateTransaction_Expense_Returns201WithCategoryName()
    {
        var token = await RegisterAndGetTokenAsync();
        using var client = AuthenticatedClient(token);

        var response = await client.PostAsJsonAsync("/api/transactions", new
        {
            amount = 50.00m,
            type = "Expense",
            categoryId = FoodCategoryId,
            description = "Lunch",
            date = DateTime.UtcNow
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Food", body.GetProperty("categoryName").GetString());
    }

    [Fact]
    public async Task CreateTransaction_Income_Returns201()
    {
        var token = await RegisterAndGetTokenAsync();
        using var client = AuthenticatedClient(token);

        var response = await client.PostAsJsonAsync("/api/transactions", new
        {
            amount = 3000m,
            type = "Income",
            categoryId = SalaryCategoryId,
            description = "Monthly salary",
            date = DateTime.UtcNow
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task CreateTransaction_AmountNotPositive_Returns400(decimal amount)
    {
        var token = await RegisterAndGetTokenAsync();
        using var client = AuthenticatedClient(token);

        var response = await client.PostAsJsonAsync("/api/transactions", new
        {
            amount,
            type = "Expense",
            categoryId = FoodCategoryId,
            description = "Test",
            date = DateTime.UtcNow
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTransaction_NonExistentCategoryId_Returns400()
    {
        var token = await RegisterAndGetTokenAsync();
        using var client = AuthenticatedClient(token);

        var response = await client.PostAsJsonAsync("/api/transactions", new
        {
            amount = 50m,
            type = "Expense",
            categoryId = Guid.NewGuid(),
            description = "Test",
            date = DateTime.UtcNow
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTransaction_DateInFuture_Returns400()
    {
        var token = await RegisterAndGetTokenAsync();
        using var client = AuthenticatedClient(token);

        var response = await client.PostAsJsonAsync("/api/transactions", new
        {
            amount = 50m,
            type = "Expense",
            categoryId = FoodCategoryId,
            description = "Test",
            date = DateTime.UtcNow.AddDays(1)
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetTransactions_DefaultPagination_ReturnsCorrectShape()
    {
        var token = await RegisterAndGetTokenAsync();
        using var client = AuthenticatedClient(token);

        var response = await client.GetAsync("/api/transactions");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(body.TryGetProperty("page", out _));
        Assert.True(body.TryGetProperty("pageSize", out _));
        Assert.True(body.TryGetProperty("totalCount", out _));
        Assert.True(body.TryGetProperty("items", out _));
    }

    [Fact]
    public async Task GetTransaction_NotFound_Returns404()
    {
        var token = await RegisterAndGetTokenAsync();
        using var client = AuthenticatedClient(token);

        var response = await client.GetAsync($"/api/transactions/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTransaction_NotFound_Returns404()
    {
        var token = await RegisterAndGetTokenAsync();
        using var client = AuthenticatedClient(token);

        var response = await client.DeleteAsync($"/api/transactions/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetDashboard_AuthenticatedUser_Returns200()
    {
        var token = await RegisterAndGetTokenAsync();
        using var client = AuthenticatedClient(token);

        var response = await client.GetAsync("/api/dashboard");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(body.TryGetProperty("totalIncome", out _));
        Assert.True(body.TryGetProperty("totalExpenses", out _));
        Assert.True(body.TryGetProperty("balance", out _));
    }
}
