using InventoryDemo.Domain.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Infrastructure.Persistance.Repositories.Confirmations
{
    public interface IConfirmationRepository : IRepository<Confirmation>
    {
        ValueTask<Confirmation> GetConfirmation(Guid code, CancellationToken cancellationToken = default);
    }
}
