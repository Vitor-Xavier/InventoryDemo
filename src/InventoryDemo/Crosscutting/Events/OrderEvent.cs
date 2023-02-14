using System;

namespace InventoryDemo.Crosscutting.Events
{
    public class OrderEvent
    {
        public DateTime RequestedAt { get; set; }

        public bool ForceUpdate { get; set; }
    }
}
