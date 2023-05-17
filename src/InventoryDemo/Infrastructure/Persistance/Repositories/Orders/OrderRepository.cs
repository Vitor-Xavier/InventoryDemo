using EFCore.BulkExtensions;
using InventoryDemo.Crosscutting;
using InventoryDemo.Domain.Models;
using InventoryDemo.Infrastructure.Persistance.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Infrastructure.Persistance.Repositories.Orders
{
    public class OrderRepository : Repository<Order, InventoryContext>, IOrderRepository
    {
        public OrderRepository(InventoryContext context) : base(context) { }

        public IAsyncEnumerable<OrderDto> GetOrders() =>
            _context.Orders.AsNoTracking().Select(order => new OrderDto(order.OrderId, order.Date, order.Note)).AsAsyncEnumerable();

        public IAsyncEnumerable<OrderExportDto> GetOrdersWithProducts() =>
            _context.Orders.AsNoTracking().Select(o => new OrderExportDto
            {
                OrderId = o.OrderId,
                Date = o.Date,
                Note = o.Note,
                Products = o.OrderProducts.Select(p => new ProductOrderDto(p.ProductId, p.Product.Name, p.Product.Code, p.Quantity, p.Product.PricePerUnit, p.Quantity * p.Product.PricePerUnit))
            }).AsAsyncEnumerable();

        public Task<OrderDto> GetOrder(int orderId, CancellationToken cancellationToken = default) =>
            _context.Orders.AsNoTracking().Where(order => order.OrderId == orderId).Select(order => new OrderDto(order.OrderId, order.Date, order.Note)).FirstOrDefaultAsync(cancellationToken);

        public async Task<IEnumerable<OrderTableDto>> GetOrders(int skip, int take, CancellationToken cancellationToken = default) =>
            await _context.Orders.AsNoTracking().OrderBy(order => order.OrderId).Select(order => new OrderTableDto(order.OrderId, order.Date, order.Note)).Skip(skip).Take(take).ToListAsync(cancellationToken);

        public async Task BulkInsert(IList<Order> orders, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                var transaction = _context.Database.BeginTransaction();
                try
                {
                    var bulkConfig = new BulkConfig { IncludeGraph = true, BatchSize = 2000, BulkCopyTimeout = 240, SqlBulkCopyOptions = SqlBulkCopyOptions.TableLock };
                    await _context.BulkInsertAsync(orders, bulkConfig, cancellationToken: cancellationToken);

                    await transaction.CommitAsync(cancellationToken);
                }
                catch (Exception e)
                {
                    try
                    {
                        await transaction.RollbackAsync();
                    }
                    catch
                    {
                        throw e;
                    }
                }
            });
        }
    }
}
