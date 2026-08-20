using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FinTrack.Server.Services.Dashboard;
using FinTrack.Shared.DTOs.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.Server.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }

        throw new UnauthorizedAccessException("User identity not found.");
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
