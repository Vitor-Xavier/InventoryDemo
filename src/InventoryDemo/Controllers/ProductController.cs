using InventoryDemo.Models;
using InventoryDemo.Services.Products;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService) => _productService = productService;

        /// <summary>
        /// Busca paginada de Produtos.
        /// </summary>
        /// <param name="skip">Quantidade de itens a pular</param>
        /// <param name="take">Quantidade de itens a retrair</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns>Tabela de Produtos</returns>
        [HttpGet]
        public async Task<IActionResult> GetProducts(int skip = 0, int take = 10, CancellationToken cancellationToken = default)
        {
            var suppliers = await _productService.GetProducts(skip, take, cancellationToken);
            return Ok(suppliers);
        }

        /// <summary>
        /// Busca de Produto.
        /// </summary>
        /// <param name="productId">Identificação do Produto</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns>Produto</returns>
        [HttpGet("{productId:int}")]
        public async Task<IActionResult> GetProduct(int productId, CancellationToken cancellationToken = default)
        {
            var suppliers = await _productService.GetProduct(productId, cancellationToken);
            return Ok(suppliers);
        }

        /// <summary>
        /// Insere Produto.
        /// </summary>
        /// <param name="product">Dados do Produto</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns>Dados do Produto</returns>
        [HttpPost]
        public async Task<IActionResult> InsertProduct(Product product, CancellationToken cancellationToken = default)
        {
            await _productService.CreateProduct(product, cancellationToken);
            return Created(nameof(ProductController), product);
        }

        /// <summary>
        /// Altera dados de Produto.
        /// </summary>
        /// <param name="productId">Identificação do Produto</param>
        /// <param name="product">Dados do Fornecedor</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        [HttpPut("{productId:int}")]
        public async Task<IActionResult> UpdateProduct(int productId, Product product, CancellationToken cancellationToken = default)
        {
            await _productService.UpdateProduct(productId, product, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Remove Produto.
        /// </summary>
        /// <param name="productId">Identificação do Produto</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        [HttpDelete("{productId:int}")]
        public async Task<IActionResult> DeleteProduct(int productId, CancellationToken cancellationToken = default)
        {
            await _productService.DeleteProduct(productId, cancellationToken);
            return NoContent();
        }
    }
}
