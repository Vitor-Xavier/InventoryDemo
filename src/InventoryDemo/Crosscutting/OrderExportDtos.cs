using InventoryDemo.Models;
using System;

namespace InventoryDemo.Crosscutting
{
    public record OrderExportGetDto(int OrderExportId, OrderExportStatus Status, DateTime? StartTime, DateTime? EndTime, int UserId, string Username);
}
