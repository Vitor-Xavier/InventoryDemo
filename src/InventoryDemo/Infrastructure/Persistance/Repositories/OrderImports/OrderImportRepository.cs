using InventoryDemo.Crosscutting;
using InventoryDemo.Domain.Models;
using InventoryDemo.Infrastructure.Persistance.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Infrastructure.Persistance.Repositories.OrderImports
{
    public class OrderImportRepository : Repository<OrderImport, InventoryContext>, IOrderImportRepository
    {
        public OrderImportRepository(InventoryContext context) : base(context) { }

        public Task<OrderImportGetDto> GetOrderImportById(int orderImportId, CancellationToken cancellationToken = default) =>
            _context.OrderImports.Where(e => e.OrderImportId == orderImportId).Select(e => new OrderImportGetDto(e.OrderImportId, e.ImportStatus, e.ProcessingStarted, e.ProcessingEnded, e.UserId, e.User.Username)).FirstOrDefaultAsync(cancellationToken);
    }
}
