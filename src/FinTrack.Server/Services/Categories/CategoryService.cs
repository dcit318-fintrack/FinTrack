using FinTrack.Server.Data;
using FinTrack.Server.Models;
using FinTrack.Shared.DTOs.Category;
using FinTrack.Shared.DTOs.Common;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Server.Services.Categories;

public class CategoryService : ICategoryService
{
    private readonly FinTrackDbContext _dbContext;

    public CategoryService(FinTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        return await _dbContext.Categories
            .AsNoTracking()
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type
            })
            .ToListAsync();
    }

    public async Task<Result<CategoryDto>> CreateCategoryAsync(CreateCategoryRequest request)
    {
        var nameNormalized = request.Name.Trim();
        var typeNormalized = request.Type.Equals("Income", StringComparison.OrdinalIgnoreCase) ? "Income" : "Expense";

        var exists = await _dbContext.Categories
            .AnyAsync(c => c.Name.ToLower() == nameNormalized.ToLower());

        if (exists)
        {
            return Result<CategoryDto>.Failure(
                "Validation failed.",
                new Dictionary<string, string[]>
                {
                    { "name", new[] { $"Category '{nameNormalized}' already exists." } }
                });
        }

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = nameNormalized,
            Type = typeNormalized
        };

        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();

        return Result<CategoryDto>.Success(new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Type = category.Type
        });
    }
}
