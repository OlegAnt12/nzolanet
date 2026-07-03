using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace NzolaWebAPI.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var utilizadorId = Context.User?.FindFirst("utilizadorId")?.Value;
            if (utilizadorId != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{utilizadorId}");
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var utilizadorId = Context.User?.FindFirst("utilizadorId")?.Value;
            if (utilizadorId != null)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{utilizadorId}");
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
