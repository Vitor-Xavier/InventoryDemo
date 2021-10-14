using InventoryDemo.Crosscutting;
using InventoryDemo.Models;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Services.OrderExports
{
    public interface IOrderImportService
    {
        Task ImportOrders(string path, DataFormat dataFormat, CancellationToken cancellationToken = default);

        Task<OrderImportGetDto> GetOrderImport(int orderImportId, CancellationToken cancellationToken = default);

        Task<OrderImport> CreateOrderImport(IFormFile dataFile, DataFormat dataFormat, CancellationToken cancellationToken = default);

        Task UpdateOrderImport(int orderImportId, OrderImport orderImport, CancellationToken cancellationToken = default);

        void CancelOrderImport(int orderImportId);
    }
}
