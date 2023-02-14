using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace InventoryDemo.Domain.Models
{
    public class Order : BaseEntity, ICloneable
    {
        public int OrderId { get; set; }

        public DateTime Date { get; set; }

        public string Note { get; set; }

        [JsonIgnore]
        public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new HashSet<OrderProduct>();

        public object Clone() => MemberwiseClone();

        public override bool Equals(object obj) => obj is Order order && order.OrderId == OrderId;

        public override int GetHashCode() => HashCode.Combine(OrderId);
    }
}
