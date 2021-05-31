using InventoryDemo.Crosscutting;
using InventoryDemo.Events;
using InventoryDemo.Models;
using InventoryDemo.Repositories.OrderExports;
using InventoryDemo.Repositories.Orders;
using InventoryDemo.Services.OrderExportCancellationHashs;
using MassTransit;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Services.OrderExports
{
    public class OrderExportService : IOrderExportService
    {
        public readonly IOrderRepository _orderRepository;

        public readonly IOrderExportRepository _orderExportRepository;

        public readonly IOrderExportCancellationHash _orderExportCancellationHash;

        public readonly IBus _bus;

        public OrderExportService(IOrderRepository orderRepository, 
                                  IOrderExportRepository orderExportRepository, 
                                  IBus bus, 
                                  IOrderExportCancellationHash orderExportCancellationHash)
        {
            _bus = bus;
            _orderRepository = orderRepository;
            _orderExportRepository = orderExportRepository;
            _orderExportCancellationHash = orderExportCancellationHash;
        }

        public Task<OrderExportGetDto> GetOrderExport(int orderExportId, CancellationToken cancellationToken = default) =>
            _orderExportRepository.GetOrderExportById(orderExportId, cancellationToken);

        public async Task<OrderExport> CreateOrderExport(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var orderExport = new OrderExport { ExportStatus = OrderExportStatus.Waiting };
            await _orderExportRepository.Add(orderExport, cancellationToken);

            await _bus.Publish(new OrderExportEvent { OrderExportId = orderExport.OrderExportId }, cancellationToken);

            return orderExport;
        }

        public async Task UpdateOrderExport(int orderExportId, OrderExport orderExport, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            orderExport.OrderExportId = orderExportId;

            await _orderExportRepository.Edit(orderExport, cancellationToken);
        }

        public void CancelOrderExport(int orderExportId) => _orderExportCancellationHash.Cancel(orderExportId);

        public async Task<string> ExportOrders(CancellationToken cancellationToken = default)
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
                    stringBuilder.AppendLine($"{product.ProductId},{product.Name},{product.Code},{product.PricePerUnit.ToString(CultureInfo.InvariantCulture)},{product.Quantity.ToString(CultureInfo.InvariantCulture)},{product.TotalPrice.ToString(CultureInfo.InvariantCulture)}");

                stringBuilder.AppendLine();
            }
            cancellationToken.ThrowIfCancellationRequested();

            string path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}-Order.csv");
            await File.AppendAllTextAsync(path, stringBuilder.ToString(), Encoding.UTF8, cancellationToken);

            return path;
        }
    }
}
