using InventoryDemo.Events;
using InventoryDemo.Models;
using InventoryDemo.Services.OrderExportCancellationHashs;
using InventoryDemo.Services.OrderExports;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace InventoryDemo.Consumers
{
    public class OrderExportConsumer : IConsumer<OrderExportEvent>
    {
        private readonly IServiceProvider _serviceProvider;

        public OrderExportConsumer(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task Consume(ConsumeContext<OrderExportEvent> context)
        {
            var orderExportCancellationHash = _serviceProvider.GetService<IOrderExportCancellationHash>();
            var cancellationToken = orderExportCancellationHash.GetOrCreateCancellationToken(context.Message.OrderExportId);

            var orderExportService = _serviceProvider.GetService<IOrderExportService>();
            var orderExport = new OrderExport { ExportStatus = OrderExportStatus.Processing, ProcessingStarted = DateTime.Now };
            try
            {
                await orderExportService.UpdateOrderExport(context.Message.OrderExportId, orderExport, cancellationToken);

                string url = await orderExportService.ExportOrders(cancellationToken);

                orderExport.ProcessingEnded = DateTime.Now;
                orderExport.ExportStatus = OrderExportStatus.Processed;
                await orderExportService.UpdateOrderExport(context.Message.OrderExportId, orderExport);
            }
            catch (TaskCanceledException)
            {
                orderExport.ExportStatus = OrderExportStatus.Cancelled;
                await orderExportService.UpdateOrderExport(context.Message.OrderExportId, orderExport);
            }
            catch (OperationCanceledException)
            {
                orderExport.ExportStatus = OrderExportStatus.Cancelled;
                await orderExportService.UpdateOrderExport(context.Message.OrderExportId, orderExport);
            }
            catch (Exception)
            {
                orderExport.ExportStatus = OrderExportStatus.Error;
                await orderExportService.UpdateOrderExport(context.Message.OrderExportId, orderExport);
            }
        }
    }
}
