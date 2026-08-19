using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FinTrack.Server.Services.Budgets;
using FinTrack.Shared.DTOs.Budget;
using FinTrack.Shared.DTOs.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.Server.Controllers;

[ApiController]
[Route("api/budgets")]
[Authorize]
public class BudgetsController : ControllerBase
{
    private readonly IBudgetService _budgetService;

    public BudgetsController(IBudgetService budgetService)
    {
        _budgetService = budgetService;
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
    [ProducesResponseType(typeof(List<BudgetDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBudgets([FromQuery] string? month)
    {
        var userId = GetUserId();
        var budgets = await _budgetService.GetBudgetsAsync(userId, month ?? string.Empty);
        return Ok(budgets);
    }

    [HttpPost]
    [ProducesResponseType(typeof(BudgetDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateBudgetRequest request)
    {
        var userId = GetUserId();
        var (success, dto, errorMessage, errors, isConflict) = await _budgetService.CreateAsync(userId, request);
        if (!success)
        {
            if (isConflict)
            {
                return Conflict(new ErrorResponse { Message = errorMessage ?? "Duplicate budget." });
            }

            return BadRequest(new ErrorResponse
            {
                Message = errorMessage ?? "Failed to create budget.",
                Errors = errors
            });
        }

        return StatusCode(StatusCodes.Status201Created, dto);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(BudgetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBudgetRequest request)
    {
        var userId = GetUserId();
        var (success, dto, errorMessage) = await _budgetService.UpdateAsync(userId, id, request);
        if (!success)
        {
            return NotFound();
        }

        return Ok(dto);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();
        var success = await _budgetService.DeleteAsync(userId, id);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}
