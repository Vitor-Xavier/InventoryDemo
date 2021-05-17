using InventoryDemo.Crosscutting;
using InventoryDemo.Models;
using InventoryDemo.Repositories.Orders;
using InventoryDemo.Services.Cache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Services.Orders
{
    public class OrderService : IOrderService
    {
        public readonly IOrderRepository _orderRepository;

        public readonly ICacheService _cacheService;

        public OrderService(IOrderRepository orderRepository, ICacheService cacheService)
        {
            _orderRepository = orderRepository;
            _cacheService = cacheService;
        }

        public async Task UpdateCacheOrders()
        {
            if (await _cacheService.GetCacheValue<bool>("updating:orders")) return;
            await _cacheService.SetCacheValue("updating:orders", true);
            try
            {
                await foreach (var order in _orderRepository.GetOrders())
                    await _cacheService.SetCacheValue($"orders:{order.OrderId}", order);
            }
            finally
            {
                await _cacheService.SetCacheValue("updating:orders", false);
                await _cacheService.SetCacheValue("latest.update:orders", DateTime.Now);
            }
        }

        public Task<IEnumerable<OrderTableDto>> GetOrders(int skip, int take, CancellationToken cancellationToken = default) =>
            _orderRepository.GetOrders(skip, take, cancellationToken);

        public IAsyncEnumerable<OrderDto> GetOrders() => _orderRepository.GetOrders();

        public async Task<OrderDto> GetOrder(int orderId, CancellationToken cancellationToken = default)
        {
            var order = await _cacheService.GetCacheValue<Order>($"orders:{orderId}");
            return (order is not null) ? new OrderDto(orderId, order.Date, order.Note) : await _orderRepository.GetOrder(orderId, cancellationToken);
        }

        public async Task CreateOrder(Order order, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!IsValid(order)) throw new Exception("Registro inválido");

            await _orderRepository.Add(order, cancellationToken);
            await _cacheService.SetCacheValue($"orders:{order.OrderId}", order);
        }

        public async Task UpdateOrder(int orderId, Order order, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!IsValid(order)) throw new Exception("Registro inválido");
            order.OrderId = orderId;

            await _orderRepository.Edit(order, cancellationToken);
            await _cacheService.SetCacheValue($"orders:{orderId}", order);
        }

        public async Task DeleteOrder(int orderId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Order order = new() { OrderId = orderId, Deleted = true };

            await _orderRepository.Delete(order, cancellationToken);
            await _cacheService.DeleteCacheValue($"orders:{orderId}");
        }

        public async Task<string> ExportOrder(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var stringBuilder = new StringBuilder();

            await foreach (var order in _orderRepository.GetOrdersWithProducts())
            {
                stringBuilder.AppendLine($"Order {order.OrderId} {order.Date:dd/MM/yyyy HH:mm:ss}");
                stringBuilder.AppendLine(order.Note);
                stringBuilder.AppendLine();

                stringBuilder.AppendLine("Id,Name,Code,Price Per Unit,Quantity,Price");
                foreach (var product in order.Products)
                    stringBuilder.AppendLine($"{product.ProductId},{product.Name},{product.Code},{product.PricePerUnit},{product.Quantity},{product.TotalPrice}");

                stringBuilder.AppendLine();
            }

            string path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}-Order.csv");
            await File.AppendAllTextAsync(path, stringBuilder.ToString(), Encoding.UTF8, cancellationToken);

            return path;
        }

        public bool IsValid(Order order) => order is { Date: { Year: >= 2021 } };
    }
}
