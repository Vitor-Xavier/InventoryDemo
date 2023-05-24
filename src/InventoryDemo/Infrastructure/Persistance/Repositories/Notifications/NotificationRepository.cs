using InventoryDemo.Crosscutting;
using InventoryDemo.Domain.Models;
using InventoryDemo.Infrastructure.Persistance.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Infrastructure.Persistance.Repositories.Notifications
{
    public class NotificationRepository : Repository<Notification, InventoryContext>, INotificationRepository
    {
        public NotificationRepository(InventoryContext context) : base(context) { }

        public async Task<IEnumerable<NotificationListDto>> GetNotificationsByUsername(string username, int skip, int take, CancellationToken cancellationToken = default) =>
            await _context.UserNotifications.AsNoTracking().Where(userNotification => !userNotification.Deleted && userNotification.User.Username == username).OrderByDescending(notification => notification.CreatedAt)
                .Select(userNotification => new NotificationListDto(userNotification.NotificationId, userNotification.Notification.Title, userNotification.Notification.Content, userNotification.Notification.Type, userNotification.Notification.Route, userNotification.ReadAt)).Skip(skip).Take(take).ToListAsync(cancellationToken);

        public Task<int> GetTotalNotificationsByUsername(string username, CancellationToken cancellationToken = default) =>
            _context.UserNotifications.AsNoTracking().Where(userNotification => userNotification.User.Username == username).CountAsync(cancellationToken);

        public Task<int> GetUnreadNotificationCountByUsername(string username, CancellationToken cancellationToken = default) =>
            _context.UserNotifications.AsNoTracking().Where(userNotification => userNotification.User.Username == username && userNotification.ReadAt == null).CountAsync(cancellationToken);
    }
}
