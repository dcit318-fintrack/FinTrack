using FinTrack.Server.Data;
using FinTrack.Server.Models;
using FinTrack.Server.Services.Budgets;
using FinTrack.Server.Services.Dashboard;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit;

namespace FinTrack.Tests;

public class DashboardServiceTests
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
    public async Task GetDashboardSummaryAsync_ShouldCalculateTotalsAndRecentTransactions()
    {
        using var context = GetInMemoryDbContext();
        var budgetService = new BudgetService(context, NullLogger<BudgetService>.Instance);
        var service = new DashboardService(context, budgetService, NullLogger<DashboardService>.Instance);

        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var categoryId = Guid.Parse("11111111-1111-1111-1111-111111111111"); // Food

        var date = new DateTime(2026, 8, 15, 10, 0, 0, DateTimeKind.Utc);

        // Add transactions for our user
        context.Transactions.AddRange(
            new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CategoryId = categoryId,
                Amount = 1500m,
                Type = "Income",
                Description = "Salary bonus",
                Date = date,
                CreatedAt = date
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CategoryId = categoryId,
                Amount = 200m,
                Type = "Expense",
                Description = "Groceries",
                Date = date,
                CreatedAt = date.AddHours(1)
            },
            // Other user transaction (should be isolated)
            new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = otherUserId,
                CategoryId = categoryId,
                Amount = 5000m,
                Type = "Income",
                Description = "Other user salary",
                Date = date,
                CreatedAt = date
            }
        );

        // Add a budget at risk (>80% used)
        context.Budgets.Add(new Budget
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CategoryId = categoryId,
            Limit = 220m, // 200 spent / 220 limit = 90.9% > 80%
            Month = "2026-08",
            CreatedAt = date
        });

        await context.SaveChangesAsync();

        // Act
        var summary = await service.GetDashboardSummaryAsync(userId, "2026-08");

        // Assert
        Assert.NotNull(summary);
        Assert.Equal(1500m, summary.TotalIncome);
        Assert.Equal(200m, summary.TotalExpenses);
        Assert.Equal(1300m, summary.Balance);
        Assert.Equal("2026-08", summary.Month);
        Assert.Equal(2, summary.RecentTransactions.Count);
        Assert.Single(summary.BudgetsAtRisk);
        Assert.Equal("Food", summary.BudgetsAtRisk[0].CategoryName);
    }
}
