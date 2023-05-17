using InventoryDemo.Crosscutting;
using InventoryDemo.Domain.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Infrastructure.Persistance.Repositories.Notifications
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task<IEnumerable<NotificationListDto>> GetNotificationsByUsername(string username, int skip, int take, CancellationToken cancellationToken = default);

        Task<int> GetTotalNotificationsByUsername(string username, CancellationToken cancellationToken = default);

        Task<int> GetUnreadNotificationCountByUsername(string username, CancellationToken cancellationToken = default);
    }
}
