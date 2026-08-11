using FinTrack.Server.Models;
using FinTrack.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Identity;

namespace FinTrack.Server.Services.Auth;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<(bool success, AuthResponse? response, string? errorMessage, Dictionary<string, string[]>? errors)> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            var fieldErrors = new Dictionary<string, string[]>
            {
                { "email", new[] { "An account with this email address already exists." } }
            };
            return (false, null, "User registration failed.", fieldErrors);
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors
                .GroupBy(e => e.Code)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

            return (false, null, "User creation failed.", errors);
        }

        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        var response = new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            AccessToken = token,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt
        };

        return (true, response, null, null);
    }

    public async Task<(bool success, AuthResponse? response, string? errorMessage)> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return (false, null, "Invalid email or password.");
        }

        var isValidPassword = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isValidPassword)
        {
            return (false, null, "Invalid email or password.");
        }

        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        var response = new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            AccessToken = token,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt
        };

        return (true, response, null);
    }

    public async Task<(bool success, AuthResponse? response, string? errorMessage)> RefreshTokenAsync(RefreshTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return (false, null, "Invalid refresh token.");
        }

        // For v1 demo: returns simulated token refresh acknowledgement
        return (false, null, "Token refresh mechanism requires persistent refresh token.");
    }
}