using InventoryDemo.Crosscutting;
using InventoryDemo.Models;
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
        /// <returns>Dados da Exportação de Pedidos</returns>
        [HttpGet("{orderExportId:int}")]
        public async Task<ActionResult<OrderExportGetDto>> GetOrderExport(int orderExportId, CancellationToken cancellationToken = default)
        {
            var orderExport = await _orderExportService.GetOrderExport(orderExportId, cancellationToken);
            return Ok(orderExport);
        }

        /// <summary>
        /// Requere exportação Pedido.
        /// </summary>
        /// <param name="dataFormat">Formato da exportação</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns>Dados da Exportação de Pedidos</returns>
        [HttpPost("{dataFormat}")]
        public async Task<ActionResult<OrderExport>> CreateOrderExport(DataFormat dataFormat, CancellationToken cancellationToken = default)
        {
            var orderExport = await _orderExportService.CreateOrderExport(dataFormat, cancellationToken);
            return AcceptedAtAction(nameof(GetOrderExport), new { orderExportId = orderExport.OrderExportId }, orderExport);
        }

        /// <summary>
        /// Cancela exportação de Pedidos.
        /// </summary>
        /// <param name="orderExportId">Identificação da exportação de Pedidos</param>
        [HttpDelete("{orderExportId:int}")]
        public IActionResult CancelOrderExport(int orderExportId)
        {
            _orderExportService.CancelOrderExport(orderExportId);
            return AcceptedAtAction(nameof(GetOrderExport), new { orderExportId });
        }
    }
}
