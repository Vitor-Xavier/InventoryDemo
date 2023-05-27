using InventoryDemo.Crosscutting;
using InventoryDemo.Domain.Models;
using InventoryDemo.Infrastructure.Persistance.Repositories.Categories;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        public readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public Task<CategoryDto> GetCategory(int categoryId, CancellationToken cancellationToken = default) =>
            _categoryRepository.GetCategory(categoryId, cancellationToken);

        public async Task<IEnumerable<CategoryTableDto>> GetCategories(int skip, int take, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var categories = await _categoryRepository.GetCategories(skip, take, cancellationToken);

            return categories;
        }

        public async Task<IEnumerable<SubcategoryDto>> GetSubcategories(int skip, int take, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var categories = await _categoryRepository.GetCategoriesWithSub(skip, take, cancellationToken);

            return categories;
        }

        public async Task CreateCategory(Category category, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!IsValid(category)) throw new BadHttpRequestException("Categoria inválida");

            await _categoryRepository.Add(category, cancellationToken);
        }

        public async Task UpdateCategory(int categoryId, Category category, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!IsValid(category)) throw new BadHttpRequestException("Categoria inválida");
            category.CategoryId = categoryId;

            await _categoryRepository.Edit(category, cancellationToken);
        }

        public async Task DeleteCategory(int categoryId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Category category = new() { CategoryId = categoryId, Deleted = true };

            await _categoryRepository.Delete(category, cancellationToken);
        }

        public bool IsValid(Category category) => category is { Title.Length: > 0 };
    }
}
