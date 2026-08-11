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
    public void GenerateToken_ShouldReturnValidJwtToken()
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
    }
}
