using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.BackgroundServices.QueuedServices
{
    public class QueuedHostedService : BackgroundService
	{
		private readonly ILogger<QueuedHostedService> _logger;

		private readonly int _concurrentTasks;

		public IBackgroundTaskQueue TaskQueue { get; }

		public QueuedHostedService(IQueuedConfig<QueuedHostedService> config, 
								   IBackgroundTaskQueue taskQueue,
								   ILogger<QueuedHostedService> logger)
		{
			TaskQueue = taskQueue;
			_logger = logger;
			_concurrentTasks = config.ConcurrentTasks;
		}

		protected override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation($"Queued Hosted Service is running.");

			await BackgroundProcessing(cancellationToken);
		}

		private async Task BackgroundProcessing(CancellationToken cancellationToken)
		{
			var semaphore = new SemaphoreSlim(_concurrentTasks);

			void HandleTask(Task task) => semaphore.Release();

			while (!cancellationToken.IsCancellationRequested)
			{
				await semaphore.WaitAsync(cancellationToken);
				var item = await TaskQueue.DequeueAsync(cancellationToken);

				var task = item(cancellationToken);
				_ = task.ContinueWith(HandleTask, cancellationToken);
			}
		}

		public override async Task StopAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("Queued Hosted Service is stopping.");

			await base.StopAsync(cancellationToken);
		}
	}
}
