using InventoryDemo.Crosscutting;
using InventoryDemo.Services.Confirmations;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfirmationController : ControllerBase
    {
        private readonly IConfirmationService _confirmationService;

        public ConfirmationController(IConfirmationService confirmationService) => _confirmationService = confirmationService;

        /// <summary>
        /// Requere importação de Pedido.
        /// </summary>
        /// <param name="codeChallenge">Código para Verificação</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns>Dados do Produto</returns>
        [HttpPost]
        public async Task<ActionResult<ConfirmationResponseDto>> CreateOrderExport(string codeChallenge, CancellationToken cancellationToken = default)
        {
            var confirmation = await _confirmationService.CreateConfirmation(codeChallenge, cancellationToken);
            return CreatedAtAction(null, confirmation);
        }
    }
}
