using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FinTrack.Tests.Dashboard;

public class DashboardTests(WebApplicationFactory<Program> factory)
    : IntegrationTestBase(factory)
{
    [Fact]
    public async Task GetDashboard_Authenticated_Returns200WithAllExpectedFields()
    {
        var token = await RegisterAndGetTokenAsync();
        using var client = AuthenticatedClient(token);

        var response = await client.GetAsync("/api/dashboard");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(body.TryGetProperty("totalIncome", out _));
        Assert.True(body.TryGetProperty("totalExpenses", out _));
        Assert.True(body.TryGetProperty("balance", out _));
        Assert.True(body.TryGetProperty("month", out _));
        Assert.True(body.TryGetProperty("recentTransactions", out _));
        Assert.True(body.TryGetProperty("budgetsAtRisk", out _));
    }

    [Fact]
    public async Task GetDashboard_Unauthenticated_Returns401()
    {
        var response = await Client.GetAsync("/api/dashboard");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
