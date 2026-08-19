using FinTrack.Server.Services.Dashboard;
using FinTrack.Shared.DTOs.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.Server.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController : AuthorizedController
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(DashboardSummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboard([FromQuery] string? month)
    {
        var userId = GetUserId();
        var summary = await _dashboardService.GetDashboardSummaryAsync(userId, month);
        return Ok(summary);
    }
}
