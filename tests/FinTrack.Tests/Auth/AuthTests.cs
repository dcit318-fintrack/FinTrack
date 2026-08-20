using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FinTrack.Tests.Auth;

public class AuthTests(WebApplicationFactory<Program> factory)
    : IntegrationTestBase(factory)
{
    [Fact]
    public async Task Register_WithValidData_Returns201AndTokens()
    {
        var payload = new
        {
            email = $"user_{Guid.NewGuid()}@example.com",
            password = "Password1!",
            fullName = "Test User"
        };

        var response = await Client.PostAsJsonAsync("/api/auth/register", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Assert.True(body.TryGetProperty("userId", out _));
        Assert.True(body.TryGetProperty("accessToken", out _));
        Assert.True(body.TryGetProperty("refreshToken", out _));
        Assert.True(body.TryGetProperty("expiresAt", out _));
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_Returns400()
    {
        var email = $"dup_{Guid.NewGuid()}@example.com";
        var payload = new { email, password = "Password1!", fullName = "Test User" };

        await Client.PostAsJsonAsync("/api/auth/register", payload);
        var response = await Client.PostAsJsonAsync("/api/auth/register", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithWeakPassword_Returns400()
    {
        var payload = new
        {
            email = $"user_{Guid.NewGuid()}@example.com",
            password = "weak",
            fullName = "Test User"
        };

        var response = await Client.PostAsJsonAsync("/api/auth/register", payload);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithValidCredentials_Returns200AndTokens()
    {
        var email = $"user_{Guid.NewGuid()}@example.com";
        await Client.PostAsJsonAsync("/api/auth/register",
            new { email, password = "Password1!", fullName = "Test User" });

        var response = await Client.PostAsJsonAsync("/api/auth/login",
            new { email, password = "Password1!" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Assert.True(body.TryGetProperty("accessToken", out _));
        Assert.True(body.TryGetProperty("refreshToken", out _));
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns401WithGenericMessage()
    {
        var email = $"user_{Guid.NewGuid()}@example.com";
        await Client.PostAsJsonAsync("/api/auth/register",
            new { email, password = "Password1!", fullName = "Test User" });

        var response = await Client.PostAsJsonAsync("/api/auth/login",
            new { email, password = "WrongPassword!" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Assert.Equal("Invalid email or password", body.GetProperty("message").GetString());
    }

    [Fact]
    public async Task Login_WithUnknownEmail_Returns401()
    {
        var response = await Client.PostAsJsonAsync("/api/auth/login",
            new { email = "nobody@example.com", password = "Password1!" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithNoToken_Returns401()
    {
        var response = await Client.GetAsync("/api/transactions");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithMalformedToken_Returns401()
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "notavalidjwt");
        var response = await client.GetAsync("/api/transactions");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_WithInvalidToken_Returns400()
    {
        var response = await Client.PostAsJsonAsync("/api/auth/refresh",
            new { refreshToken = "invalid-token" });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
