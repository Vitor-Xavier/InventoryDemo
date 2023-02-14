using InventoryDemo.Crosscutting.Events;
using InventoryDemo.Domain.Models;
using InventoryDemo.Services.CancellationHashs.OrderExports;
using InventoryDemo.Services.Notifications;
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
            var orderExport = new OrderExport { ExportStatus = OrderExportStatus.Processing, UserId = context.Message.UserId, ProcessingStarted = DateTime.Now };
            try
            {
                await orderExportService.UpdateOrderExport(context.Message.OrderExportId, orderExport, cancellationToken);

                string url = await orderExportService.ExportOrders(context.Message.DataFormat, cancellationToken);

                orderExport.Url = url;
                orderExport.ProcessingEnded = DateTime.Now;
                orderExport.DataFormat = context.Message.DataFormat;
                orderExport.ExportStatus = OrderExportStatus.Processed;
                await orderExportService.UpdateOrderExport(context.Message.OrderExportId, orderExport);

                var notificationService = _serviceProvider.GetService<INotificationService>();
                await notificationService.SendPrivateNotification(context.Message.Username, "Exportação de pedidos", "Exportação de pedidos finalizada", NotificationType.Success, "", cancellationToken);
            }
            catch (TaskCanceledException)
            {
                orderExport.ExportStatus = OrderExportStatus.Cancelled;
                await orderExportService.UpdateOrderExport(context.Message.OrderExportId, orderExport);

                var notificationService = _serviceProvider.GetService<INotificationService>();
                await notificationService.SendPrivateNotification(context.Message.Username, "Exportação de pedidos", "Exportação de pedidos cancelada", NotificationType.Information, "", cancellationToken);
            }
            catch (OperationCanceledException)
            {
                orderExport.ExportStatus = OrderExportStatus.Cancelled;
                await orderExportService.UpdateOrderExport(context.Message.OrderExportId, orderExport);

                var notificationService = _serviceProvider.GetService<INotificationService>();
                await notificationService.SendPrivateNotification(context.Message.Username, "Exportação de pedidos", "Exportação de pedidos cancelada", NotificationType.Information, "", cancellationToken);
            }
            catch (Exception)
            {
                orderExport.ExportStatus = OrderExportStatus.Error;
                await orderExportService.UpdateOrderExport(context.Message.OrderExportId, orderExport);

                var notificationService = _serviceProvider.GetService<INotificationService>();
                await notificationService.SendPrivateNotification(context.Message.Username, "Exportação de pedidos", "Erro na exportação de pedidos", NotificationType.Error, "", cancellationToken);
            }
        }
    }
}
