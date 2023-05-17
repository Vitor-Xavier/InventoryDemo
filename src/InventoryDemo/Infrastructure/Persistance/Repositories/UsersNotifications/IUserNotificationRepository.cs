using InventoryDemo.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Infrastructure.Persistance.Repositories.UsersNotifications
{
    public interface IUserNotificationRepository : IRepository<UserNotification>
    {
        Task BatchReadByUser(int userId, CancellationToken cancellationToken = default);
    }
}
