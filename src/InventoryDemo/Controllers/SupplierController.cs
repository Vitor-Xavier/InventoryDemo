using InventoryDemo.Crosscutting;
using InventoryDemo.Models;
using InventoryDemo.Services.Suppliers;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService) => _supplierService = supplierService;

        /// <summary>
        /// Busca paginada de Fornecedores.
        /// </summary>
        /// <param name="skip">Quantidade de itens a pular</param>
        /// <param name="take">Quantidade de itens a retrair</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns>Tabela de Fornecedores</returns>
        [HttpGet]
        public async Task<ActionResult<TableDto<SupplierTableDto>>> GetSuppliers(int skip = 0, int take = 10, CancellationToken cancellationToken = default)
        {
            var suppliers = await _supplierService.GetSuppliers(skip, take, cancellationToken);
            return Ok(suppliers);
        }

        /// <summary>
        /// Busca Fornecedor.
        /// </summary>
        /// <param name="supplierId">Identificador do Fornecedor</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns>Fornecedor</returns>
        [HttpGet("{supplierId:int}")]
        public async Task<ActionResult<SupplierDto>> GetSupplier(int supplierId, CancellationToken cancellationToken = default)
        {
            var suppliers = await _supplierService.GetSupplier(supplierId, cancellationToken);
            return Ok(suppliers);
        }

        /// <summary>
        /// Insere Fornecedor.
        /// </summary>
        /// <param name="supplier">Dados do Fornecedor</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns>Dados do Fornecedor</returns>
        [HttpPost]
        public async Task<ActionResult<Supplier>> CreateSupplier(Supplier supplier, CancellationToken cancellationToken = default)
        {
            await _supplierService.CreateSupplier(supplier, cancellationToken);
            return CreatedAtAction(nameof(GetSupplier), new { supplierId = supplier.SupplierId }, supplier);
        }

        /// <summary>
        /// Altera dados de Fornecedor.
        /// </summary>
        /// <param name="supplierId">Identificação do Fornecedor</param>
        /// <param name="supplier">Dados do Fornecedor</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        [HttpPut("{supplierId:int}")]
        public async Task<IActionResult> UpdateSupplier(int supplierId, Supplier supplier, CancellationToken cancellationToken = default)
        {
            await _supplierService.UpdateSupplier(supplierId, supplier, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Remove Fornecedor.
        /// </summary>
        /// <param name="supplierId">Identificação do Fornecedor</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        [HttpDelete("{supplierId:int}")]
        public async Task<IActionResult> DeleteSupplier(int supplierId, CancellationToken cancellationToken = default)
        {
            await _supplierService.DeleteSupplier(supplierId, cancellationToken);
            return NoContent();
        }
    }
}
