using InventoryDemo.Conventions;
using InventoryDemo.Crosscutting;
using InventoryDemo.Domain.Models;
using InventoryDemo.Services.Categories;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [ApiConventionType(typeof(InventoryApiConventions))]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService) => _categoryService = categoryService;

        /// <summary>
        /// Busca paginada de Categorias.
        /// </summary>
        /// <param name="skip">Quantidade de itens a pular</param>
        /// <param name="take">Quantidade de itens a retrair</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns>Tabela de Categorias</returns>
        [HttpGet]
        public async Task<ActionResult<CategoryTableDto>> GetCategories(int skip = 0, int take = 10, CancellationToken cancellationToken = default)
        {
            var categories = await _categoryService.GetCategories(skip, take, cancellationToken);
            return Ok(categories);
        }

        /// <summary>
        /// Busca paginada de Categorias, junto a subcategorias.
        /// </summary>
        /// <param name="skip">Quantidade de itens a pular</param>
        /// <param name="take">Quantidade de itens a retrair</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns>Tabela de Categorias</returns>
        [HttpGet("subcategories")]
        public async Task<ActionResult<CategoryTableDto>> GetSubcategories(int skip = 0, int take = 10, CancellationToken cancellationToken = default)
        {
            var categories = await _categoryService.GetSubcategories(skip, take, cancellationToken);
            return Ok(categories);
        }

        /// <summary>
        /// Busca de Categoria.
        /// </summary>
        /// <param name="categoryId">Identificação do Categoria</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns>Categoria</returns>
        [HttpGet("{categoryId:int}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int categoryId, CancellationToken cancellationToken = default)
        {
            var category = await _categoryService.GetCategory(categoryId, cancellationToken);
            return Ok(category);
        }

        /// <summary>
        /// Insere Categoria.
        /// </summary>
        /// <param name="category">Dados do Categoria</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns>Dados do Categoria</returns>
        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory(Category category, CancellationToken cancellationToken = default)
        {
            await _categoryService.CreateCategory(category, cancellationToken);
            return CreatedAtAction(nameof(GetCategory), new { categoryId = category.CategoryId }, category);
        }

        /// <summary>
        /// Altera dados de Categoria.
        /// </summary>
        /// <param name="categoryId">Identificação do Categoria</param>
        /// <param name="category">Dados do Fornecedor</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        [HttpPut("{categoryId:int}")]
        public async Task<IActionResult> UpdateCategory(int categoryId, Category category, CancellationToken cancellationToken = default)
        {
            await _categoryService.UpdateCategory(categoryId, category, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Remove Categoria.
        /// </summary>
        /// <param name="categoryId">Identificação do Categoria</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        [HttpDelete("{categoryId:int}")]
        public async Task<IActionResult> DeleteCategory(int categoryId, CancellationToken cancellationToken = default)
        {
            await _categoryService.DeleteCategory(categoryId, cancellationToken);
            return NoContent();
        }
    }
}
