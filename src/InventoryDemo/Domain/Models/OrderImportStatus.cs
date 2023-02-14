namespace InventoryDemo.Domain.Models
{
    public enum OrderImportStatus
    {
        None,
        Waiting,
        Processing,
        Processed,
        Cancelled,
        Error
    }
}
