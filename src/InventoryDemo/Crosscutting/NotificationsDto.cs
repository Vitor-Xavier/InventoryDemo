using InventoryDemo.Models;
using System;

namespace InventoryDemo.Crosscutting
{
    public record NotificationDto(string Title, string Content, NotificationType Type, string Route);

    public record NotificationListDto(int NotificationId, string Title, string Content, NotificationType Type, string Route, DateTime? ReadAt);
}
