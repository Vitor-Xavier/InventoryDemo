using InventoryDemo.Crosscutting;
using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryDemo.Models
{
    public class OrderExport : BaseEntity
    {
        public int OrderExportId { get; set; }

        [StringLength(300)]
        public string Url { get; set; }

        public DataFormat DataFormat { get; set; }

        public DateTime? ProcessingStarted { get; set; }

        public DateTime? ProcessingEnded { get; set; }

        public OrderExportStatus ExportStatus { get; set; }

        public override bool Equals(object obj) => obj is OrderExport order && order.OrderExportId == OrderExportId;

        public override int GetHashCode() => HashCode.Combine(OrderExportId);
    }
}
