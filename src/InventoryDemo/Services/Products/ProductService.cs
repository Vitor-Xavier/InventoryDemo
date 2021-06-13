using InventoryDemo.Crosscutting;
using InventoryDemo.Models;
using InventoryDemo.Repositories.Products;
using System;
using System.Collections.Generic;
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

        public Task<IEnumerable<ProductTableDto>> GetProducts(int skip, int take, CancellationToken cancellationToken = default) =>
            _productRepository.GetProducts(skip, take, cancellationToken);

        public async Task CreateProduct(Product product, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!IsValid(product)) throw new Exception("Registro inválido");

            await _productRepository.Add(product, cancellationToken);
        }
        
        public async Task UpdateProduct(int productId, Product product, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!IsValid(product)) throw new Exception("Registro inválido");
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
            product is { Name: { Length: > 0 }, Description: { Length: > 0 }, Code: { Length: > 0 }, PricePerUnit: > 0.0m, MinimumRequired: >= 0.0m };
    }
}
