using InventoryDemo.Crosscutting;
using InventoryDemo.Domain.Models;
using InventoryDemo.Services.Factories;
using InventoryDemo.Services.Strategies;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Services.Contexts
{
    public class OrderFormatContext : IOrderFormatContext
    {
        private readonly IOrderFormatStrategy[] _strategies;

        public OrderFormatContext(IOrderFormatFactory factory) => _strategies = factory.Create();

        public Task<IEnumerable<Order>> Import(string path, DataFormat format, CancellationToken cancellationToken = default) =>
            _strategies.FirstOrDefault(s => s.DataFormat == format).Import(path, cancellationToken);

        public Task<string> Export(IAsyncEnumerable<OrderExportDto> data, DataFormat format, CancellationToken cancellationToken = default) =>
            _strategies.FirstOrDefault(s => s.DataFormat == format).Export(data, cancellationToken);
    }
}
