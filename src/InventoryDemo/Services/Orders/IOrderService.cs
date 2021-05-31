using InventoryDemo.Crosscutting;
using InventoryDemo.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Services.Orders
{
    public interface IOrderService
    {
        Task UpdateCacheOrders();

        Task<IEnumerable<OrderTableDto>> GetOrders(int skip, int take, CancellationToken cancellationToken = default);

        Task<OrderDto> GetOrder(int orderId, CancellationToken cancellationToken = default);

        Task CreateOrder(Order order, CancellationToken cancellationToken = default);

        Task UpdateOrder(int orderId, Order order, CancellationToken cancellationToken = default);

        Task DeleteOrder(int orderId, CancellationToken cancellationToken = default);
    }
}
