using InventoryDemo.Models;

namespace InventoryDemo.Crosscutting
{
    public record PrivateNotificationDto(string To, string Title, string Content, NotificationType Type, string Route) : NotificationDto(Title, Content, Type, Route);
}
