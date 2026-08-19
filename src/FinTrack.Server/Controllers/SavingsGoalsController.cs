using FinTrack.Server.Services.Savings;
using FinTrack.Shared.DTOs.Common;
using FinTrack.Shared.DTOs.Savings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.Server.Controllers;

[ApiController]
[Route("api/savings-goals")]
[Authorize]
public class SavingsGoalsController : AuthorizedController
{
    private readonly ISavingsGoalService _savingsGoalService;

    public SavingsGoalsController(ISavingsGoalService savingsGoalService)
    {
        _savingsGoalService = savingsGoalService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<SavingsGoalDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSavingsGoals()
    {
        var userId = GetUserId();
        var goals = await _savingsGoalService.GetSavingsGoalsAsync(userId);
        return Ok(goals);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SavingsGoalDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateSavingsGoalRequest request)
    {
        var userId = GetUserId();
        var result = await _savingsGoalService.CreateAsync(userId, request);
        if (!result.IsSuccess)
        {
            return BadRequest(new ErrorResponse
            {
                Message = result.ErrorMessage ?? "Failed to create savings goal.",
                Errors = result.ValidationErrors
            });
        }

        return StatusCode(StatusCodes.Status201Created, result.Data);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(SavingsGoalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSavingsGoalRequest request)
    {
        var userId = GetUserId();
        var result = await _savingsGoalService.UpdateAsync(userId, id, request);
        if (!result.IsSuccess)
        {
            return NotFound();
        }

        return Ok(result.Data);
    }

    [HttpPost("{id:guid}/contribute")]
    [ProducesResponseType(typeof(SavingsGoalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Contribute(Guid id, [FromBody] ContributeRequest request)
    {
        var userId = GetUserId();
        var result = await _savingsGoalService.ContributeAsync(userId, id, request);
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
        var success = await _savingsGoalService.DeleteAsync(userId, id);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}
