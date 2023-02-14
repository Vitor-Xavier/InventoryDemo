using InventoryDemo.Crosscutting;
using InventoryDemo.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Infrastructure.Repositories.OrderExports
{
    public interface IOrderExportRepository : IRepository<OrderExport>
    {
        Task<OrderExportGetDto> GetOrderExportById(int orderExportId, CancellationToken cancellationToken = default);
    }
}
