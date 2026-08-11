using FinTrack.Server.Data;
using FinTrack.Server.Models;
using FinTrack.Shared.DTOs.Category;
using FinTrack.Shared.DTOs.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Server.Controllers;

[ApiController]
[Route("api/categories")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly FinTrackDbContext _dbContext;

    public CategoriesController(FinTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _dbContext.Categories
            .AsNoTracking()
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type
            })
            .ToListAsync();

        return Ok(categories);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        var nameNormalized = request.Name.Trim();
        var typeNormalized = request.Type.Equals("Income", StringComparison.OrdinalIgnoreCase) ? "Income" : "Expense";

        var exists = await _dbContext.Categories
            .AnyAsync(c => c.Name.ToLower() == nameNormalized.ToLower());

        if (exists)
        {
            var errors = new Dictionary<string, string[]>
            {
                { "name", new[] { $"Category '{nameNormalized}' already exists." } }
            };
            return BadRequest(new ErrorResponse { Message = "Validation failed.", Errors = errors });
        }

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = nameNormalized,
            Type = typeNormalized
        };

        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();

        var dto = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Type = category.Type
        };

        return CreatedAtAction(nameof(GetCategories), new { id = dto.Id }, dto);
    }
}
