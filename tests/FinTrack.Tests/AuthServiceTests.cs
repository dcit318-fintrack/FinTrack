using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FinTrack.Server.Models;
using FinTrack.Server.Services.Auth;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace FinTrack.Tests;

public class AuthServiceTests
{
    private readonly IConfiguration _config;
    private readonly JwtTokenGenerator _jwtTokenGenerator;

    public AuthServiceTests()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"Jwt:Secret", "Test_Super_Secret_Key_For_Jwt_Token_Generation_2026_Must_Be_Long_Enough!"},
            {"Jwt:Issuer", "FinTrackTestServer"},
            {"Jwt:Audience", "FinTrackTestClient"},
            {"Jwt:ExpiryInMinutes", "30"}
        };

        _config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _jwtTokenGenerator = new JwtTokenGenerator(_config);
    }

    [Fact]
    public void GenerateToken_ShouldReturnValidJwtTokenWithCorrectClaims()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = "student@example.com",
            FullName = "Student User"
        };

        // Act
        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
        Assert.True(expiresAt > DateTime.UtcNow);

        // Parse token and assert claims
        var handler = new JwtSecurityTokenHandler();
        Assert.True(handler.CanReadToken(token));

        var jwtToken = handler.ReadJwtToken(token);
        Assert.Equal("FinTrackTestServer", jwtToken.Issuer);
        Assert.Contains("FinTrackTestClient", jwtToken.Audiences);

        var subClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub || c.Type == "sub");
        Assert.NotNull(subClaim);
        Assert.Equal(user.Id.ToString(), subClaim.Value);

        var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email || c.Type == "email");
        Assert.NotNull(emailClaim);
        Assert.Equal(user.Email, emailClaim.Value);
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnRandomBase64String()
    {
        // Act
        var token1 = _jwtTokenGenerator.GenerateRefreshToken();
        var token2 = _jwtTokenGenerator.GenerateRefreshToken();

        // Assert
        Assert.NotNull(token1);
        Assert.NotEmpty(token1);
        Assert.NotNull(token2);
        Assert.NotEmpty(token2);
        Assert.NotEqual(token1, token2);

        // Verify base64 valid
        var bytes = Convert.FromBase64String(token1);
        Assert.Equal(32, bytes.Length);
    }
}
