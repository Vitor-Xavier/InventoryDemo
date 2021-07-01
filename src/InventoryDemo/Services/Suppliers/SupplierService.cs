using InventoryDemo.Crosscutting;
using InventoryDemo.Models;
using InventoryDemo.Repositories.Suppliers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Services.Suppliers
{
    public class SupplierService : ISupplierService
    {
        public readonly ISupplierRepository _productRepository;

        public SupplierService(ISupplierRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public Task<SupplierDto> GetSupplier(int supplierId, CancellationToken cancellationToken = default) =>
            _productRepository.GetSupplier(supplierId, cancellationToken);

        public Task<IEnumerable<SupplierTableDto>> GetSuppliers(int skip, int take, CancellationToken cancellationToken = default) =>
            _productRepository.GetSuppliers(skip, take, cancellationToken);

        public async Task CreateSupplier(Supplier supplier, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!IsValid(supplier)) throw new Exception("Registro inválido");

            await _productRepository.Add(supplier, cancellationToken);
        }

        public async Task UpdateSupplier(int productId, Supplier supplier, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!IsValid(supplier)) throw new Exception("Registro inválido");
            supplier.SupplierId = productId;

            await _productRepository.Edit(supplier, cancellationToken);
        }

        public async Task DeleteSupplier(int productId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Supplier supplier = new() { SupplierId = productId, Deleted = true };

            await _productRepository.Delete(supplier, cancellationToken);
        }

        public bool IsValid(Supplier supplier) => supplier is { Name: { Length: > 0 } };
    }
}
