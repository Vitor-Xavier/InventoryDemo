using System;
using System.ComponentModel.DataAnnotations.Schema;

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

        public virtual Product Product { get; set; }

        public virtual Supplier Supplier { get; set; }

        public override bool Equals(object obj) => obj is Purchase purchase && purchase.PurchaseId == PurchaseId;

        public override int GetHashCode() => HashCode.Combine(PurchaseId);
    }
}
