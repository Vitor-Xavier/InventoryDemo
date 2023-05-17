using InventoryDemo.Crosscutting;
using InventoryDemo.Crosscutting.Events;
using InventoryDemo.Domain.Models;
using InventoryDemo.Infrastructure.Persistance.Repositories.OrderExports;
using InventoryDemo.Infrastructure.Persistance.Repositories.Orders;
using InventoryDemo.Infrastructure.Persistance.Repositories.Users;
using InventoryDemo.Services.CancellationHashs.OrderExports;
using InventoryDemo.Services.Contexts;
using MassTransit;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Services.OrderExports
{
    public class OrderExportService : IOrderExportService
    {
        public readonly IOrderRepository _orderRepository;

        public readonly IOrderFormatContext _orderFormatContext;

        public readonly IOrderExportRepository _orderExportRepository;

        private readonly IUserRepository _userRepository;

        private readonly IHttpContextAccessor _accessor;

        public readonly IOrderExportCancellationHash _orderExportCancellationHash;

        public readonly IBus _bus;

        public OrderExportService(IOrderRepository orderRepository, 
                                  IUserRepository userRepository,
                                  IOrderFormatContext orderFormatContext,
                                  IOrderExportRepository orderExportRepository, 
                                  IBus bus,
                                  IHttpContextAccessor accessor,
                                  IOrderExportCancellationHash orderExportCancellationHash)
        {
            _bus = bus;
            _accessor = accessor;
            _userRepository = userRepository;
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

            string username = _accessor?.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
            var user = await _userRepository.GetUserByUsername(username, cancellationToken);

            var orderExport = new OrderExport { DataFormat = dataFormat, UserId = user.UserId, ExportStatus = OrderExportStatus.Waiting };
            await _orderExportRepository.Add(orderExport, cancellationToken);

            await _bus.Publish(new OrderExportEvent { OrderExportId = orderExport.OrderExportId, UserId = user.UserId, Username = username, DataFormat = dataFormat }, cancellationToken);

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
