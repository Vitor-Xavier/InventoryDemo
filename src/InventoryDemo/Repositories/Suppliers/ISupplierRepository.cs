using InventoryDemo.Crosscutting;
using InventoryDemo.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Repositories.Suppliers
{
    public interface ISupplierRepository : IRepository<Supplier>
    {
        Task<SupplierDto> GetSupplier(int supplierId, CancellationToken cancellationToken = default);

        Task<IEnumerable<SupplierTableDto>> GetSuppliers(int skip, int take, CancellationToken cancellationToken = default);
    }
}
