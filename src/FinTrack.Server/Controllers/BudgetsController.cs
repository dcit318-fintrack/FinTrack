using FinTrack.Server.Services.Budgets;
using FinTrack.Shared.DTOs.Budget;
using FinTrack.Shared.DTOs.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.Server.Controllers;

[ApiController]
[Route("api/budgets")]
[Authorize]
public class BudgetsController : AuthorizedController
{
    private readonly IBudgetService _budgetService;

    public BudgetsController(IBudgetService budgetService)
    {
        _budgetService = budgetService;
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
        var result = await _budgetService.CreateAsync(userId, request);
        if (!result.IsSuccess)
        {
            if (result.IsConflict)
            {
                return Conflict(new ErrorResponse { Message = result.ErrorMessage ?? "Duplicate budget." });
            }

            return BadRequest(new ErrorResponse
            {
                Message = result.ErrorMessage ?? "Failed to create budget.",
                Errors = result.ValidationErrors
            });
        }

        return StatusCode(StatusCodes.Status201Created, result.Data);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(BudgetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBudgetRequest request)
    {
        var userId = GetUserId();
        var result = await _budgetService.UpdateAsync(userId, id, request);
        if (!result.IsSuccess)
        {
            return NotFound();
        }

        return Ok(result.Data);
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
