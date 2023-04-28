using System;
using System.Text.Json.Serialization;

namespace InventoryDemo.Domain.Models
{
    public class Confirmation : BaseEntity
    {
        public Guid ConfirmationId { get; set; }

        public string CodeVerifier { get; set; }

        public ConfirmationStatus State { get; set; }

        public DateTime ExpiresAt { get; set; }

        public int UserId { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }

        public override bool Equals(object obj) => obj is Confirmation confirmation && confirmation.ConfirmationId == ConfirmationId;

        public override int GetHashCode() => HashCode.Combine(ConfirmationId);
    }
}
