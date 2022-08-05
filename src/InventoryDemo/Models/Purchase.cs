using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InventoryDemo.Models
{
    public class Purchase : BaseEntity
    {
        public int PurchaseId { get; set; }

        public int SupplierId { get; set; }

        public int ProductId { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal Quantity { get; set; }

        public DateTime Date { get; set; }

        [JsonIgnore]
        public virtual Product Product { get; set; }

        [JsonIgnore]
        public virtual Supplier Supplier { get; set; }

        public override bool Equals(object obj) => obj is Purchase purchase && purchase.PurchaseId == PurchaseId;

        public override int GetHashCode() => HashCode.Combine(PurchaseId);
    }
}
