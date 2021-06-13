using InventoryDemo.Crosscutting;
using InventoryDemo.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Repositories.Products
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<ProductDto> GetProduct(int productId, CancellationToken cancellationToken = default);

        Task<IEnumerable<ProductTableDto>> GetProducts(int skip, int take, CancellationToken cancellationToken = default);

        Task<IEnumerable<Product>> GetByOrder(int orderId, CancellationToken cancellationToken = default);
    }
}
