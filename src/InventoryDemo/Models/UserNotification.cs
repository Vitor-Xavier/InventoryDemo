using System;

namespace InventoryDemo.Models
{
    public class UserNotification : BaseEntity
    {
        public int NotificationId { get; set; }

        public int UserId { get; set; }

        public DateTime? ReadAt { get; set; }

        public virtual User User { get; set; }

        public virtual Notification Notification { get; set; }

        public override bool Equals(object obj) => obj is UserNotification userNotification && userNotification.NotificationId == NotificationId && userNotification.UserId == UserId;

        public override int GetHashCode() => HashCode.Combine(NotificationId, UserId);
    }
}
