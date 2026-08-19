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

var backendUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5104/";
if (!backendUrl.EndsWith("/")) backendUrl += "/";

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(backendUrl) });
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFinTrackApiService, FinTrackApiService>();

await builder.Build().RunAsync();
