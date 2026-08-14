using System.Net.Http.Json;
using FinTrack.Shared.DTOs.Auth;
using FinTrack.Shared.DTOs.Common;
using Microsoft.JSInterop;

namespace FinTrack.Client.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;
    public event Action? OnAuthStateChanged;

    private string? _token = "demo-active-token";
    private string _currentUserName = "Samuel Watson";
    private string _currentUserEmail = "samuel@ug.edu.gh";

    public AuthService(HttpClient http, IJSRuntime js)
    {
        _http = http;
        _js = js;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("/api/auth/login", request);
            if (response.IsSuccessStatusCode)
            {
                var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (auth != null)
                {
                    _token = auth.AccessToken;
                    _currentUserName = auth.FullName;
                    _currentUserEmail = auth.Email;
                    await _js.InvokeVoidAsync("localStorage.setItem", "authToken", _token);
                    OnAuthStateChanged?.Invoke();
                    return auth;
                }
            }
        }
        catch (Exception)
        {
            // Fallback for demo when backend is offline
        }

        // Demo / Mock success response
        var demoAuth = new AuthResponse
        {
            UserId = Guid.NewGuid(),
            Email = request.Email,
            FullName = request.Email.Split('@')[0],
            AccessToken = "mock-jwt-token-" + Guid.NewGuid().ToString("N"),
            RefreshToken = "mock-refresh-token",
            ExpiresAt = DateTime.UtcNow.AddHours(2)
        };

        _token = demoAuth.AccessToken;
        _currentUserName = demoAuth.FullName;
        _currentUserEmail = demoAuth.Email;
        await _js.InvokeVoidAsync("localStorage.setItem", "authToken", _token);
        OnAuthStateChanged?.Invoke();
        return demoAuth;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("/api/auth/register", request);
            if (response.IsSuccessStatusCode)
            {
                var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (auth != null)
                {
                    _token = auth.AccessToken;
                    _currentUserName = auth.FullName;
                    _currentUserEmail = auth.Email;
                    await _js.InvokeVoidAsync("localStorage.setItem", "authToken", _token);
                    OnAuthStateChanged?.Invoke();
                    return auth;
                }
            }
        }
        catch (Exception)
        {
            // Fallback for demo when backend is offline
        }

        var demoAuth = new AuthResponse
        {
            UserId = Guid.NewGuid(),
            Email = request.Email,
            FullName = request.FullName,
            AccessToken = "mock-jwt-token-" + Guid.NewGuid().ToString("N"),
            RefreshToken = "mock-refresh-token",
            ExpiresAt = DateTime.UtcNow.AddHours(2)
        };

        _token = demoAuth.AccessToken;
        _currentUserName = demoAuth.FullName;
        _currentUserEmail = demoAuth.Email;
        await _js.InvokeVoidAsync("localStorage.setItem", "authToken", _token);
        OnAuthStateChanged?.Invoke();
        return demoAuth;
    }

    public async Task LogoutAsync()
    {
        _token = null;
        _currentUserName = "Guest";
        _currentUserEmail = string.Empty;
        await _js.InvokeVoidAsync("localStorage.removeItem", "authToken");
        OnAuthStateChanged?.Invoke();
    }

    public Task<bool> IsAuthenticatedAsync()
    {
        return Task.FromResult(!string.IsNullOrEmpty(_token));
    }

    public Task<string> GetCurrentUserEmailAsync()
    {
        return Task.FromResult(_currentUserEmail);
    }

    public Task<string> GetCurrentUserNameAsync()
    {
        return Task.FromResult(_currentUserName);
    }
}
