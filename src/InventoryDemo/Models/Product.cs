using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InventoryDemo.Models
{
    public class Product : BaseEntity
    {
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        [Column(TypeName = "decimal(9, 2)")]
        public decimal PricePerUnit { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal MinimumRequired { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal Quantity { get; set; }

        [JsonIgnore]
        public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new HashSet<OrderProduct>();

        public override bool Equals(object obj) => obj is Product product && product.ProductId == ProductId;

        public override int GetHashCode() => HashCode.Combine(ProductId);
    }
}
