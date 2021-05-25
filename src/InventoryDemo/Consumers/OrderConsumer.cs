using InventoryDemo.Events;
using InventoryDemo.Services.Cache;
using InventoryDemo.Services.Orders;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace InventoryDemo.Consumers
{
    public class OrderConsumer : IConsumer<OrderEvent>
    {
        private readonly IServiceProvider _serviceProvider;

        public OrderConsumer(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task Consume(ConsumeContext<OrderEvent> context)
        {
            var cacheService = _serviceProvider.GetService<ICacheService>();
            var latestUpdate = await cacheService.GetCacheValue<DateTime>("latest.update:orders");

            if (latestUpdate >= context.Message.RequestedAt) return;

            if (context.Message.ForceUpdate || latestUpdate < DateTime.Now.AddHours(-1))
            {
                var orderService = _serviceProvider.GetService<IOrderService>();
                await orderService.UpdateCacheOrders();
            }
        }
    }
}
