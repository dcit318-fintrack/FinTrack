// Auth tests for POST /api/auth/register, /login, /refresh
// Activate tests (remove Skip) once the auth endpoints are implemented (#17).

using Microsoft.AspNetCore.Mvc.Testing;

namespace FinTrack.Tests.Auth;

public class AuthTests(WebApplicationFactory<Program> factory)
    : IntegrationTestBase(factory)
{
    // -------------------------------------------------------------------------
    // Register
    // -------------------------------------------------------------------------

    [Fact(Skip = "Pending: POST /api/auth/register (#17)")]
    public async Task Register_WithValidData_Returns201AndTokens()
    {
        // Arrange
        var payload = new
        {
            email = $"user_{Guid.NewGuid()}@example.com",
            password = "Password1!",
            fullName = "Test User"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", payload);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Assert.True(body.TryGetProperty("userId", out _), "Response should contain userId");
        Assert.True(body.TryGetProperty("accessToken", out _), "Response should contain accessToken");
        Assert.True(body.TryGetProperty("refreshToken", out _), "Response should contain refreshToken");
        Assert.True(body.TryGetProperty("expiresAt", out _), "Response should contain expiresAt");
    }

    [Fact(Skip = "Pending: POST /api/auth/register (#17)")]
    public async Task Register_WithDuplicateEmail_ReturnsErrorStatus()
    {
        var email = $"dup_{Guid.NewGuid()}@example.com";
        var payload = new { email, password = "Password1!", fullName = "Test User" };

        await Client.PostAsJsonAsync("/api/auth/register", payload);
        var response = await Client.PostAsJsonAsync("/api/auth/register", payload);

        // Contract says 400 or 409 — confirm exact code with Backend on Day 1
        Assert.True(
            response.StatusCode == System.Net.HttpStatusCode.BadRequest ||
            response.StatusCode == System.Net.HttpStatusCode.Conflict,
            $"Expected 400 or 409 for duplicate email, got {(int)response.StatusCode}");
    }

    [Theory(Skip = "Pending: POST /api/auth/register (#17)")]
    [InlineData(null, "Password1!", "Full Name")]  // missing email
    [InlineData("a@b.com", null, "Full Name")]       // missing password
    [InlineData("a@b.com", "Password1!", null)]      // missing fullName
    public async Task Register_WithMissingRequiredField_Returns400(
        string? email, string? password, string? fullName)
    {
        var payload = new { email, password, fullName };
        var response = await Client.PostAsJsonAsync("/api/auth/register", payload);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(Skip = "Pending: POST /api/auth/register (#17)")]
    public async Task Register_WithInvalidEmailFormat_Returns400()
    {
        var payload = new { email = "not-an-email", password = "Password1!", fullName = "Test User" };
        var response = await Client.PostAsJsonAsync("/api/auth/register", payload);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    // -------------------------------------------------------------------------
    // Login
    // -------------------------------------------------------------------------

    [Fact(Skip = "Pending: POST /api/auth/login (#17)")]
    public async Task Login_WithValidCredentials_Returns200AndTokens()
    {
        var email = $"user_{Guid.NewGuid()}@example.com";
        await Client.PostAsJsonAsync("/api/auth/register",
            new { email, password = "Password1!", fullName = "Test User" });

        var response = await Client.PostAsJsonAsync("/api/auth/login",
            new { email, password = "Password1!" });

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Assert.True(body.TryGetProperty("accessToken", out _));
    }

    [Fact(Skip = "Pending: POST /api/auth/login (#17)")]
    public async Task Login_WithWrongPassword_Returns401WithGenericMessage()
    {
        var email = $"user_{Guid.NewGuid()}@example.com";
        await Client.PostAsJsonAsync("/api/auth/register",
            new { email, password = "Password1!", fullName = "Test User" });

        var response = await Client.PostAsJsonAsync("/api/auth/login",
            new { email, password = "WrongPassword!" });

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        var message = body.GetProperty("message").GetString();
        // Contract requirement: must not say which field was wrong
        Assert.Equal("Invalid email or password", message);
    }

    [Fact(Skip = "Pending: POST /api/auth/login (#17)")]
    public async Task Login_WithUnknownEmail_Returns401WithSameGenericMessage()
    {
        var response = await Client.PostAsJsonAsync("/api/auth/login",
            new { email = "nobody@example.com", password = "Password1!" });

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Assert.Equal("Invalid email or password", body.GetProperty("message").GetString());
    }

    // -------------------------------------------------------------------------
    // Token protection
    // -------------------------------------------------------------------------

    [Fact(Skip = "Pending: auth middleware (#17)")]
    public async Task ProtectedEndpoint_WithNoToken_Returns401()
    {
        var response = await Client.GetAsync("/api/transactions");
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(Skip = "Pending: auth middleware (#17)")]
    public async Task ProtectedEndpoint_WithMalformedToken_Returns401()
    {
        Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "notavalidjwt");
        var response = await Client.GetAsync("/api/transactions");
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // -------------------------------------------------------------------------
    // Refresh token
    // -------------------------------------------------------------------------

    [Fact(Skip = "Pending: POST /api/auth/refresh (#17)")]
    public async Task RefreshToken_WithValidToken_Returns200WithNewTokens()
    {
        // Register → get refreshToken → call /api/auth/refresh
        // Assert new accessToken, refreshToken, expiresAt returned
        throw new NotImplementedException();
    }

    [Fact(Skip = "Pending: POST /api/auth/refresh (#17)")]
    public async Task RefreshToken_WithInvalidToken_Returns401()
    {
        var response = await Client.PostAsJsonAsync("/api/auth/refresh",
            new { refreshToken = "invalid-token" });
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
