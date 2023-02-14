using InventoryDemo.Crosscutting;
using InventoryDemo.Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Services.Strategies
{
    public class JsonFormatStrategy : IOrderFormatStrategy
    {
        public DataFormat DataFormat => DataFormat.Json;

        public async Task<string> Export(IAsyncEnumerable<OrderExportDto> data, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            HashSet<OrderExportDto> orders = new();
            await foreach (var order in data) orders.Add(order);

            string path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}-Order.json");
            FileStream stream = new(path, FileMode.Create, FileAccess.Write);
            await JsonSerializer.SerializeAsync(stream, orders, cancellationToken: cancellationToken);
            await stream.DisposeAsync();

            return path;
        }

        public async Task<IEnumerable<Order>> Import(string path, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            FileStream stream = new(path, FileMode.Open, FileAccess.Read);
            var orders = await JsonSerializer.DeserializeAsync<IEnumerable<OrderImportDto>>(stream, cancellationToken: cancellationToken);

            return orders.Select(o => new Order
            {
                Note = o.Note,
                Date = o.Date,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Deleted = false,
                OrderProducts = o.Products.Select(o => new OrderProduct
                {
                    ProductId = o.ProductId,
                    Quantity = o.Quantity,
                }).ToList()
            });
        }
    }
}
