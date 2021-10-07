using InventoryDemo.Crosscutting;
using InventoryDemo.Events;
using InventoryDemo.Models;
using InventoryDemo.Repositories.OrderExports;
using InventoryDemo.Repositories.Orders;
using InventoryDemo.Services.CancellationHashs.OrderExports;
using InventoryDemo.Services.Contexts;
using MassTransit;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Services.OrderExports
{
    public class OrderExportService : IOrderExportService
    {
        public readonly IOrderRepository _orderRepository;

        public readonly IOrderFormatContext _orderFormatContext;

        public readonly IOrderExportRepository _orderExportRepository;

        public readonly IOrderExportCancellationHash _orderExportCancellationHash;

        public readonly IBus _bus;

        public OrderExportService(IOrderRepository orderRepository, 
                                  IOrderFormatContext orderFormatContext,
                                  IOrderExportRepository orderExportRepository, 
                                  IBus bus, 
                                  IOrderExportCancellationHash orderExportCancellationHash)
        {
            _bus = bus;
            _orderRepository = orderRepository;
            _orderFormatContext = orderFormatContext;
            _orderExportRepository = orderExportRepository;
            _orderExportCancellationHash = orderExportCancellationHash;
        }

        public Task<OrderExportGetDto> GetOrderExport(int orderExportId, CancellationToken cancellationToken = default) =>
            _orderExportRepository.GetOrderExportById(orderExportId, cancellationToken);

        public async Task<OrderExport> CreateOrderExport(DataFormat dataFormat, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var orderExport = new OrderExport { DataFormat = dataFormat, ExportStatus = OrderExportStatus.Waiting };
            await _orderExportRepository.Add(orderExport, cancellationToken);

            await _bus.Publish(new OrderExportEvent { OrderExportId = orderExport.OrderExportId, DataFormat = dataFormat }, cancellationToken);

            return orderExport;
        }

        public async Task UpdateOrderExport(int orderExportId, OrderExport orderExport, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            orderExport.OrderExportId = orderExportId;

            await _orderExportRepository.Edit(orderExport, cancellationToken);
        }

        public void CancelOrderExport(int orderExportId) => _orderExportCancellationHash.Cancel(orderExportId);

        public Task<string> ExportOrders(DataFormat dataFormat, CancellationToken cancellationToken = default) =>
            _orderFormatContext.Export(_orderRepository.GetOrdersWithProducts(), dataFormat, cancellationToken);
    }
}
