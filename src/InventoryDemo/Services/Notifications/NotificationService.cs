using InventoryDemo.Crosscutting;
using InventoryDemo.Hubs;
using InventoryDemo.Models;
using InventoryDemo.Repositories.Notifications;
using InventoryDemo.Repositories.Users;
using InventoryDemo.Repositories.UsersNotifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryDemo.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        private readonly IUserNotificationRepository _userNotificationRepository;

        private readonly IUserRepository _userRepository;

        private readonly IHubContext<NotificationHub> _hubContext;

        private readonly IHttpContextAccessor _accessor;

        public NotificationService(INotificationRepository notificationRepository,
                                   IUserNotificationRepository userNotificationRepository,
                                   IUserRepository userRepository,
                                   IHubContext<NotificationHub> hubContext,
                                   IHttpContextAccessor accessor)
        {
            _notificationRepository = notificationRepository;
            _userNotificationRepository = userNotificationRepository;
            _userRepository = userRepository;
            _hubContext = hubContext;
            _accessor = accessor;
        }

        public async Task<TableDto<NotificationListDto>> GetNotifications(int skip, int take, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string username = _accessor?.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

            var notifications = await _notificationRepository.GetNotificationsByUsername(username, skip, take, cancellationToken);
            var total = await _notificationRepository.GetTotalNotificationsByUsername(username, cancellationToken);

            return new TableDto<NotificationListDto>(notifications, total);
        }

        public async Task<int> GetUnreadNotificationCount(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string username = _accessor?.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

            return await _notificationRepository.GetUnreadNotificationCountByUsername(username, cancellationToken);
        }

        public async Task ReadNotification(int notificationId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string username = _accessor?.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
            var user = await _userRepository.GetUserByUsername(username, cancellationToken);
            
            UserNotification userNotification = new() { NotificationId = notificationId, UserId = user.UserId, ReadAt = DateTime.Now };
            await _userNotificationRepository.Edit(userNotification, cancellationToken);

            var count = await GetUnreadNotificationCount(cancellationToken);
            await _hubContext.Clients.Users(username).SendAsync("UpdateCount", count, cancellationToken: cancellationToken);
        }

        public async Task ReadAllNotifications(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string username = _accessor?.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
            var user = await _userRepository.GetUserByUsername(username, cancellationToken);

            await _userNotificationRepository.BatchReadByUser(user.UserId, cancellationToken);

            await _hubContext.Clients.Users(username).SendAsync("UpdateCount", 0, cancellationToken: cancellationToken);
        }

        public Task SendNotification(NotificationDto notification, CancellationToken cancellationToken = default) =>
            SendNotification(notification.Title, notification.Content, notification.Type, notification.Route, cancellationToken);

        public async Task SendNotification(string title, string content, NotificationType type, string route, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var users = await _userRepository.GetUsers(cancellationToken);
            Notification notification = new()
            {
                Title = title,
                Content = content,
                Type = type,
                Route = route,
                UsersNotification = users.Select(u => new UserNotification
                {
                    UserId = u.UserId
                }).ToList(),
            };
            await _notificationRepository.Add(notification, cancellationToken);

            await _hubContext.Clients.All?.SendAsync("ReceiveMessage", notification.NotificationId, "all", title, content, type, route, cancellationToken: cancellationToken);
        }

        public Task SendPrivateNotification(PrivateNotificationDto notification, CancellationToken cancellationToken = default) =>
            SendPrivateNotification(notification.To, notification.Title, notification.Content, notification.Type, notification.Route, cancellationToken);

        public async Task SendPrivateNotification(string to, string title, string content, NotificationType type, string route, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _userRepository.GetUserByUsername(to, cancellationToken);
            Notification notification = new()
            {
                Title = title,
                Content = content,
                Type = type,
                Route = route,
                UsersNotification = new HashSet<UserNotification> { new() { UserId = user.UserId } },
            };
            await _notificationRepository.Add(notification, cancellationToken);

            await _hubContext.Clients.Users(to)?.SendAsync("ReceiveMessage", notification.NotificationId, to, title, content, type, route, cancellationToken: cancellationToken);
        }
    }
}
