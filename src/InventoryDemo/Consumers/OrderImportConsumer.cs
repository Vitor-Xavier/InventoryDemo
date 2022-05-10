using InventoryDemo.Events;
using InventoryDemo.Models;
using InventoryDemo.Services.CancellationHashs.OrderImports;
using InventoryDemo.Services.Notifications;
using InventoryDemo.Services.OrderExports;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace InventoryDemo.Consumers
{
    public class OrderImportConsumer : IConsumer<OrderImportEvent>
    {
        private readonly IServiceProvider _serviceProvider;

        public OrderImportConsumer(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task Consume(ConsumeContext<OrderImportEvent> context)
        {
            var orderExportCancellationHash = _serviceProvider.GetService<IOrderImportCancellationHash>();
            var cancellationToken = orderExportCancellationHash.GetOrCreateCancellationToken(context.Message.OrderImportId);

            var orderImportService = _serviceProvider.GetService<IOrderImportService>();
            var orderImport = new OrderImport { ImportStatus = OrderImportStatus.Processing, UserId = context.Message.UserId, ProcessingStarted = DateTime.Now };
            try
            {
                await orderImportService.UpdateOrderImport(context.Message.OrderImportId, orderImport, cancellationToken);

                await orderImportService.ImportOrders(context.Message.Path, context.Message.DataFormat, cancellationToken);

                orderImport.ProcessingEnded = DateTime.Now;
                orderImport.DataFormat = context.Message.DataFormat;
                orderImport.ImportStatus = OrderImportStatus.Processed;
                await orderImportService.UpdateOrderImport(context.Message.OrderImportId, orderImport);

                var notificationService = _serviceProvider.GetService<INotificationService>();
                await notificationService.SendPrivateNotification(context.Message.Username, "Importação de pedidos", "Importação de pedidos finalizada", NotificationType.Success, "", cancellationToken);
            }
            catch (TaskCanceledException)
            {
                orderImport.ImportStatus = OrderImportStatus.Cancelled;
                await orderImportService.UpdateOrderImport(context.Message.OrderImportId, orderImport);

                var notificationService = _serviceProvider.GetService<INotificationService>();
                await notificationService.SendPrivateNotification(context.Message.Username, "Importação de pedidos", "Importação de pedidos finalizada", NotificationType.Information, "", cancellationToken);
            }
            catch (OperationCanceledException)
            {
                orderImport.ImportStatus = OrderImportStatus.Cancelled;
                await orderImportService.UpdateOrderImport(context.Message.OrderImportId, orderImport);

                var notificationService = _serviceProvider.GetService<INotificationService>();
                await notificationService.SendPrivateNotification(context.Message.Username, "Importação de pedidos", "Importação de pedidos finalizada", NotificationType.Information, "", cancellationToken);
            }
            catch (Exception)
            {
                orderImport.ImportStatus = OrderImportStatus.Error;
                await orderImportService.UpdateOrderImport(context.Message.OrderImportId, orderImport);

                var notificationService = _serviceProvider.GetService<INotificationService>();
                await notificationService.SendPrivateNotification(context.Message.Username, "Importação de pedidos", "Importação de pedidos finalizada", NotificationType.Error, "", cancellationToken);
            }
        }
    }
}
