// Shared test infrastructure for all FinTrack integration tests.

using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FinTrack.Tests;

/// <summary>
/// Base class for all integration tests.
/// Spins up the FinTrack.Server in-process using WebApplicationFactory.
/// Each test class inherits this and gets a ready-made HttpClient.
/// </summary>
public abstract class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly HttpClient Client;
    protected readonly WebApplicationFactory<Program> Factory;

    protected IntegrationTestBase(WebApplicationFactory<Program> factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    /// <summary>
    /// Registers a fresh user and returns their access token.
    /// Call this at the start of any test that needs an authenticated client.
    /// </summary>
    protected async Task<string> RegisterAndGetTokenAsync(
        string email = "test@example.com",
        string password = "Password1!",
        string fullName = "Test User")
    {
        var uniqueEmail = email == "test@example.com" ? $"test_{Guid.NewGuid()}@example.com" : email;
        var response = await Client.PostAsJsonAsync("/api/auth/register", new { email = uniqueEmail, password, fullName });
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Register failed: {response.StatusCode} - {err}");
        }
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("accessToken").GetString()!;
    }

    /// <summary>
    /// Returns an HttpClient pre-configured with the given Bearer token.
    /// </summary>
    protected HttpClient AuthenticatedClient(string token)
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
