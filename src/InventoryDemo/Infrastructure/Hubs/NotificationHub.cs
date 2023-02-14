using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InventoryDemo.Infrastructure.Hubs
{
    public class NotificationHub : Hub
    {
        public const string ROUTE = "/hubs/notifications";

        public async Task SendMessage(string message)
        {
            var username = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
            await Clients.All?.SendAsync("ReceiveMessage", username, message);
        }

        public async Task SendPrivateMessage(string user, string message)
        {
            var username = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
            await Clients.Users(new string[] { user, username })?.SendAsync("ReceiveMessage", username, user, message);
        }
    }
}
