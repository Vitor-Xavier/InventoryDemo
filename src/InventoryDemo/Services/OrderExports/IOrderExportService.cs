using InventoryDemo.Crosscutting;
using InventoryDemo.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Services.OrderExports
{
    public interface IOrderExportService
    {
        Task<string> ExportOrders(DataFormat dataFormat, CancellationToken cancellationToken = default);

        Task<OrderExportGetDto> GetOrderExport(int orderExportId, CancellationToken cancellationToken = default);

        Task<OrderExport> CreateOrderExport(DataFormat dataFormat, CancellationToken cancellationToken = default);

        Task UpdateOrderExport(int orderExportId, OrderExport orderExport, CancellationToken cancellationToken = default);

        void CancelOrderExport(int orderExportId);
    }
}
