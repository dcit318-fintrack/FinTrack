using FinTrack.Server.Data;
using FinTrack.Server.Models;
using FinTrack.Server.Services.Transactions;
using FinTrack.Shared.DTOs.Transaction;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FinTrack.Tests;

public class TransactionServiceTests
{
    private FinTrackDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<FinTrackDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new FinTrackDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateTransaction_AndIsolateByUserId()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var service = new TransactionService(context);

        var userA = Guid.NewGuid();
        var userB = Guid.NewGuid();
        var categoryId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var createReq = new CreateTransactionRequest
        {
            Amount = 150.50m,
            Type = "Expense",
            CategoryId = categoryId,
            Description = "Groceries",
            Date = DateTime.UtcNow
        };

        // Act
        var (success, dto, errorMessage, errors) = await service.CreateAsync(userA, createReq);

        // Assert
        Assert.True(success);
        Assert.NotNull(dto);
        Assert.Equal(150.50m, dto.Amount);

        // Verify User A can fetch it
        var pagedUserA = await service.GetTransactionsAsync(userA, null, null, null, null);
        Assert.Single(pagedUserA.Items);

        // Verify User B receives empty list (Data isolation)
        var pagedUserB = await service.GetTransactionsAsync(userB, null, null, null, null);
        Assert.Empty(pagedUserB.Items);
    }
}
