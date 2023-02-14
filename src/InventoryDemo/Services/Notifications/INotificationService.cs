using InventoryDemo.Crosscutting;
using InventoryDemo.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Services.Notifications
{
    public interface INotificationService
    {
        Task<TableDto<NotificationListDto>> GetNotifications(int skip, int take, CancellationToken cancellationToken = default);

        Task<int> GetUnreadNotificationCount(CancellationToken cancellationToken = default);

        Task ReadNotification(int notificationId, CancellationToken cancellationToken = default);

        Task ReadAllNotifications(CancellationToken cancellationToken = default);

        public Task SendNotification(NotificationDto notification, CancellationToken cancellationToken = default);

        public Task SendNotification(string title, string content, NotificationType type, string route, CancellationToken cancellationToken = default);

        public Task SendPrivateNotification(PrivateNotificationDto notification, CancellationToken cancellationToken = default);

        public Task SendPrivateNotification(string to, string title, string content, NotificationType type, string route, CancellationToken cancellationToken = default);
    }
}
