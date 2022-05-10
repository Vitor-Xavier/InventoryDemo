using InventoryDemo.Models;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Repositories.UsersNotifications
{
    public interface IUserNotificationRepository : IRepository<UserNotification>
    {
        Task BatchReadByUser(int userId, CancellationToken cancellationToken = default);
    }
}
