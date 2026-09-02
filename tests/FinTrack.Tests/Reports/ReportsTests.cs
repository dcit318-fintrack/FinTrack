using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FinTrack.Tests.Reports;

public class ReportsTests(WebApplicationFactory<Program> factory)
    : IntegrationTestBase(factory)
{
    [Fact]
    public async Task SpendingByCategory_Unauthenticated_Returns401()
    {
        var response = await Client.GetAsync("/api/reports/spending-by-category?from=2026-08-01&to=2026-08-31");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task SpendingByCategory_ValidDateRange_Returns200WithCorrectShape()
    {
        var token = await RegisterAndGetTokenAsync();
        using var client = AuthenticatedClient(token);

        var response = await client.GetAsync("/api/reports/spending-by-category?from=2026-08-01&to=2026-08-31");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(body.TryGetProperty("from", out _));
        Assert.True(body.TryGetProperty("to", out _));
        Assert.True(body.TryGetProperty("totalSpent", out _));
        Assert.True(body.TryGetProperty("categories", out _));
    }

    [Fact]
    public async Task SpendingByCategory_MissingDates_DefaultsToCurrentMonth()
    {
        var token = await RegisterAndGetTokenAsync();
        using var client = AuthenticatedClient(token);

        var response = await client.GetAsync("/api/reports/spending-by-category");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(body.TryGetProperty("from", out _));
        Assert.True(body.TryGetProperty("to", out _));
    }

    [Fact]
    public async Task SpendingByCategory_OnlyFromDate_Returns400()
    {
        var token = await RegisterAndGetTokenAsync();
        using var client = AuthenticatedClient(token);

        var response = await client.GetAsync("/api/reports/spending-by-category?from=2026-08-01");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task IncomeVsExpense_InvalidGranularity_Returns400()
    {
        var token = await RegisterAndGetTokenAsync();
        using var client = AuthenticatedClient(token);

        var response = await client.GetAsync("/api/reports/income-vs-expense?from=2026-01-01&to=2026-08-31&granularity=annual");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task IncomeVsExpense_ValidGranularity_Returns200()
    {
        var token = await RegisterAndGetTokenAsync();
        using var client = AuthenticatedClient(token);

        var response = await client.GetAsync("/api/reports/income-vs-expense?from=2026-01-01&to=2026-08-31&granularity=monthly");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("monthly", body.GetProperty("granularity").GetString());
        Assert.True(body.TryGetProperty("points", out _));
    }
}
