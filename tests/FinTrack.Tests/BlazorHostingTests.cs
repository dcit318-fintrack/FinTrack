using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FinTrack.Tests;

public class BlazorHostingTests : IntegrationTestBase
{
    public BlazorHostingTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsJsonStatusOk()
    {
        var response = await Client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"status\":\"healthy\"", body);
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/login")]
    [InlineData("/register")]
    [InlineData("/dashboard")]
    [InlineData("/transactions")]
    [InlineData("/budgets")]
    [InlineData("/savings")]
    public async Task SpaRoutes_ReturnIndexHtml(string path)
    {
        var response = await Client.GetAsync(path);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);

        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("FinTrack", html);
        Assert.Contains("blazor.webassembly.js", html);
    }
}
