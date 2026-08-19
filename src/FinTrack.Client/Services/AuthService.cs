using System.Net.Http.Json;
using FinTrack.Shared.DTOs.Auth;
using Microsoft.JSInterop;

namespace FinTrack.Client.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;
    public event Action? OnAuthStateChanged;

    private string? _token;
    private string _currentUserName = "Samuel Watson";
    private string _currentUserEmail = "samuel@ug.edu.gh";

    public AuthService(HttpClient http, IJSRuntime js)
    {
        _http = http;
        _js = js;
    }

    public async Task<string?> GetTokenAsync()
    {
        if (string.IsNullOrEmpty(_token))
        {
            try
            {
                _token = await _js.InvokeAsync<string?>("localStorage.getItem", "authToken");
            }
            catch
            {
                // Ignored in prerendering
            }

            if (string.IsNullOrEmpty(_token))
            {
                // Auto-authenticate with the seeded backend demo user
                try
                {
                    var demoLogin = await _http.PostAsJsonAsync("api/auth/login", new LoginRequest
                    {
                        Email = "samuel@ug.edu.gh",
                        Password = "Password123!"
                    });
                    if (demoLogin.IsSuccessStatusCode)
                    {
                        var auth = await demoLogin.Content.ReadFromJsonAsync<AuthResponse>();
                        if (auth != null)
                        {
                            _token = auth.AccessToken;
                            _currentUserName = auth.FullName;
                            _currentUserEmail = auth.Email;
                            try { await _js.InvokeVoidAsync("localStorage.setItem", "authToken", _token); } catch { }
                        }
                    }
                }
                catch
                {
                    // Server starting up
                }
            }
        }
        return _token;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", request);
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

        var errorText = await response.Content.ReadAsStringAsync();
        throw new InvalidOperationException(!string.IsNullOrWhiteSpace(errorText) ? errorText : "Invalid email or password");
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/auth/register", request);
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

        var errorText = await response.Content.ReadAsStringAsync();
        throw new InvalidOperationException(!string.IsNullOrWhiteSpace(errorText) ? errorText : "Registration failed.");
    }

    public async Task LogoutAsync()
    {
        _token = null;
        _currentUserName = "Guest";
        _currentUserEmail = string.Empty;
        await _js.InvokeVoidAsync("localStorage.removeItem", "authToken");
        OnAuthStateChanged?.Invoke();
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
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

