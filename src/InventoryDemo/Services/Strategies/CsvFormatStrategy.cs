using InventoryDemo.Crosscutting;
using InventoryDemo.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Services.Strategies
{
    public class CsvFormatStrategy : IOrderFormatStrategy
    {
        public DataFormat DataFormat => DataFormat.Csv;

        public async Task<string> Export(IAsyncEnumerable<OrderExportDto> data, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var stringBuilder = new StringBuilder();

            await foreach (var order in data)
            {
                stringBuilder.AppendLine($"Order {order.OrderId} {order.Date:dd/MM/yyyy HH:mm:ss}");
                stringBuilder.AppendLine(order.Note);
                stringBuilder.AppendLine();

                stringBuilder.AppendLine("Id;Name;Code;Price Per Unit;Quantity;Price");
                foreach (var product in order.Products)
                    stringBuilder.AppendLine($"{product.ProductId};{product.Name};{product.Code};{product.PricePerUnit.ToString(CultureInfo.InvariantCulture)};{product.Quantity.ToString(CultureInfo.InvariantCulture)};{product.TotalPrice.ToString(CultureInfo.InvariantCulture)}");

                stringBuilder.AppendLine();
            }
            cancellationToken.ThrowIfCancellationRequested();

            string path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}-Order.csv");
            await File.AppendAllTextAsync(path, stringBuilder.ToString(), Encoding.UTF8, cancellationToken);

            return path;
        }

        public async Task<IEnumerable<Order>> Import(string path, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string[] lines = await File.ReadAllLinesAsync(path, cancellationToken);
            HashSet<Order> orders = new();
            Order order = new();
            foreach (string line in lines)
            {
                string[] columns = line.Split(';');

                if (columns.Length == 6)
                {
                    order.OrderProducts.Add(new OrderProduct
                    {
                        ProductId = int.TryParse(columns[0], out int productId) ? productId : default,
                        Quantity = int.TryParse(columns[4], out int quantity) ? quantity : default,
                    });
                }
                else if (columns.Length == 1)
                {
                    if (columns[0].StartsWith("Order"))
                    {
                        orders.Add((Order)order.Clone());
                        order = new();
                    }
                    else
                    {
                        order.Note = columns[0];
                    }
                }
            }
            return orders;
        }
    }
}
