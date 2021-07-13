using System.Collections.Generic;

namespace InventoryDemo.Crosscutting
{
    public record TableDto<T>(IEnumerable<T> Data, int Total);
}
