using InventoryDemo.Crosscutting;
using InventoryDemo.Domain.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Infrastructure.Repositories.Suppliers
{
    public interface ISupplierRepository : IRepository<Supplier>
    {
        Task<SupplierDto> GetSupplier(int supplierId, CancellationToken cancellationToken = default);

        Task<int> GetTotalSuppliers(CancellationToken cancellationToken = default);

        Task<IEnumerable<SupplierTableDto>> GetSuppliers(int skip, int take, CancellationToken cancellationToken = default);
    }
}
