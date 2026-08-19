using FinTrack.Shared.DTOs.Category;
using FinTrack.Shared.DTOs.Common;

namespace FinTrack.Server.Services.Categories;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetCategoriesAsync();
    Task<Result<CategoryDto>> CreateCategoryAsync(CreateCategoryRequest request);
}
