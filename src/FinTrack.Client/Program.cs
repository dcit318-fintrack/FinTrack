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

// Reads ApiBaseUrl from wwwroot/appsettings.json.
// In production (Netlify), set this to the Render backend URL.
// Falls back to the host's BaseAddress for local development.
builder.Services.AddScoped(sp =>
{
    var http = new HttpClient();
    var apiUrl = builder.Configuration["ApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress;
    http.BaseAddress = new Uri(apiUrl);
    return http;
});

builder.Services.AddScoped<IFinTrackApiService, FinTrackApiService>();
builder.Services.AddScoped<IAuthService, AuthService>();

await builder.Build().RunAsync();
