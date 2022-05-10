using InventoryDemo.Crosscutting;
using System;

namespace InventoryDemo.Models
{
    public class OrderImport : BaseEntity
    {
        public int OrderImportId { get; set; }

        public DataFormat DataFormat { get; set; }

        public DateTime? ProcessingStarted { get; set; }

        public DateTime? ProcessingEnded { get; set; }

        public OrderImportStatus ImportStatus { get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; }

        public override bool Equals(object obj) => obj is OrderImport order && order.OrderImportId == OrderImportId;

        public override int GetHashCode() => HashCode.Combine(OrderImportId);
    }
}
