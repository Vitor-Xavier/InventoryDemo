using InventoryDemo.Crosscutting;
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
        /// <param name="dataFormat">Formato da exportação</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns>Dados do Produto</returns>
        [HttpPost("{dataFormat}")]
        public async Task<IActionResult> InsertOrderExport(DataFormat dataFormat, CancellationToken cancellationToken = default)
        {
            var orderExport = await _orderExportService.CreateOrderExport(dataFormat, cancellationToken);
            return Accepted(orderExport);
        }

        /// <summary>
        /// Cancela exportação de Pedidos.
        /// </summary>
        /// <param name="orderExportId">Identificação da exportação de Pedidos</param>
        [HttpDelete("{orderExportId:int}")]
        public IActionResult CancelOrderExport(int orderExportId)
        {
            _orderExportService.CancelOrderExport(orderExportId);
            return Accepted();
        }
    }
}
