using InventoryDemo.Context;
using InventoryDemo.Crosscutting;
using InventoryDemo.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Repositories.OrderExports
{
    public class OrderImportRepository : Repository<OrderImport, InventoryContext>, IOrderImportRepository
    {
        public OrderImportRepository(InventoryContext context) : base(context) { }

        public Task<OrderImportGetDto> GetOrderImportById(int orderImportId, CancellationToken cancellationToken = default) =>
            _context.OrderImports.Where(e => e.OrderImportId == orderImportId).Select(e => new OrderImportGetDto(e.OrderImportId, e.ImportStatus, e.ProcessingStarted, e.ProcessingEnded)).FirstOrDefaultAsync(cancellationToken);
    }
}
