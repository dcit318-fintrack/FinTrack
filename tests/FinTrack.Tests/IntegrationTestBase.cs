// Shared test infrastructure for all FinTrack integration tests.
// Once FinTrack.Server has a real Program entry point, swap the comment below.

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

    protected IntegrationTestBase(WebApplicationFactory<Program> factory)
    {
        Client = factory.CreateClient();
    }

    // ---------------------------------------------------------------------------
    // Helpers — add to these as the API takes shape
    // ---------------------------------------------------------------------------

    /// <summary>
    /// Registers a fresh user and returns their access token.
    /// Call this at the start of any test that needs an authenticated client.
    /// </summary>
    protected async Task<string> RegisterAndGetTokenAsync(
        string email = "test@example.com",
        string password = "Password1!",
        string fullName = "Test User")
    {
        // TODO: implement once POST /api/auth/register exists
        // var response = await Client.PostAsJsonAsync("/api/auth/register", new { email, password, fullName });
        // response.EnsureSuccessStatusCode();
        // var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        // return body.GetProperty("accessToken").GetString()!;
        throw new NotImplementedException("Implement once /api/auth/register is live.");
    }

    /// <summary>
    /// Returns an HttpClient pre-configured with the given Bearer token.
    /// </summary>
    protected HttpClient AuthenticatedClient(string token)
    {
        var client = Client;
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
