using System;

namespace InventoryDemo.Crosscutting
{
    public record OrderTableDto(int OrderId, DateTime Date, string Note);

    public record OrderDto(int OrderId, DateTime Date, string Note);
}
