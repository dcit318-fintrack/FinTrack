using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FinTrack.Tests.SavingsGoals;

public class SavingsGoalTests(WebApplicationFactory<Program> factory)
    : IntegrationTestBase(factory)
{
    [Fact]
    public async Task CreateGoal_ValidData_Returns201WithAllFields()
    {
        var token = await RegisterAndGetTokenAsync();
        using var client = AuthenticatedClient(token);

        var response = await client.PostAsJsonAsync("/api/savings-goals", new
        {
            name = "Emergency Fund",
            targetAmount = 10000m,
            targetDate = DateTime.UtcNow.AddMonths(6)
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Emergency Fund", body.GetProperty("name").GetString());
        Assert.Equal(0m, body.GetProperty("currentAmount").GetDecimal());
        Assert.False(body.GetProperty("isAchieved").GetBoolean());
    }

    [Fact]
    public async Task CreateGoal_TargetDateInPast_Returns400()
    {
        var token = await RegisterAndGetTokenAsync();
        using var client = AuthenticatedClient(token);

        var response = await client.PostAsJsonAsync("/api/savings-goals", new
        {
            name = "Past Goal",
            targetAmount = 1000m,
            targetDate = DateTime.UtcNow.AddDays(-1)
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetGoals_Returns200WithArray()
    {
        var token = await RegisterAndGetTokenAsync();
        using var client = AuthenticatedClient(token);

        var response = await client.GetAsync("/api/savings-goals");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(JsonValueKind.Array, body.ValueKind);
    }

    [Fact]
    public async Task DeleteGoal_NotFound_Returns404()
    {
        var token = await RegisterAndGetTokenAsync();
        using var client = AuthenticatedClient(token);

        var response = await client.DeleteAsync($"/api/savings-goals/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
