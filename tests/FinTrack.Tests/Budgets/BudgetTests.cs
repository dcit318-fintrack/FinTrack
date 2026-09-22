using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FinTrack.Tests.Budgets;

public class BudgetTests(WebApplicationFactory<Program> factory)
    : IntegrationTestBase(factory)
{
    private const string FoodCategoryId = "11111111-1111-1111-1111-111111111111";

    [Fact]
    public async Task CreateBudget_ValidData_Returns201()
    {
        var token = await RegisterAndGetTokenAsync();
        using var client = AuthenticatedClient(token);

        var response = await client.PostAsJsonAsync("/api/budgets", new
        {
            categoryId = FoodCategoryId,
            limit = 500.00m,
            month = "2026-08"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateBudget_DuplicateCategoryAndMonth_Returns409()
    {
        var token = await RegisterAndGetTokenAsync();
        using var client = AuthenticatedClient(token);

        var payload = new { categoryId = FoodCategoryId, limit = 500.00m, month = "2026-08" };
        await client.PostAsJsonAsync("/api/budgets", payload);
        var response = await client.PostAsJsonAsync("/api/budgets", payload);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task GetBudgets_DefaultsToCurrentMonth_ReturnsCorrectShape()
    {
        var token = await RegisterAndGetTokenAsync();
        using var client = AuthenticatedClient(token);

        var response = await client.GetAsync("/api/budgets");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(JsonValueKind.Array, body.ValueKind);
    }

    [Fact]
    public async Task DeleteBudget_NotFound_Returns404()
    {
        var token = await RegisterAndGetTokenAsync();
        using var client = AuthenticatedClient(token);

        var response = await client.DeleteAsync($"/api/budgets/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
