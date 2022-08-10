using InventoryDemo.Crosscutting;
using InventoryDemo.Models;
using InventoryDemo.Services.Orders;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService) => _orderService = orderService;

        /// <summary>
        /// Busca paginada de Pedidos.
        /// </summary>
        /// <param name="skip">Quantidade de itens a pular</param>
        /// <param name="take">Quantidade de itens a retrair</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns>Tabela de Pedidos</returns>
        [HttpGet]
        public async Task<ActionResult<TableDto<OrderTableDto>>> GetOrders(int skip = 0, int take = 10, CancellationToken cancellationToken = default)
        {
            var suppliers = await _orderService.GetOrders(skip, take, cancellationToken);
            return Ok(suppliers);
        }

        /// <summary>
        /// Busca por determinado Pedido.
        /// </summary>
        /// <param name="orderId">Identificação do Pedido</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns>Pedido</returns>
        [HttpGet("{orderId:int}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int orderId, CancellationToken cancellationToken = default)
        {
            var suppliers = await _orderService.GetOrder(orderId, cancellationToken);
            return Ok(suppliers);
        }

        /// <summary>
        /// Insere Pedido.
        /// </summary>
        /// <param name="order">Dados do Pedido</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns>Dados do Pedido</returns>
        [HttpPost]
        public async Task<ActionResult<Order>> InsertOrder(Order order, CancellationToken cancellationToken = default)
        {
            await _orderService.CreateOrder(order, cancellationToken);
            return CreatedAtAction(nameof(GetOrder), new { orderId = order.OrderId }, order);
        }

        /// <summary>
        /// Altera dados de Pedido.
        /// </summary>
        /// <param name="orderId">Identificação do Pedido</param>
        /// <param name="order">Dados do Pedido</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        [HttpPut("{orderId:int}")]
        public async Task<IActionResult> UpdateOrder(int orderId, Order order, CancellationToken cancellationToken = default)
        {
            await _orderService.UpdateOrder(orderId, order, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Remove Pedido.
        /// </summary>
        /// <param name="orderId">Identificação do Pedido</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        [HttpDelete("{orderId:int}")]
        public async Task<IActionResult> DeleteOrder(int orderId, CancellationToken cancellationToken = default)
        {
            await _orderService.DeleteOrder(orderId, cancellationToken);
            return NoContent();
        }
    }
}
