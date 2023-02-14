using InventoryDemo.Domain.Models;
using System;

namespace InventoryDemo.Crosscutting
{
    public record OrderImportGetDto(int OrderImportId, OrderImportStatus Status, DateTime? StartTime, DateTime? EndTime, int UserId, string Username);
}
