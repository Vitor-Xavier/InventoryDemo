using System;
using System.Collections.Generic;

namespace InventoryDemo.Models
{
    public class Order : BaseEntity
    {
        public int OrderId { get; set; }

        public DateTime Date { get; set; }

        public string Note { get; set; }

        public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new HashSet<OrderProduct>();

        public override bool Equals(object obj) => obj is Order order && order.OrderId == OrderId;

        public override int GetHashCode() => HashCode.Combine(OrderId);
    }
}
