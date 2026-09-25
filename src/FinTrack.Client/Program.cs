using System.Globalization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FinTrack.Client;
using FinTrack.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
culture.NumberFormat.CurrencySymbol = "GH₵";
culture.NumberFormat.CurrencyPositivePattern = 0;
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

builder.Services.AddTransient<AuthenticationHandler>();
builder.Services.AddScoped(sp =>
{
    var authHandler = sp.GetRequiredService<AuthenticationHandler>();
    authHandler.InnerHandler = new HttpClientHandler();
    var http = new HttpClient(authHandler);
    var apiUrl = builder.Configuration["ApiBaseUrl"];
    var isHttpsHost = builder.HostEnvironment.BaseAddress.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

    if (string.IsNullOrWhiteSpace(apiUrl) || apiUrl == "/" || (isHttpsHost && apiUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase)))
    {
        // On HTTPS hosts (like Netlify), use same-origin BaseAddress to route through the secure proxy
        http.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    }
    else
    {
        http.BaseAddress = Uri.TryCreate(apiUrl, UriKind.Absolute, out var absUri)
            ? absUri
            : new Uri(new Uri(builder.HostEnvironment.BaseAddress), apiUrl);
    }
    return http;
});

builder.Services.AddScoped<IFinTrackApiService, FinTrackApiService>();
builder.Services.AddScoped<IAuthService, AuthService>();

await builder.Build().RunAsync();
