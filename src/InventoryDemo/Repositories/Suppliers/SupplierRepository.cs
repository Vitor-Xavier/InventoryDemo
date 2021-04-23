using InventoryDemo.Context;
using InventoryDemo.Crosscutting;
using InventoryDemo.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Repositories.Suppliers
{
    public class SupplierRepository : Repository<Supplier, InventoryContext>, ISupplierRepository
    {
        public SupplierRepository(InventoryContext context) : base(context) { }

        public async Task<IEnumerable<SupplierTableDto>> GetSuppliers(int skip, int take, CancellationToken cancellationToken = default) =>
            await _context.Suppliers.AsNoTracking().OrderBy(supplier => supplier.Name).Select(supplier => new SupplierTableDto(supplier.SupplierId, supplier.Name)).Skip(skip).Take(take).ToListAsync(cancellationToken);
    }
}
