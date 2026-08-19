using System.Text;
using FinTrack.Server.Data;
using FinTrack.Server.Middleware;
using FinTrack.Server.Models;
using FinTrack.Server.Services.Auth;
using FinTrack.Server.Services.Budgets;
using FinTrack.Server.Services.Dashboard;
using FinTrack.Server.Services.Reports;
using FinTrack.Server.Services.Savings;
using FinTrack.Server.Services.Transactions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<FinTrackDbContext>(options =>
    options.UseInMemoryDatabase("FinTrackInMemoryDb"));

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 6;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<FinTrackDbContext>()
    .AddDefaultTokenProviders();

var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "FinTrack_Super_Secret_Key_For_Jwt_Token_Generation_2026_Must_Be_Long_Enough!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "FinTrackServer";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "FinTrackClient";

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IBudgetService, BudgetService>();
builder.Services.AddScoped<ISavingsGoalService, SavingsGoalService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IReportService, ReportService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FinTrackDbContext>();
    dbContext.Database.EnsureCreated();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var demoEmail = "samuel@ug.edu.gh";
    var demoUser = userManager.FindByEmailAsync(demoEmail).GetAwaiter().GetResult();
    if (demoUser == null)
    {
        demoUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = demoEmail,
            Email = demoEmail,
            FullName = "Samuel Watson",
            CreatedAt = DateTime.UtcNow
        };
        var createRes = userManager.CreateAsync(demoUser, "Password123!").GetAwaiter().GetResult();
        if (createRes.Succeeded)
        {
            var foodCat = dbContext.Categories.FirstOrDefault(c => c.Name == "Food");
            var transportCat = dbContext.Categories.FirstOrDefault(c => c.Name == "Transport");
            var rentCat = dbContext.Categories.FirstOrDefault(c => c.Name == "Rent");
            var entertainmentCat = dbContext.Categories.FirstOrDefault(c => c.Name == "Entertainment");
            var salaryCat = dbContext.Categories.FirstOrDefault(c => c.Name == "Salary");

            if (foodCat != null && salaryCat != null && transportCat != null && rentCat != null)
            {
                dbContext.Transactions.AddRange(
                    new Transaction { Id = Guid.NewGuid(), UserId = demoUser.Id, Amount = 84.50m, Type = "Expense", CategoryId = foodCat.Id, Description = "Whole Foods Market", Date = DateTime.Today.AddHours(10).AddMinutes(42) },
                    new Transaction { Id = Guid.NewGuid(), UserId = demoUser.Id, Amount = 4.75m, Type = "Expense", CategoryId = foodCat.Id, Description = "Starbucks", Date = DateTime.Today.AddHours(8).AddMinutes(15) },
                    new Transaction { Id = Guid.NewGuid(), UserId = demoUser.Id, Amount = 50.00m, Type = "Expense", CategoryId = foodCat.Id, Description = "Grocery", Date = DateTime.Today.AddHours(10).AddMinutes(24) },
                    new Transaction { Id = Guid.NewGuid(), UserId = demoUser.Id, Amount = 3250.00m, Type = "Income", CategoryId = salaryCat.Id, Description = "Tech Corp Inc.", Date = DateTime.Today.AddDays(-1).AddHours(9) },
                    new Transaction { Id = Guid.NewGuid(), UserId = demoUser.Id, Amount = 24.20m, Type = "Expense", CategoryId = transportCat.Id, Description = "Uber", Date = DateTime.Today.AddDays(-1).AddHours(18).AddMinutes(30) },
                    new Transaction { Id = Guid.NewGuid(), UserId = demoUser.Id, Amount = 600.00m, Type = "Expense", CategoryId = rentCat.Id, Description = "Rent", Date = DateTime.Today.AddDays(-5).AddHours(12) }
                );

                dbContext.Budgets.AddRange(
                    new Budget { Id = Guid.NewGuid(), UserId = demoUser.Id, CategoryId = foodCat.Id, Limit = 500.00m, Month = DateTime.Today.ToString("yyyy-MM") },
                    new Budget { Id = Guid.NewGuid(), UserId = demoUser.Id, CategoryId = transportCat.Id, Limit = 150.00m, Month = DateTime.Today.ToString("yyyy-MM") },
                    new Budget { Id = Guid.NewGuid(), UserId = demoUser.Id, CategoryId = entertainmentCat?.Id ?? foodCat.Id, Limit = 100.00m, Month = DateTime.Today.ToString("yyyy-MM") }
                );

                dbContext.SavingsGoals.AddRange(
                    new SavingsGoal { Id = Guid.NewGuid(), UserId = demoUser.Id, Name = "New Laptop", TargetAmount = 2000.00m, CurrentAmount = 1500.00m, TargetDate = DateTime.Today.AddMonths(4) },
                    new SavingsGoal { Id = Guid.NewGuid(), UserId = demoUser.Id, Name = "Emergency Fund", TargetAmount = 5000.00m, CurrentAmount = 1200.00m, TargetDate = DateTime.Today.AddYears(1) },
                    new SavingsGoal { Id = Guid.NewGuid(), UserId = demoUser.Id, Name = "Vacation", TargetAmount = 2000.00m, CurrentAmount = 450.00m, TargetDate = DateTime.Today.AddMonths(8) }
                );

                dbContext.SaveChanges();
            }
        }
    }
}

app.UseMiddleware<ApiExceptionMiddleware>();



app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
