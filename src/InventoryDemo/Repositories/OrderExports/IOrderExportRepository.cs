using InventoryDemo.Crosscutting;
using InventoryDemo.Models;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Repositories.OrderExports
{
    public interface IOrderExportRepository : IRepository<OrderExport>
    {
        Task<OrderExportGetDto> GetOrderExportById(int orderExportId, CancellationToken cancellationToken = default);
    }
}
