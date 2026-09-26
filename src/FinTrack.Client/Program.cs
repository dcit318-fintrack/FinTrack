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
culture.NumberFormat.CurrencyNegativePattern = 1;
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

builder.Services.AddTransient<AuthenticationHandler>();
builder.Services.AddScoped(sp =>
{
    var authHandler = sp.GetRequiredService<AuthenticationHandler>();
    authHandler.InnerHandler = new HttpClientHandler();
    var http = new HttpClient(authHandler);
    var apiUrl = builder.Configuration["ApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress;
    http.BaseAddress = new Uri(apiUrl);
    return http;
});

builder.Services.AddScoped<IFinTrackApiService, FinTrackApiService>();
builder.Services.AddScoped<IAuthService, AuthService>();

await builder.Build().RunAsync();
