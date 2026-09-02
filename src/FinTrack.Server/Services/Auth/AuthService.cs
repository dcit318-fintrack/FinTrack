using FinTrack.Server.Models;
using FinTrack.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Server.Services.Auth;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ILogger<AuthService> _logger;

    public AuthService(UserManager<ApplicationUser> userManager, IJwtTokenGenerator jwtTokenGenerator, ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _logger = logger;
    }

    public async Task<(bool success, AuthResponse? response, string? errorMessage, Dictionary<string, string[]>? errors)> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            _logger.LogWarning("Registration attempt with existing email: {Email}", request.Email);
            var errors = new Dictionary<string, string[]>
            {
                { "email", new[] { "A user with this email address already exists." } }
            };
            return (false, null, "Validation failed.", errors);
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
            _logger.LogWarning("User registration failed for {Email}: {Errors}", request.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
            var errors = result.Errors
                .GroupBy(e => e.Code)
                .ToDictionary(
                    g => g.Key.ToLower(),
                    g => g.Select(e => e.Description).ToArray()
                );
            return (false, null, "Validation failed.", errors);
        }

        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        _logger.LogInformation("User registered successfully: {Email}", request.Email);

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
            _logger.LogWarning("Login attempt with unknown email: {Email}", request.Email);
            return (false, null, "Invalid email or password.");
        }

        var isValidPassword = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isValidPassword)
        {
            _logger.LogWarning("Invalid password for user: {Email}", request.Email);
            return (false, null, "Invalid email or password.");
        }

        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        _logger.LogInformation("User logged in: {Email}", request.Email);

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

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);
        if (user == null || user.RefreshTokenExpiryTime == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            _logger.LogWarning("Invalid or expired refresh token used");
            return (false, null, "Invalid or expired refresh token.");
        }

        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user);
        var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        _logger.LogInformation("Refresh token rotated for user: {UserId}", user.Id);

        var response = new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            AccessToken = token,
            RefreshToken = newRefreshToken,
            ExpiresAt = expiresAt
        };

        return (true, response, null);
    }
}
