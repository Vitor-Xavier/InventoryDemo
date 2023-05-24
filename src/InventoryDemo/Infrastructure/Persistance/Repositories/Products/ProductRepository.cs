using InventoryDemo.Crosscutting;
using InventoryDemo.Domain.Models;
using InventoryDemo.Infrastructure.Persistance.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Infrastructure.Persistance.Repositories.Products
{
    public class ProductRepository : Repository<Product, InventoryContext>, IProductRepository
    {
        public ProductRepository(InventoryContext context) : base(context) { }

        public async Task<ProductDto> GetProduct(int productId, CancellationToken cancellationToken = default) =>
            await _context.Products.AsNoTracking().Where(product => product.ProductId == productId)
                .Select(product => new ProductDto(product.ProductId, product.Name, product.Code, product.Description, product.PricePerUnit, product.MinimumRequired)).FirstOrDefaultAsync(cancellationToken);

        public async Task<IEnumerable<ProductTableDto>> GetProducts(int skip, int take, CancellationToken cancellationToken = default) =>
            await _context.Products.AsNoTracking().Where(product => !product.Deleted).OrderBy(product => product.Name)
                .Select(product => new ProductTableDto(product.ProductId, product.Name, product.Code, product.Description, product.PricePerUnit, product.MinimumRequired)).Skip(skip).Take(take).ToListAsync(cancellationToken);

        public Task<int> GetTotalProducts(CancellationToken cancellationToken = default) =>
            _context.Products.AsNoTracking().Where(product => !product.Deleted).CountAsync(cancellationToken);

        public async Task<IEnumerable<Product>> GetByOrder(int orderId, CancellationToken cancellationToken = default) =>
            await _context.Products.Where(product => product.OrderProducts.Any(orderProducts => orderProducts.OrderId == orderId)).ToListAsync(cancellationToken);
    }
}
