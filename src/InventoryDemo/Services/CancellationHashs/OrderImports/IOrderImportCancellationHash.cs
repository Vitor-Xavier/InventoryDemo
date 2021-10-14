using System.Threading;

namespace InventoryDemo.Services.CancellationHashs.OrderImports
{
    public interface IOrderImportCancellationHash
    {
        CancellationToken GetOrCreateCancellationToken(int orderImportId);

        void Cancel(int orderImportId);
    }
}
