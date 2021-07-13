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

        public Task<SupplierDto> GetSupplier(int supplierId, CancellationToken cancellationToken = default) =>
            _context.Suppliers.AsNoTracking().Where(supplier => supplier.SupplierId == supplierId).Select(supplier => new SupplierDto(supplier.Name)).FirstOrDefaultAsync(cancellationToken);

        public Task<int> GetTotalSuppliers(CancellationToken cancellationToken = default) =>
            _context.Suppliers.AsNoTracking().Where(supplier => !supplier.Deleted).CountAsync(cancellationToken);

        public async Task<IEnumerable<SupplierTableDto>> GetSuppliers(int skip, int take, CancellationToken cancellationToken = default) =>
            await _context.Suppliers.AsNoTracking().Where(supplier => !supplier.Deleted).OrderBy(supplier => supplier.SupplierId).Select(supplier => new SupplierTableDto(supplier.SupplierId, supplier.Name)).Skip(skip).Take(take).ToListAsync(cancellationToken);
    }
}
