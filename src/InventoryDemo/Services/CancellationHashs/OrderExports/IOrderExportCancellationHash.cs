using System.Threading;

namespace InventoryDemo.Services.CancellationHashs.OrderExports
{
    public interface IOrderExportCancellationHash
    {
        CancellationToken GetOrCreateCancellationToken(int orderExportId);

        void Cancel(int orderExportId);
    }
}
