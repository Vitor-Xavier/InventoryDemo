using InventoryDemo.Crosscutting;
using InventoryDemo.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Services.Contexts
{
    public interface IOrderFormatContext
    {
        Task<IEnumerable<Order>> Import(string path, DataFormat format, CancellationToken cancellationToken = default);

        Task<string> Export(IAsyncEnumerable<OrderExportDto> data, DataFormat format, CancellationToken cancellationToken = default);
    }
}
