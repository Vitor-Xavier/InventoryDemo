using InventoryDemo.Crosscutting;
using InventoryDemo.Models;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Services.Products
{
    public interface IProductService
    {
        Task<ProductDto> GetProduct(int productId, CancellationToken cancellationToken = default);

        Task<TableDto<ProductTableDto>> GetProducts(int skip, int take, CancellationToken cancellationToken = default);

        Task CreateProduct(Product product, CancellationToken cancellationToken = default);

        Task UpdateProduct(int productId, Product product, CancellationToken cancellationToken = default);

        Task DeleteProduct(int productId, CancellationToken cancellationToken = default);
    }
}
