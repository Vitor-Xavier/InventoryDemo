using InventoryDemo.Domain.Models;
using InventoryDemo.Infrastructure.Persistance.Context;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Infrastructure.Persistance.Repositories.Confirmations
{
    public class ConfirmationRepository : Repository<Confirmation, InventoryContext>, IConfirmationRepository
    {
        public ConfirmationRepository(InventoryContext context) : base(context) { }

        public ValueTask<Confirmation> GetConfirmation(Guid code, CancellationToken cancellationToken = default) =>
            _context.Confirmations.FindAsync(new object[] { code }, cancellationToken: cancellationToken);
    }
}
