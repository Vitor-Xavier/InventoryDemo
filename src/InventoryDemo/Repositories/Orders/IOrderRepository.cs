using InventoryDemo.Crosscutting;
using InventoryDemo.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Repositories.Orders
{
    public interface IOrderRepository : IRepository<Order>
    {
        IAsyncEnumerable<OrderDto> GetOrders();

        Task<OrderDto> GetOrder(int orderId, CancellationToken cancellationToken = default);

        Task<IEnumerable<OrderTableDto>> GetOrders(int skip, int take, CancellationToken cancellationToken = default);
    }
}
