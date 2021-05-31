using InventoryDemo.Context;
using InventoryDemo.Crosscutting;
using InventoryDemo.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Repositories.OrderExports
{
    public class OrderExportRepository : Repository<OrderExport, InventoryContext>, IOrderExportRepository
    {
        public OrderExportRepository(InventoryContext context) : base(context) { }

        public Task<OrderExportGetDto> GetOrderExportById(int orderExportId, CancellationToken cancellationToken = default) =>
            _context.OrderExports.Where(e => e.OrderExportId == orderExportId).Select(e => new OrderExportGetDto(e.OrderExportId, e.ExportStatus, e.ProcessingStarted, e.ProcessingEnded)).FirstOrDefaultAsync(cancellationToken);
    }
}
