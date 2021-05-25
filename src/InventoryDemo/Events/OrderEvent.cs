using System;

namespace InventoryDemo.Events
{
    public class OrderEvent
    {
        public DateTime RequestedAt { get; set; }

        public bool ForceUpdate { get; set; }
    }
}
