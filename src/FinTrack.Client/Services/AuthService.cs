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

    private string? _token;
    private string _currentUserName = string.Empty;
    private string _currentUserEmail = string.Empty;

    public AuthService(HttpClient http, IJSRuntime js)
    {
        _http = http;
        _js = js;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var response = await _http.PostAsJsonAsync("/api/auth/login", request);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Login failed ({(int)response.StatusCode}): {errorBody}");
        }

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>()
            ?? throw new InvalidOperationException("Received an empty response from the login endpoint.");

        _token = auth.AccessToken;
        _currentUserName = auth.FullName;
        _currentUserEmail = auth.Email;
        await _js.InvokeVoidAsync("localStorage.setItem", "authToken", _token);
        OnAuthStateChanged?.Invoke();
        return auth;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var response = await _http.PostAsJsonAsync("/api/auth/register", request);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Registration failed ({(int)response.StatusCode}): {errorBody}");
        }

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>()
            ?? throw new InvalidOperationException("Received an empty response from the register endpoint.");

        _token = auth.AccessToken;
        _currentUserName = auth.FullName;
        _currentUserEmail = auth.Email;
        await _js.InvokeVoidAsync("localStorage.setItem", "authToken", _token);
        OnAuthStateChanged?.Invoke();
        return auth;
    }

    public async Task LogoutAsync()
    {
        _token = null;
        _currentUserName = string.Empty;
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
