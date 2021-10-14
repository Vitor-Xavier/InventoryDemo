using InventoryDemo.Crosscutting;
using InventoryDemo.Events;
using InventoryDemo.Models;
using InventoryDemo.Repositories.OrderExports;
using InventoryDemo.Repositories.Orders;
using InventoryDemo.Services.CancellationHashs.OrderImports;
using InventoryDemo.Services.Contexts;
using MassTransit;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Services.OrderExports
{
    public class OrderImportService : IOrderImportService
    {
        public readonly IOrderFormatContext _orderFormatContext;

        public readonly IOrderImportRepository _orderImportRepository;

        public readonly IOrderImportCancellationHash _orderImportCancellationHash;

        public readonly IOrderRepository _orderRepository;

        public readonly IBus _bus;

        public OrderImportService(IOrderFormatContext orderFormatContext,
                                  IOrderImportRepository orderImportRepository,
                                  IOrderRepository orderRepository,
                                  IBus bus,
                                  IOrderImportCancellationHash orderImportCancellationHash)
        {
            _bus = bus;
            _orderRepository = orderRepository;
            _orderFormatContext = orderFormatContext;
            _orderImportRepository = orderImportRepository;
            _orderImportCancellationHash = orderImportCancellationHash;
        }

        public Task<OrderImportGetDto> GetOrderImport(int orderImportId, CancellationToken cancellationToken = default) =>
            _orderImportRepository.GetOrderImportById(orderImportId, cancellationToken);

        public async Task<OrderImport> CreateOrderImport(IFormFile dataFile, DataFormat dataFormat, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}-Order.json");
            using FileStream fileStream = File.Create(path);
            await dataFile.CopyToAsync(fileStream, cancellationToken);
            await fileStream.FlushAsync(cancellationToken);

            var orderImport = new OrderImport { DataFormat = dataFormat, ImportStatus = OrderImportStatus.Waiting };
            await _orderImportRepository.Add(orderImport, cancellationToken);

            await _bus.Publish(new OrderImportEvent { OrderImportId = orderImport.OrderImportId, Path = path, DataFormat = dataFormat }, cancellationToken);

            return orderImport;
        }

        public async Task UpdateOrderImport(int orderImportId, OrderImport orderImport, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            orderImport.OrderImportId = orderImportId;

            await _orderImportRepository.Edit(orderImport, cancellationToken);
        }

        public void CancelOrderImport(int orderExportId) => _orderImportCancellationHash.Cancel(orderExportId);

        public async Task ImportOrders(string path, DataFormat dataFormat, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var orders = await _orderFormatContext.Import(path, dataFormat, cancellationToken);
            await _orderRepository.BulkInsert(orders.ToList());
        }
    }
}
