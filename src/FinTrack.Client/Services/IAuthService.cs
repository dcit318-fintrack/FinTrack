using FinTrack.Shared.DTOs.Auth;

namespace FinTrack.Client.Services;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<string> GetCurrentUserEmailAsync();
    Task<string> GetCurrentUserNameAsync();
    event Action? OnAuthStateChanged;
}
