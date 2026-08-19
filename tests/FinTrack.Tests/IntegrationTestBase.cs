using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FinTrack.Tests;

public abstract class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly HttpClient Client;
    private readonly WebApplicationFactory<Program> _factory;

    protected IntegrationTestBase(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        Client = factory.CreateClient();
    }

    protected async Task<string> RegisterAndGetTokenAsync(
        string email = "test@example.com",
        string password = "Password1!",
        string fullName = "Test User")
    {
        var response = await Client.PostAsJsonAsync("/api/auth/register", new
        {
            email,
            password,
            fullName
        });

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("accessToken").GetString()!;
    }

    protected HttpClient AuthenticatedClient(string token)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
