using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InventoryDemo.Domain.Models
{
    public class User : BaseEntity
    {
        public int UserId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }

        public string ProfileImage { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserNotification> UserNotifications { get; set; }

        public override bool Equals(object obj) => obj is User user && user.UserId == UserId;

        public override int GetHashCode() => HashCode.Combine(UserId);
    }
}
