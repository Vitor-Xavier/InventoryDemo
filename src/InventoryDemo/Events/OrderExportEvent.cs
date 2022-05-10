﻿using InventoryDemo.Crosscutting;

namespace InventoryDemo.Events
{
    public class OrderExportEvent
    {
        public int OrderExportId { get; set; }

        public int UserId { get; set; }

        public string Username { get; set; }

        public DataFormat DataFormat { get; set; }
    }
}
