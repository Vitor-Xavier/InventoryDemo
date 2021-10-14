using InventoryDemo.Crosscutting;
using InventoryDemo.Models;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Repositories.OrderExports
{
    public interface IOrderImportRepository : IRepository<OrderImport>
    {
        Task<OrderImportGetDto> GetOrderImportById(int orderImportId, CancellationToken cancellationToken = default);
    }
}
