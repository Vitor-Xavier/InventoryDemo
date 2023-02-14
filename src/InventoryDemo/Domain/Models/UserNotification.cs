using System;
using System.Text.Json.Serialization;

namespace InventoryDemo.Domain.Models
{
    public class UserNotification : BaseEntity
    {
        public int NotificationId { get; set; }

        public int UserId { get; set; }

        public DateTime? ReadAt { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }

        [JsonIgnore]
        public virtual Notification Notification { get; set; }

        public override bool Equals(object obj) => obj is UserNotification userNotification && userNotification.NotificationId == NotificationId && userNotification.UserId == UserId;

        public override int GetHashCode() => HashCode.Combine(NotificationId, UserId);
    }
}
