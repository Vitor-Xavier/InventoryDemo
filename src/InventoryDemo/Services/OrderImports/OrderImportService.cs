using InventoryDemo.Crosscutting;
using InventoryDemo.Domain.Models;
using InventoryDemo.Crosscutting.Events;
using InventoryDemo.Infrastructure.Repositories.OrderImports;
using InventoryDemo.Infrastructure.Repositories.Orders;
using InventoryDemo.Infrastructure.Repositories.Users;
using InventoryDemo.Services.CancellationHashs.OrderImports;
using InventoryDemo.Services.Contexts;
using MassTransit;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Services.OrderExports
{
    public class OrderImportService : IOrderImportService
    {
        private readonly IOrderFormatContext _orderFormatContext;

        private readonly IOrderImportRepository _orderImportRepository;

        private readonly IOrderImportCancellationHash _orderImportCancellationHash;

        private readonly IOrderRepository _orderRepository;

        private readonly IUserRepository _userRepository;

        private readonly IHttpContextAccessor _accessor;

        private readonly IBus _bus;

        public OrderImportService(IOrderFormatContext orderFormatContext,
                                  IOrderImportRepository orderImportRepository,
                                  IOrderRepository orderRepository,
                                  IUserRepository userRepository,
                                  IHttpContextAccessor accessor,
                                  IBus bus,
                                  IOrderImportCancellationHash orderImportCancellationHash)
        {
            _bus = bus;
            _accessor = accessor;
            _userRepository = userRepository;
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

            string username = _accessor?.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
            var user = await _userRepository.GetUserByUsername(username, cancellationToken);

            var orderImport = new OrderImport { DataFormat = dataFormat, UserId = user.UserId, ImportStatus = OrderImportStatus.Waiting };
            await _orderImportRepository.Add(orderImport, cancellationToken);

            await _bus.Publish(new OrderImportEvent { OrderImportId = orderImport.OrderImportId, UserId = user.UserId, Username = username, Path = path, DataFormat = dataFormat }, cancellationToken);

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
            await _orderRepository.BulkInsert(orders.ToList(), CancellationToken.None);
        }
    }
}
