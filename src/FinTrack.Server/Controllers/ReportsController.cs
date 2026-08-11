using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FinTrack.Server.Services.Reports;
using FinTrack.Shared.DTOs.Report;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.Server.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
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

    [HttpGet("spending-by-category")]
    [ProducesResponseType(typeof(SpendingByCategoryReportDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSpendingByCategory(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        if (from == default || to == default)
        {
            var now = DateTime.UtcNow;
            from = new DateTime(now.Year, now.Month, 1);
            to = from.AddMonths(1).AddTicks(-1);
        }

        var userId = GetUserId();
        var report = await _reportService.GetSpendingByCategoryReportAsync(userId, from, to);
        return Ok(report);
    }

    [HttpGet("income-vs-expense")]
    [ProducesResponseType(typeof(IncomeVsExpenseReportDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetIncomeVsExpense(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        [FromQuery] string granularity = "monthly")
    {
        if (from == default || to == default)
        {
            var now = DateTime.UtcNow;
            from = now.AddMonths(-5);
            to = now;
        }

        var userId = GetUserId();
        var report = await _reportService.GetIncomeVsExpenseReportAsync(userId, from, to, granularity);
        return Ok(report);
    }
}
