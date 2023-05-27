using InventoryDemo.Crosscutting;
using InventoryDemo.Domain.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Infrastructure.Persistance.Repositories.Categories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<CategoryDto> GetCategory(int categoryId, CancellationToken cancellationToken = default);

        Task<int> GetTotalCategories(CancellationToken cancellationToken = default);

        Task<IEnumerable<CategoryTableDto>> GetCategories(int skip, int take, CancellationToken cancellationToken = default);

        Task<IEnumerable<SubcategoryDto>> GetCategoriesWithSub(int skip, int take, CancellationToken cancellationToken = default);
    }
}
