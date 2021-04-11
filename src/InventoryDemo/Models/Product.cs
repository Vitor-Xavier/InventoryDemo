using System;

namespace InventoryDemo.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal PricePerUnit { get; set; }

        public override bool Equals(object obj) => obj is Product product && product.ProductId == ProductId;

        public override int GetHashCode() => HashCode.Combine(ProductId);
    }
}
