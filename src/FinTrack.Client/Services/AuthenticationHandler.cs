using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FinTrack.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FinTrack.Client.Services;

public class AuthenticationHandler : DelegatingHandler
{
    private readonly IJSRuntime _js;
    private readonly NavigationManager _nav;
    private static readonly SemaphoreSlim _refreshLock = new(1, 1);

    public AuthenticationHandler(IJSRuntime js, NavigationManager nav)
    {
        _js = js;
        _nav = nav;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            var token = await _js.InvokeAsync<string?>("localStorage.getItem", "authToken");
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
        catch
        {
            // Ignore during early startup or if JavaScript interop is not ready
        }

        var response = await base.SendAsync(request, cancellationToken);

        var path = request.RequestUri?.AbsolutePath ?? string.Empty;
        var isAuthEndpoint = path.Contains("/api/auth/login", StringComparison.OrdinalIgnoreCase) ||
                             path.Contains("/api/auth/register", StringComparison.OrdinalIgnoreCase) ||
                             path.Contains("/api/auth/refresh", StringComparison.OrdinalIgnoreCase);

        if (response.StatusCode == HttpStatusCode.Unauthorized && !isAuthEndpoint)
        {
            await _refreshLock.WaitAsync(cancellationToken);
            try
            {
                var refreshToken = await _js.InvokeAsync<string?>("localStorage.getItem", "refreshToken");
                if (!string.IsNullOrWhiteSpace(refreshToken))
                {
                    var baseAddress = request.RequestUri != null
                        ? new Uri(request.RequestUri.GetLeftPart(UriPartial.Authority))
                        : new Uri(_nav.BaseUri);
                    var refreshUri = new Uri(baseAddress, "api/auth/refresh");

                    var refreshPayload = new RefreshTokenRequest { RefreshToken = refreshToken };
                    using var refreshMsg = new HttpRequestMessage(HttpMethod.Post, refreshUri)
                    {
                        Content = JsonContent.Create(refreshPayload)
                    };

                    var refreshResponse = await base.SendAsync(refreshMsg, cancellationToken);
                    if (refreshResponse.IsSuccessStatusCode)
                    {
                        var auth = await refreshResponse.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: cancellationToken);
                        if (auth != null && !string.IsNullOrWhiteSpace(auth.AccessToken))
                        {
                            await _js.InvokeVoidAsync("localStorage.setItem", "authToken", auth.AccessToken);
                            if (!string.IsNullOrWhiteSpace(auth.RefreshToken))
                            {
                                await _js.InvokeVoidAsync("localStorage.setItem", "refreshToken", auth.RefreshToken);
                            }
                            if (!string.IsNullOrWhiteSpace(auth.FullName))
                            {
                                await _js.InvokeVoidAsync("localStorage.setItem", "authUserName", auth.FullName);
                            }

                            // Clone and retry original request with fresh token
                            using var retryRequest = await CloneHttpRequestMessageAsync(request);
                            retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
                            return await base.SendAsync(retryRequest, cancellationToken);
                        }
                    }
                }

                // If refresh failed or was not available, clear auth and redirect to login
                await _js.InvokeVoidAsync("localStorage.removeItem", "authToken");
                await _js.InvokeVoidAsync("localStorage.removeItem", "refreshToken");
                await _js.InvokeVoidAsync("localStorage.removeItem", "authUserName");
                await _js.InvokeVoidAsync("localStorage.removeItem", "authUserEmail");
                _nav.NavigateTo("login");
            }
            catch
            {
                // Ignore refresh errors and return original 401 response
            }
            finally
            {
                _refreshLock.Release();
            }
        }

        return response;
    }

    private static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage req)
    {
        var clone = new HttpRequestMessage(req.Method, req.RequestUri);
        if (req.Content != null)
        {
            var ms = new MemoryStream();
            await req.Content.CopyToAsync(ms);
            ms.Position = 0;
            clone.Content = new StreamContent(ms);
            foreach (var h in req.Content.Headers)
            {
                clone.Content.Headers.TryAddWithoutValidation(h.Key, h.Value);
            }
        }
        foreach (var header in req.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
        return clone;
    }
}
