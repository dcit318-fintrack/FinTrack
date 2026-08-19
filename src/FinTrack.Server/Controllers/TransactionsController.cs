using FinTrack.Server.Services.Transactions;
using FinTrack.Shared.DTOs.Common;
using FinTrack.Shared.DTOs.Transaction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.Server.Controllers;

[ApiController]
[Route("api/transactions")]
[Authorize]
public class TransactionsController : AuthorizedController
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<TransactionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTransactions(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] Guid? categoryId,
        [FromQuery] string? type,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25)
    {
        var userId = GetUserId();
        var result = await _transactionService.GetTransactionsAsync(userId, from, to, categoryId, type, page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = GetUserId();
        var dto = await _transactionService.GetByIdAsync(userId, id);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTransactionRequest request)
    {
        var userId = GetUserId();
        var result = await _transactionService.CreateAsync(userId, request);
        if (!result.IsSuccess)
        {
            return BadRequest(new ErrorResponse
            {
                Message = result.ErrorMessage ?? "Failed to create transaction.",
                Errors = result.ValidationErrors
            });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTransactionRequest request)
    {
        var userId = GetUserId();
        var result = await _transactionService.UpdateAsync(userId, id, request);
        if (!result.IsSuccess)
        {
            if (result.ErrorMessage == "Transaction not found.")
            {
                return NotFound();
            }

            return BadRequest(new ErrorResponse
            {
                Message = result.ErrorMessage ?? "Failed to update transaction.",
                Errors = result.ValidationErrors
            });
        }

        return Ok(result.Data);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();
        var success = await _transactionService.DeleteAsync(userId, id);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}
