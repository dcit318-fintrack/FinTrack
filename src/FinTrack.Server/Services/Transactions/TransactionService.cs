using FinTrack.Server.Data;
using FinTrack.Server.Models;
using FinTrack.Shared.DTOs.Common;
using FinTrack.Shared.DTOs.Transaction;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Server.Services.Transactions;

public class TransactionService : ITransactionService
{
    private readonly FinTrackDbContext _dbContext;

    public TransactionService(FinTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<TransactionDto>> GetTransactionsAsync(
        Guid userId,
        DateTime? from,
        DateTime? to,
        Guid? categoryId,
        string? type,
        int page = 1,
        int pageSize = 25)
    {
        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 25 : pageSize;

        var query = _dbContext.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId);

        if (from.HasValue)
        {
            query = query.Where(t => t.Date >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(t => t.Date <= to.Value);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(t => t.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(type))
        {
            query = query.Where(t => t.Type.ToLower() == type.ToLower());
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TransactionDto
            {
                Id = t.Id,
                Amount = t.Amount,
                Type = t.Type,
                CategoryId = t.CategoryId,
                CategoryName = t.Category.Name,
                Description = t.Description,
                Date = t.Date
            })
            .ToListAsync();

        return new PagedResult<TransactionDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<TransactionDto?> GetByIdAsync(Guid userId, Guid id)
    {
        return await _dbContext.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId && t.Id == id)
            .Select(t => new TransactionDto
            {
                Id = t.Id,
                Amount = t.Amount,
                Type = t.Type,
                CategoryId = t.CategoryId,
                CategoryName = t.Category.Name,
                Description = t.Description,
                Date = t.Date
            })
            .FirstOrDefaultAsync();
    }

    public async Task<Result<TransactionDto>> CreateAsync(
        Guid userId,
        CreateTransactionRequest request)
    {
        var category = request.CategoryId.HasValue
            ? await _dbContext.Categories.FindAsync(request.CategoryId.Value)
            : null;

        if (category == null)
        {
            return Result<TransactionDto>.Failure(
                "Validation failed.",
                new Dictionary<string, string[]>
                {
                    { "categoryId", new[] { "Category does not exist." } }
                });
        }

        if (!string.IsNullOrWhiteSpace(request.Type) && !request.Type.Equals(category.Type, StringComparison.OrdinalIgnoreCase))
        {
            return Result<TransactionDto>.Failure(
                "Validation failed.",
                new Dictionary<string, string[]>
                {
                    { "type", new[] { $"Transaction type '{request.Type}' does not match category type '{category.Type}'." } }
                });
        }

        if (request.Date > DateTime.UtcNow.AddMinutes(5))
        {
            return Result<TransactionDto>.Failure(
                "Validation failed.",
                new Dictionary<string, string[]>
                {
                    { "date", new[] { "Transaction date cannot be in the future." } }
                });
        }

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CategoryId = request.CategoryId!.Value,
            Amount = Math.Round(request.Amount, 2),
            Type = category.Type,
            Description = request.Description ?? string.Empty,
            Date = request.Date,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync();

        var dto = new TransactionDto
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Type = transaction.Type,
            CategoryId = transaction.CategoryId,
            CategoryName = category.Name,
            Description = transaction.Description,
            Date = transaction.Date
        };

        return Result<TransactionDto>.Success(dto);
    }

    public async Task<Result<TransactionDto>> UpdateAsync(
        Guid userId,
        Guid id,
        UpdateTransactionRequest request)
    {
        var transaction = await _dbContext.Transactions
            .FirstOrDefaultAsync(t => t.UserId == userId && t.Id == id);

        if (transaction == null)
        {
            return Result<TransactionDto>.Failure("Transaction not found.");
        }

        var category = request.CategoryId.HasValue
            ? await _dbContext.Categories.FindAsync(request.CategoryId.Value)
            : null;

        if (category == null)
        {
            return Result<TransactionDto>.Failure(
                "Validation failed.",
                new Dictionary<string, string[]>
                {
                    { "categoryId", new[] { "Category does not exist." } }
                });
        }

        if (!string.IsNullOrWhiteSpace(request.Type) && !request.Type.Equals(category.Type, StringComparison.OrdinalIgnoreCase))
        {
            return Result<TransactionDto>.Failure(
                "Validation failed.",
                new Dictionary<string, string[]>
                {
                    { "type", new[] { $"Transaction type '{request.Type}' does not match category type '{category.Type}'." } }
                });
        }

        transaction.Amount = Math.Round(request.Amount, 2);
        transaction.Type = category.Type;
        transaction.CategoryId = request.CategoryId!.Value;
        transaction.Description = request.Description ?? string.Empty;
        transaction.Date = request.Date;

        await _dbContext.SaveChangesAsync();

        var dto = new TransactionDto
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Type = transaction.Type,
            CategoryId = transaction.CategoryId,
            CategoryName = category.Name,
            Description = transaction.Description,
            Date = transaction.Date
        };

        return Result<TransactionDto>.Success(dto);
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid id)
    {
        var transaction = await _dbContext.Transactions
            .FirstOrDefaultAsync(t => t.UserId == userId && t.Id == id);

        if (transaction == null)
        {
            return false;
        }

        _dbContext.Transactions.Remove(transaction);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
