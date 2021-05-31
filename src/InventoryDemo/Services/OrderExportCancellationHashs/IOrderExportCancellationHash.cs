using System;
using System.Threading;

namespace InventoryDemo.Services.OrderExportCancellationHashs
{
    public interface IOrderExportCancellationHash
    {
        CancellationToken GetOrCreateCancellationToken(int orderExportId);

        void Cancel(int orderExportId);
    }
}
