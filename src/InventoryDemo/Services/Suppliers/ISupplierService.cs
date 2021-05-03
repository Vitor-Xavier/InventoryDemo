﻿using InventoryDemo.Crosscutting;
using InventoryDemo.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Services.Suppliers
{
    public interface ISupplierService
    {
        Task<IEnumerable<SupplierTableDto>> GetSuppliers(int skip, int take, CancellationToken cancellationToken = default);

        Task CreateSupplier(Supplier supplier, CancellationToken cancellationToken = default);

        Task UpdateSupplier(int supplierId, Supplier supplier, CancellationToken cancellationToken = default);

        Task DeleteSupplier(int supplierId, CancellationToken cancellationToken = default);
    }
}