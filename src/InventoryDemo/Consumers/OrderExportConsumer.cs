using InventoryDemo.Events;
using InventoryDemo.Services.Orders;
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
            var orderService = _serviceProvider.GetService<IOrderService>();
            string path = await orderService.ExportOrder();
        }
    }
}
