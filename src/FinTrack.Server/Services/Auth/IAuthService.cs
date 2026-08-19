using FinTrack.Shared.DTOs.Auth;

namespace FinTrack.Server.Services.Auth;

public interface IAuthService
{
    Task<(bool success, AuthResponse? response, string? errorMessage, Dictionary<string, string[]>? errors)> RegisterAsync(RegisterRequest request);
    Task<(bool success, AuthResponse? response, string? errorMessage)> LoginAsync(LoginRequest request);
    Task<(bool success, AuthResponse? response, string? errorMessage)> RefreshTokenAsync(RefreshTokenRequest request);
}
