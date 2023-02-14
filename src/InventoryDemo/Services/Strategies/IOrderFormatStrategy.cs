using InventoryDemo.Crosscutting;
using InventoryDemo.Domain.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Services.Strategies
{
    public interface IOrderFormatStrategy
    {
        DataFormat DataFormat { get; }

        Task<IEnumerable<Order>> Import(string path, CancellationToken cancellationToken = default);

        Task<string> Export(IAsyncEnumerable<OrderExportDto> data, CancellationToken cancellationToken = default);
    }
}
