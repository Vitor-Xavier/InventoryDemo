using InventoryDemo.Crosscutting;
using InventoryDemo.Domain.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Services.Categories
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryTableDto>> GetCategories(int skip, int take, CancellationToken cancellationToken = default);

        Task<IEnumerable<SubcategoryDto>> GetSubcategories(int skip, int take, CancellationToken cancellationToken = default);

        Task<CategoryDto> GetCategory(int categoryId, CancellationToken cancellationToken = default);

        Task CreateCategory(Category category, CancellationToken cancellationToken = default);

        Task UpdateCategory(int categoryId, Category category, CancellationToken cancellationToken = default);

        Task DeleteCategory(int categoryId, CancellationToken cancellationToken = default);
    }
}
