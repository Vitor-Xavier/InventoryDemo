using EFCore.BulkExtensions;
using InventoryDemo.Context;
using InventoryDemo.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Repositories.UsersNotifications
{
    public class UserNotificationRepository : Repository<UserNotification, InventoryContext>, IUserNotificationRepository
    {
        public UserNotificationRepository(InventoryContext context) : base(context) { }

        public async Task BatchReadByUser(int userId, CancellationToken cancellationToken = default)
        {
            await _context.UserNotifications.Where(u => u.UserId == userId).BatchUpdateAsync(a => new UserNotification { ReadAt = DateTime.Now }, cancellationToken: cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
