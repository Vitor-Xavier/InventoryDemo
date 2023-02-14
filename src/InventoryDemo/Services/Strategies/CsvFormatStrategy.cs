using InventoryDemo.Crosscutting;
using InventoryDemo.Domain.Models;
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

        private readonly string _orderHeader = "Order;Date;Note";

        private readonly string _productHeader = "Product;Name;Code;Price Per Unit;Quantity;Price";

        public async Task<string> Export(IAsyncEnumerable<OrderExportDto> data, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var stringBuilder = new StringBuilder();

            await foreach (var order in data)
            {
                stringBuilder.AppendLine(_orderHeader);
                stringBuilder.AppendLine($"{order.OrderId};{order.Date:dd/MM/yyyy HH:mm:ss};{order.Note}");

                stringBuilder.AppendLine(_productHeader);
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
            Order order = new()
            {
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Deleted = false
            };
            foreach (string line in lines)
            {
                string[] columns = line.Split(';');

                if (columns.Length == 1)
                {
                    orders.Add((Order)order.Clone());
                    order = new();
                }
                else if (columns.Length == 3)
                {
                    if (line == _orderHeader) continue;
                    order.Date = DateTime.TryParseExact(columns[1], "dd/MM/yyyy HH:mm:ss", new CultureInfo("pt-BR"), DateTimeStyles.None, out DateTime dateTime) ? dateTime : DateTime.Now;
                    order.Note = columns[2];
                }
                else if (columns.Length == 6)
                {
                    if (line == _productHeader) continue;
                    order.OrderProducts.Add(new OrderProduct
                    {
                        ProductId = int.TryParse(columns[0], out int productId) ? productId : default,
                        Quantity = decimal.TryParse(columns[4], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal quantity) ? quantity : default,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        Deleted = false
                    });
                }
            }
            return orders;
        }
    }
}
