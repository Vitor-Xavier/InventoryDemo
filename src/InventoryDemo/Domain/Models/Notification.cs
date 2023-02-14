using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InventoryDemo.Domain.Models
{
    public class Notification : BaseEntity
    {
        public int NotificationId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Content { get; set; }

        public NotificationType Type { get; set; }

        public string Route { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserNotification> UsersNotification { get; set; }

        public override bool Equals(object obj) => obj is Notification notification && notification.NotificationId == NotificationId;

        public override int GetHashCode() => HashCode.Combine(NotificationId);
    }
}
