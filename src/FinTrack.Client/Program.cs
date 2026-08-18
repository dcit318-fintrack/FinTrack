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

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IFinTrackApiService, FinTrackApiService>();
builder.Services.AddScoped<IAuthService, AuthService>();

await builder.Build().RunAsync();
