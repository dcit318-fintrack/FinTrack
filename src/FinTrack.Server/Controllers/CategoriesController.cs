using FinTrack.Server.Services.Categories;
using FinTrack.Shared.DTOs.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.Server.Controllers;

[ApiController]
[Route("api/categories")]
[Authorize]
public class CategoriesController : AuthorizedController
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<FinTrack.Shared.DTOs.Category.CategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _categoryService.GetCategoriesAsync();
        return Ok(categories);
    }

    [HttpPost]
    [ProducesResponseType(typeof(FinTrack.Shared.DTOs.Category.CategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCategory([FromBody] FinTrack.Shared.DTOs.Category.CreateCategoryRequest request)
    {
        var result = await _categoryService.CreateCategoryAsync(request);
        if (!result.IsSuccess)
        {
            return BadRequest(new ErrorResponse
            {
                Message = result.ErrorMessage ?? "Validation failed.",
                Errors = result.ValidationErrors
            });
        }

        return StatusCode(StatusCodes.Status201Created, result.Data);
    }
}
