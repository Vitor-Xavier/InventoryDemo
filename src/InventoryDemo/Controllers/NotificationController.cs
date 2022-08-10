using InventoryDemo.Crosscutting;
using InventoryDemo.Services.Notifications;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService) => _notificationService = notificationService;

        /// <summary>
        /// Busca notificações do usuário logado.
        /// </summary>
        /// <param name="skip">Quantidade de itens a pular</param>
        /// <param name="take">Quantidade de itens a retrair</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns>Lista de Notificações</returns>
        [HttpGet]
        public async Task<ActionResult<TableDto<NotificationListDto>>> GetNotifications(int skip = 0, int take = 10, CancellationToken cancellationToken = default)
        {
            var notifications = await _notificationService.GetNotifications(skip, take, cancellationToken);
            return Ok(notifications);
        }

        /// <summary>
        /// Busca total de notificações não lidas do usuário logado.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        /// <returns>Total de Notificações não lidas</returns>
        [HttpGet("unread/count")]
        public async Task<ActionResult<int>> GetUnreadNotificationCount(CancellationToken cancellationToken = default)
        {
            var notifications = await _notificationService.GetUnreadNotificationCount(cancellationToken);
            return Ok(notifications);
        }

        /// <summary>
        /// Marca notificação como lida.
        /// </summary>
        /// <param name="notificationId">Identificação da notificação</param>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        [HttpPut("{notificationId:int}/read")]
        public async Task<IActionResult> UpdateReadNotification(int notificationId, CancellationToken cancellationToken = default)
        {
            await _notificationService.ReadNotification(notificationId, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Marca todas as do usuário logado como lidas.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento da requisição</param>
        [HttpPut("read")]
        public async Task<IActionResult> UpdateReadNotification(CancellationToken cancellationToken = default)
        {
            await _notificationService.ReadAllNotifications(cancellationToken);
            return NoContent();
        }

        [HttpPost("Send")]
        public async Task<IActionResult> SendNotification(NotificationDto message, CancellationToken cancellationToken = default)
        {
            await _notificationService.SendNotification(message.Title, message.Content, message.Type, message.Route, cancellationToken);
            return NoContent();
        }

        [HttpPost("Private/Send")]
        public async Task<IActionResult> SendPrivateNotification(PrivateNotificationDto message, CancellationToken cancellationToken = default)
        {
            await _notificationService.SendPrivateNotification(message.To, message.Title, message.Content, message.Type, message.Route, cancellationToken);
            return NoContent();
        }
    }
}
