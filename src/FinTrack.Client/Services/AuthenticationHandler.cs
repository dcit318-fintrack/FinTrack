using System.Net.Http.Headers;
using Microsoft.JSInterop;

namespace FinTrack.Client.Services;

public class AuthenticationHandler : DelegatingHandler
{
    private readonly IJSRuntime _js;

    public AuthenticationHandler(IJSRuntime js)
    {
        _js = js;
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

        return await base.SendAsync(request, cancellationToken);
    }
}
