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
        public async Task<IActionResult> GetOrders(int skip = 0, int take = 10, CancellationToken cancellationToken = default)
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
        public async Task<IActionResult> GetOrder(int orderId, CancellationToken cancellationToken = default)
        {
            var suppliers = await _orderService.GetOrder(orderId, cancellationToken);
            return Ok(suppliers);
        }

        /// <summary>
        /// Insere Pedido.
        /// </summary>
        /// <param name="product">Dados do Produto</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns>Dados do Produto</returns>
        [HttpPost]
        public async Task<IActionResult> InsertOrder(Order product, CancellationToken cancellationToken = default)
        {
            await _orderService.CreateOrder(product, cancellationToken);
            return Created(nameof(ProductController), product);
        }

        /// <summary>
        /// Altera dados de Pedido.
        /// </summary>
        /// <param name="productId">Identificação do Pedido</param>
        /// <param name="product">Dados do Pedido</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        [HttpPut("{productId:int}")]
        public async Task<IActionResult> UpdateOrder(int productId, Order product, CancellationToken cancellationToken = default)
        {
            await _orderService.UpdateOrder(productId, product, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Remove Pedido.
        /// </summary>
        /// <param name="productId">Identificação do Pedido</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        [HttpDelete("{productId:int}")]
        public async Task<IActionResult> DeleteOrder(int productId, CancellationToken cancellationToken = default)
        {
            await _orderService.DeleteOrder(productId, cancellationToken);
            return NoContent();
        }
    }
}
