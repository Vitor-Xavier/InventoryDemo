namespace InventoryDemo.Domain.Models
{
    public enum OrderExportStatus
    {
        None,
        Waiting,
        Processing,
        Processed,
        Cancelled,
        Error
    }
}
