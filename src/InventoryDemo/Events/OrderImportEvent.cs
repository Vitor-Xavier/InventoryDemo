using InventoryDemo.Crosscutting;

namespace InventoryDemo.Events
{
    public class OrderImportEvent
    {
        public int OrderImportId { get; set; }

        public string Path { get; set; }

        public DataFormat DataFormat { get; set; }
    }
}
