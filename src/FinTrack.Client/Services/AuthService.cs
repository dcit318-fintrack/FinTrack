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
        if (!string.IsNullOrWhiteSpace(auth.RefreshToken))
        {
            await _js.InvokeVoidAsync("localStorage.setItem", "refreshToken", auth.RefreshToken);
        }
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
        if (!string.IsNullOrWhiteSpace(auth.RefreshToken))
        {
            await _js.InvokeVoidAsync("localStorage.setItem", "refreshToken", auth.RefreshToken);
        }
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
        try
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", "authToken");
            await _js.InvokeVoidAsync("localStorage.removeItem", "refreshToken");
            await _js.InvokeVoidAsync("localStorage.removeItem", "authUserName");
            await _js.InvokeVoidAsync("localStorage.removeItem", "authUserEmail");
        }
        catch
        {
            // Ignore during teardown
        }
        OnAuthStateChanged?.Invoke();
    }

    public async Task<bool> RefreshTokenAsync()
    {
        try
        {
            var refreshToken = await _js.InvokeAsync<string?>("localStorage.getItem", "refreshToken");
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                await LogoutAsync();
                return false;
            }

            var response = await _http.PostAsJsonAsync("/api/auth/refresh", new RefreshTokenRequest { RefreshToken = refreshToken });
            if (!response.IsSuccessStatusCode)
            {
                await LogoutAsync();
                return false;
            }

            var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (auth == null || string.IsNullOrWhiteSpace(auth.AccessToken))
            {
                await LogoutAsync();
                return false;
            }

            _token = auth.AccessToken;
            _currentUserName = auth.FullName;
            _currentUserEmail = auth.Email;
            await _js.InvokeVoidAsync("localStorage.setItem", "authToken", _token);
            if (!string.IsNullOrWhiteSpace(auth.RefreshToken))
            {
                await _js.InvokeVoidAsync("localStorage.setItem", "refreshToken", auth.RefreshToken);
            }
            await _js.InvokeVoidAsync("localStorage.setItem", "authUserName", _currentUserName);
            await _js.InvokeVoidAsync("localStorage.setItem", "authUserEmail", _currentUserEmail);
            OnAuthStateChanged?.Invoke();
            return true;
        }
        catch
        {
            await LogoutAsync();
            return false;
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        if (string.IsNullOrEmpty(_token))
        {
            try
            {
                _token = await _js.InvokeAsync<string?>("localStorage.getItem", "authToken");
                _currentUserName = await _js.InvokeAsync<string?>("localStorage.getItem", "authUserName") ?? string.Empty;
                _currentUserEmail = await _js.InvokeAsync<string?>("localStorage.getItem", "authUserEmail") ?? string.Empty;
            }
            catch
            {
                return false;
            }
        }

        if (string.IsNullOrEmpty(_token))
        {
            return false;
        }

        // Check if JWT token has expired; if so, attempt automatic refresh
        if (IsTokenExpired(_token))
        {
            return await RefreshTokenAsync();
        }

        return true;
    }

    private static bool IsTokenExpired(string token)
    {
        try
        {
            var parts = token.Split('.');
            if (parts.Length != 3) return true;

            var payload = parts[1];
            payload = payload.Replace('-', '+').Replace('_', '/');
            switch (payload.Length % 4)
            {
                case 2: payload += "=="; break;
                case 3: payload += "="; break;
            }

            var jsonBytes = Convert.FromBase64String(payload);
            using var doc = System.Text.Json.JsonDocument.Parse(jsonBytes);
            if (doc.RootElement.TryGetProperty("exp", out var expProp) && expProp.TryGetInt64(out var expSeconds))
            {
                var expTime = DateTimeOffset.FromUnixTimeSeconds(expSeconds);
                // Consider expired 30 seconds before actual expiry to prevent edge-case 401s
                return expTime <= DateTimeOffset.UtcNow.AddSeconds(30);
            }
            return false;
        }
        catch
        {
            return true;
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

    private static string ExtractErrorMessage(string? errorBody, int statusCode, string fallback)
    {
        if (string.IsNullOrWhiteSpace(errorBody))
        {
            return $"{fallback} (Status {statusCode})";
        }

        try
        {
            using var doc = System.Text.Json.JsonDocument.Parse(errorBody);
            var root = doc.RootElement;

            // 1. Check if there are detailed validation errors in "errors" property
            if (root.TryGetProperty("errors", out var errorsProp))
            {
                if (errorsProp.ValueKind == System.Text.Json.JsonValueKind.Object)
                {
                    var errorList = new List<string>();
                    foreach (var prop in errorsProp.EnumerateObject())
                    {
                        if (prop.Value.ValueKind == System.Text.Json.JsonValueKind.Array)
                        {
                            foreach (var item in prop.Value.EnumerateArray())
                            {
                                var s = item.GetString();
                                if (!string.IsNullOrWhiteSpace(s))
                                {
                                    errorList.Add(s);
                                }
                            }
                        }
                        else if (prop.Value.ValueKind == System.Text.Json.JsonValueKind.String)
                        {
                            var s = prop.Value.GetString();
                            if (!string.IsNullOrWhiteSpace(s))
                            {
                                errorList.Add(s);
                            }
                        }
                    }
                    if (errorList.Count > 0)
                    {
                        return string.Join(" ", errorList);
                    }
                }
                else if (errorsProp.ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    var errorList = new List<string>();
                    foreach (var item in errorsProp.EnumerateArray())
                    {
                        var s = item.GetString();
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            errorList.Add(s);
                        }
                    }
                    if (errorList.Count > 0)
                    {
                        return string.Join(" ", errorList);
                    }
                }
            }

            // 2. Check for "message" property
            if (root.TryGetProperty("message", out var messageProp) && messageProp.ValueKind == System.Text.Json.JsonValueKind.String)
            {
                var msg = messageProp.GetString();
                if (!string.IsNullOrWhiteSpace(msg))
                {
                    return msg;
                }
            }

            // 3. Check for "title" property (ProblemDetails)
            if (root.TryGetProperty("title", out var titleProp) && titleProp.ValueKind == System.Text.Json.JsonValueKind.String)
            {
                var title = titleProp.GetString();
                if (!string.IsNullOrWhiteSpace(title))
                {
                    return title;
                }
            }
        }
        catch
        {
            if (errorBody.Length < 200 && !errorBody.Contains('<'))
            {
                return errorBody;
            }
        }

        return $"{fallback} (Status {statusCode})";
    }
}

