using InventoryDemo.Services.OrderExports;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderExportController : ControllerBase
    {
        private readonly IOrderExportService _orderExportService;

        public OrderExportController(IOrderExportService orderExportService) => _orderExportService = orderExportService;

        /// <summary>
        /// Busca exportação de Pedido.
        /// </summary>
        /// <param name="orderExportId">Identificação da exportação de Pedidos</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns>Dados do Produto</returns>
        [HttpGet("{orderExportId:int}")]
        public async Task<IActionResult> GetOrderExport(int orderExportId, CancellationToken cancellationToken = default)
        {
            var orderExport = await _orderExportService.GetOrderExport(orderExportId, cancellationToken);
            return Ok(orderExport);
        }

        /// <summary>
        /// Requere exportação Pedido.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns>Dados do Produto</returns>
        [HttpPost]
        public async Task<IActionResult> InsertOrderExport(CancellationToken cancellationToken = default)
        {
            var orderExport = await _orderExportService.CreateOrderExport(cancellationToken);
            return Created(nameof(OrderExportController), orderExport);
        }

        /// <summary>
        /// Cancela exportação de Pedidos.
        /// </summary>
        /// <param name="orderExportId">Identificação da exportação de Pedidos</param>
        [HttpDelete("{orderExportId:int}")]
        public IActionResult CancelOrderExport(int orderExportId)
        {
            _orderExportService.CancelOrderExport(orderExportId);
            return NoContent();
        }
    }
}
