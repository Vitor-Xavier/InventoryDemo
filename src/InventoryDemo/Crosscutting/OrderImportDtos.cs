using InventoryDemo.Models;
using System;

namespace InventoryDemo.Crosscutting
{
    public record OrderImportGetDto(int OrderImportId, OrderImportStatus Status, DateTime? StartTime, DateTime? EndTime);
}
