using InventoryDemo.Crosscutting;
using InventoryDemo.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Infrastructure.Persistance.Repositories.OrderImports
{
    public interface IOrderImportRepository : IRepository<OrderImport>
    {
        Task<OrderImportGetDto> GetOrderImportById(int orderImportId, CancellationToken cancellationToken = default);
    }
}
