using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryDemo.Models
{
    public class OrderProduct : BaseEntity, ICloneable
    {
        public int OrderId { get; set; }

        public int ProductId { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal Quantity { get; set; }

        public Order Order { get; set; }

        public Product Product { get; set; }

        public object Clone() => (OrderProduct)MemberwiseClone();

        public override bool Equals(object obj) => obj is OrderProduct orderProduct && orderProduct.OrderId == OrderId && orderProduct.ProductId == ProductId;

        public override int GetHashCode() => HashCode.Combine(OrderId, ProductId);
    }
}
