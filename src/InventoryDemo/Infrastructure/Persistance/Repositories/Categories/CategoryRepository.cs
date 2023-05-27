using InventoryDemo.Crosscutting;
using InventoryDemo.Domain.Models;
using InventoryDemo.Infrastructure.Persistance.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Infrastructure.Persistance.Repositories.Categories
{
    public class CategoryRepository : Repository<Category, InventoryContext>, ICategoryRepository
    {
        public CategoryRepository(InventoryContext context) : base(context) { }

        public Task<CategoryDto> GetCategory(int categoryId, CancellationToken cancellationToken = default) =>
            _context.Categories.AsNoTracking().Where(category => category.CategoryId == categoryId).Select(category => new CategoryDto(category.Title, category.Description)).FirstOrDefaultAsync(cancellationToken);

        public Task<int> GetTotalCategories(CancellationToken cancellationToken = default) =>
            _context.Categories.AsNoTracking().Where(category => !category.Deleted).CountAsync(cancellationToken);

        public async Task<IEnumerable<CategoryTableDto>> GetCategories(int skip, int take, CancellationToken cancellationToken = default) =>
            await _context.Categories.AsNoTracking().Where(category => !category.Deleted).OrderBy(category => category.Title).Select(category => new CategoryTableDto(category.CategoryId, category.Title, category.Description)).Skip(skip).Take(take).ToListAsync(cancellationToken);

        public async Task<IEnumerable<SubcategoryDto>> GetCategoriesWithSub(int skip, int take, CancellationToken cancellationToken = default) =>
            await _context.Categories.AsNoTracking().Where(category => !category.Deleted && category.ParentId == null).Select(GetNestedCategories()).Skip(skip).Take(take).ToListAsync(cancellationToken);

        private static Expression<Func<Category, SubcategoryDto>> GetNestedCategories(int maxDepth = 3, int currentDepth = 0)
        {
            currentDepth++;

            Expression<Func<Category, SubcategoryDto>> result = category => new SubcategoryDto
            {
                CategoryId = category.CategoryId,
                Title = category.Title,
                Subcategories = maxDepth == currentDepth ? Array.Empty<SubcategoryDto>() :
                    category.Subcategories.AsQueryable().Select(GetNestedCategories(maxDepth, currentDepth)).ToList()
            };

            return result;
        }
    }
}
