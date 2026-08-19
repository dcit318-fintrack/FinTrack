using FinTrack.Shared.DTOs.Common;
using FinTrack.Shared.DTOs.Transaction;

namespace FinTrack.Server.Services.Transactions;

public interface ITransactionService
{
    Task<PagedResult<TransactionDto>> GetTransactionsAsync(
        Guid userId,
        DateTime? from,
        DateTime? to,
        Guid? categoryId,
        string? type,
        int page = 1,
        int pageSize = 25);

    Task<TransactionDto?> GetByIdAsync(Guid userId, Guid id);

    Task<(bool success, TransactionDto? dto, string? errorMessage, Dictionary<string, string[]>? errors)> CreateAsync(
        Guid userId,
        CreateTransactionRequest request);

    Task<(bool success, TransactionDto? dto, string? errorMessage, Dictionary<string, string[]>? errors)> UpdateAsync(
        Guid userId,
        Guid id,
        UpdateTransactionRequest request);

    Task<bool> DeleteAsync(Guid userId, Guid id);
}
