using InventoryDemo.Crosscutting;
using InventoryDemo.Domain.Models;
using InventoryDemo.Infrastructure.Persistance.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Infrastructure.Persistance.Repositories.OrderExports
{
    public class OrderExportRepository : Repository<OrderExport, InventoryContext>, IOrderExportRepository
    {
        public OrderExportRepository(InventoryContext context) : base(context) { }

        public Task<OrderExportGetDto> GetOrderExportById(int orderExportId, CancellationToken cancellationToken = default) =>
            _context.OrderExports.Where(e => e.OrderExportId == orderExportId).Select(e => new OrderExportGetDto(e.OrderExportId, e.ExportStatus, e.ProcessingStarted, e.ProcessingEnded, e.UserId, e.User.Username)).FirstOrDefaultAsync(cancellationToken);
    }
}
