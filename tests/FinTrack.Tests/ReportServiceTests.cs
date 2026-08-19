using FinTrack.Server.Data;
using FinTrack.Server.Models;
using FinTrack.Server.Services.Reports;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit;

namespace FinTrack.Tests;

public class ReportServiceTests
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
    public async Task GetSpendingByCategoryReportAsync_ShouldCalculateCategoryPercentagesCorrectly()
    {
        using var context = GetInMemoryDbContext();
        var service = new ReportService(context, NullLogger<ReportService>.Instance);

        var userId = Guid.NewGuid();
        var foodId = Guid.Parse("11111111-1111-1111-1111-111111111111"); // Food
        var transportId = Guid.Parse("22222222-2222-2222-2222-222222222222"); // Transport

        var from = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc);
        var to = new DateTime(2026, 8, 31, 23, 59, 59, DateTimeKind.Utc);

        context.Transactions.AddRange(
            new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CategoryId = foodId,
                Amount = 300m,
                Type = "Expense",
                Description = "Groceries",
                Date = new DateTime(2026, 8, 5, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CategoryId = transportId,
                Amount = 100m,
                Type = "Expense",
                Description = "Bus pass",
                Date = new DateTime(2026, 8, 10, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow
            },
            // Income transaction should not be in spending by category report
            new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CategoryId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                Amount = 2000m,
                Type = "Income",
                Description = "Monthly salary",
                Date = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow
            }
        );

        await context.SaveChangesAsync();

        // Act
        var report = await service.GetSpendingByCategoryReportAsync(userId, from, to);

        // Assert
        Assert.NotNull(report);
        Assert.Equal(400m, report.TotalSpent);
        Assert.Equal(2, report.Categories.Count);

        var foodItem = report.Categories.FirstOrDefault(c => c.CategoryName == "Food");
        Assert.NotNull(foodItem);
        Assert.Equal(300m, foodItem.Amount);
        Assert.Equal(75.0, foodItem.PercentOfTotal); // 300 / 400 = 75%

        var transportItem = report.Categories.FirstOrDefault(c => c.CategoryName == "Transport");
        Assert.NotNull(transportItem);
        Assert.Equal(100m, transportItem.Amount);
        Assert.Equal(25.0, transportItem.PercentOfTotal); // 100 / 400 = 25%
    }

    [Fact]
    public async Task GetIncomeVsExpenseReportAsync_ShouldReturnDailyPointsWithZeroFilledGaps()
    {
        using var context = GetInMemoryDbContext();
        var service = new ReportService(context, NullLogger<ReportService>.Instance);

        var userId = Guid.NewGuid();
        var foodId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var salaryId = Guid.Parse("66666666-6666-6666-6666-666666666666");

        var from = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc);
        var to = new DateTime(2026, 8, 3, 0, 0, 0, DateTimeKind.Utc);

        context.Transactions.AddRange(
            new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CategoryId = salaryId,
                Amount = 1000m,
                Type = "Income",
                Description = "Salary",
                Date = from,
                CreatedAt = DateTime.UtcNow
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CategoryId = foodId,
                Amount = 50m,
                Type = "Expense",
                Description = "Food",
                Date = to,
                CreatedAt = DateTime.UtcNow
            }
        );

        await context.SaveChangesAsync();

        // Act
        var report = await service.GetIncomeVsExpenseReportAsync(userId, from, to, "daily");

        // Assert
        Assert.NotNull(report);
        Assert.Equal(3, report.Points.Count); // Aug 1, Aug 2, Aug 3

        Assert.Equal(1000m, report.Points[0].Income);
        Assert.Equal(0m, report.Points[0].Expense);

        Assert.Equal(0m, report.Points[1].Income); // Aug 2 zero-filled gap
        Assert.Equal(0m, report.Points[1].Expense);

        Assert.Equal(0m, report.Points[2].Income);
        Assert.Equal(50m, report.Points[2].Expense);
    }
}
