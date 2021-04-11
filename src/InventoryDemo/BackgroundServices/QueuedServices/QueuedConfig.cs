namespace InventoryDemo.BackgroundServices.QueuedServices
{
    public interface IQueuedConfig<T>
    {
        int ConcurrentTasks { get; set; }
    }

    public class QueuedConfig<T> : IQueuedConfig<T>
    {
        public int ConcurrentTasks { get; set; }
    }
}
