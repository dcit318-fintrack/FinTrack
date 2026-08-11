using FinTrack.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Server.Data;

public class FinTrackDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public FinTrackDbContext(DbContextOptions<FinTrackDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Budget> Budgets => Set<Budget>();
    public DbSet<SavingsGoal> SavingsGoals => Set<SavingsGoal>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Budget unique per user, category, and month
        builder.Entity<Budget>()
            .HasIndex(b => new { b.UserId, b.CategoryId, b.Month })
            .IsUnique();

        // Configure precision for decimal currency fields
        builder.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasPrecision(18, 2);

        builder.Entity<Budget>()
            .Property(b => b.Limit)
            .HasPrecision(18, 2);

        builder.Entity<SavingsGoal>()
            .Property(s => s.TargetAmount)
            .HasPrecision(18, 2);

        builder.Entity<SavingsGoal>()
            .Property(s => s.CurrentAmount)
            .HasPrecision(18, 2);

        // Index for fast query filtering by User & Date
        builder.Entity<Transaction>()
            .HasIndex(t => new { t.UserId, t.Date });

        // Seed initial categories
        var foodId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var transportId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var rentId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var utilitiesId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var entertainmentId = Guid.Parse("55555555-5555-5555-5555-555555555555");
        var salaryId = Guid.Parse("66666666-6666-6666-6666-666666666666");
        var otherId = Guid.Parse("77777777-7777-7777-7777-777777777777");

        builder.Entity<Category>().HasData(
            new Category { Id = foodId, Name = "Food", Type = "Expense" },
            new Category { Id = transportId, Name = "Transport", Type = "Expense" },
            new Category { Id = rentId, Name = "Rent", Type = "Expense" },
            new Category { Id = utilitiesId, Name = "Utilities", Type = "Expense" },
            new Category { Id = entertainmentId, Name = "Entertainment", Type = "Expense" },
            new Category { Id = salaryId, Name = "Salary", Type = "Income" },
            new Category { Id = otherId, Name = "Other", Type = "Expense" }
        );
    }
}
