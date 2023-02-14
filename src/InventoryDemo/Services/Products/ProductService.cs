using InventoryDemo.Crosscutting;
using InventoryDemo.Domain.Models;
using InventoryDemo.Infrastructure.Repositories.Products;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Services.Products
{
    public class ProductService : IProductService
    {
        public readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public Task<ProductDto> GetProduct(int productId, CancellationToken cancellationToken = default) =>
            _productRepository.GetProduct(productId, cancellationToken);

        public async Task<TableDto<ProductTableDto>> GetProducts(int skip, int take, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var products = await _productRepository.GetProducts(skip, take, cancellationToken);
            var total = await _productRepository.GetTotalProducts(cancellationToken);

            return new TableDto<ProductTableDto>(products, total);
        }

        public async Task CreateProduct(Product product, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!IsValid(product)) throw new BadHttpRequestException("Produto inválido");

            await _productRepository.Add(product, cancellationToken);
        }
        
        public async Task UpdateProduct(int productId, Product product, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!IsValid(product)) throw new BadHttpRequestException("Produto inválido");
            product.ProductId = productId;

            await _productRepository.Edit(product, cancellationToken);
        }

        public async Task DeleteProduct(int productId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Product product = new() { ProductId = productId, Deleted = true };

            await _productRepository.Delete(product, cancellationToken);
        }

        public bool IsValid(Product product) => 
            product is { Name.Length: > 0, Description.Length: > 0, Code.Length: > 0, PricePerUnit: > 0.0m, MinimumRequired: >= 0.0m };
    }
}
