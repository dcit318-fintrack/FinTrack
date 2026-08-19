using FinTrack.Server.Services.Reports;
using FinTrack.Shared.DTOs.Common;
using FinTrack.Shared.DTOs.Report;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.Server.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize]
public class ReportsController : AuthorizedController
{
    private readonly IReportService _reportService;
    private static readonly HashSet<string> ValidGranularities = new(StringComparer.OrdinalIgnoreCase)
    {
        "daily", "weekly", "monthly"
    };

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("spending-by-category")]
    [ProducesResponseType(typeof(SpendingByCategoryReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSpendingByCategory(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        if (from == default && to == default)
        {
            var now = DateTime.UtcNow;
            from = new DateTime(now.Year, now.Month, 1);
            to = from.AddMonths(1).AddTicks(-1);
        }
        else if (from == default || to == default)
        {
            return BadRequest(new ErrorResponse
            {
                Message = "Validation failed.",
                Errors = new() { { "dateRange", new[] { "Both 'from' and 'to' query parameters must be specified together." } } }
            });
        }

        if (from > to)
        {
            return BadRequest(new ErrorResponse
            {
                Message = "Validation failed.",
                Errors = new() { { "dateRange", new[] { "'from' date cannot be after 'to' date." } } }
            });
        }

        var userId = GetUserId();
        var report = await _reportService.GetSpendingByCategoryReportAsync(userId, from, to);
        return Ok(report);
    }

    [HttpGet("income-vs-expense")]
    [ProducesResponseType(typeof(IncomeVsExpenseReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetIncomeVsExpense(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        [FromQuery] string granularity = "monthly")
    {
        if (from == default && to == default)
        {
            var now = DateTime.UtcNow;
            from = now.AddMonths(-5);
            to = now;
        }
        else if (from == default || to == default)
        {
            return BadRequest(new ErrorResponse
            {
                Message = "Validation failed.",
                Errors = new() { { "dateRange", new[] { "Both 'from' and 'to' query parameters must be specified together." } } }
            });
        }

        if (from > to)
        {
            return BadRequest(new ErrorResponse
            {
                Message = "Validation failed.",
                Errors = new() { { "dateRange", new[] { "'from' date cannot be after 'to' date." } } }
            });
        }

        if (!ValidGranularities.Contains(granularity))
        {
            return BadRequest(new ErrorResponse
            {
                Message = "Validation failed.",
                Errors = new() { { "granularity", new[] { "Granularity must be 'daily', 'weekly', or 'monthly'." } } }
            });
        }

        var userId = GetUserId();
        var report = await _reportService.GetIncomeVsExpenseReportAsync(userId, from, to, granularity);
        return Ok(report);
    }
}
