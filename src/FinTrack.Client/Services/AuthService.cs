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
            var message = FinTrackApiService.ExtractErrorMessage(errorBody, response.StatusCode, response.ReasonPhrase);
            throw new InvalidOperationException(message);
        }

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>()
            ?? throw new InvalidOperationException("Received an empty response from the login endpoint.");

        _token = auth.AccessToken;
        _currentUserName = auth.FullName;
        _currentUserEmail = auth.Email;
        await _js.InvokeVoidAsync("localStorage.setItem", "authToken", _token);
        await _js.InvokeVoidAsync("localStorage.setItem", "authUserName", _currentUserName);
        await _js.InvokeVoidAsync("localStorage.setItem", "authUserEmail", _currentUserEmail);
        OnAuthStateChanged?.Invoke();
        return auth;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var response = await _http.PostAsJsonAsync("/api/auth/register", request);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            var message = FinTrackApiService.ExtractErrorMessage(errorBody, response.StatusCode, response.ReasonPhrase);
            throw new InvalidOperationException(message);
        }

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>()
            ?? throw new InvalidOperationException("Received an empty response from the register endpoint.");

        _token = auth.AccessToken;
        _currentUserName = auth.FullName;
        _currentUserEmail = auth.Email;
        await _js.InvokeVoidAsync("localStorage.setItem", "authToken", _token);
        await _js.InvokeVoidAsync("localStorage.setItem", "authUserName", _currentUserName);
        await _js.InvokeVoidAsync("localStorage.setItem", "authUserEmail", _currentUserEmail);
        OnAuthStateChanged?.Invoke();
        return auth;
    }

    public async Task LogoutAsync()
    {
        _token = null;
        _currentUserName = string.Empty;
        _currentUserEmail = string.Empty;
        await _js.InvokeVoidAsync("localStorage.removeItem", "authToken");
        await _js.InvokeVoidAsync("localStorage.removeItem", "authUserName");
        await _js.InvokeVoidAsync("localStorage.removeItem", "authUserEmail");
        OnAuthStateChanged?.Invoke();
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        if (!string.IsNullOrEmpty(_token))
        {
            return true;
        }

        try
        {
            _token = await _js.InvokeAsync<string?>("localStorage.getItem", "authToken");
            _currentUserName = await _js.InvokeAsync<string?>("localStorage.getItem", "authUserName") ?? string.Empty;
            _currentUserEmail = await _js.InvokeAsync<string?>("localStorage.getItem", "authUserEmail") ?? string.Empty;
            return !string.IsNullOrEmpty(_token);
        }
        catch
        {
            return false;
        }
    }

    public async Task<string> GetCurrentUserEmailAsync()
    {
        if (string.IsNullOrEmpty(_currentUserEmail))
        {
            await IsAuthenticatedAsync();
        }
        return _currentUserEmail;
    }

    public async Task<string> GetCurrentUserNameAsync()
    {
        if (string.IsNullOrEmpty(_currentUserName))
        {
            await IsAuthenticatedAsync();
        }
        return _currentUserName;
    }
}
