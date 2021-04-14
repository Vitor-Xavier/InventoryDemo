using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryDemo.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }

        [Required]
        public string Name { get; set; }

        public override bool Equals(object obj) => obj is Supplier supplier && supplier.SupplierId == SupplierId;

        public override int GetHashCode() => HashCode.Combine(SupplierId);
    }
}
