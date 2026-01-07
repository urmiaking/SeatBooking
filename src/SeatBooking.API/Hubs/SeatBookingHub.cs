using Microsoft.AspNetCore.SignalR;

namespace SeatBooking.API.Hubs;

public sealed class SeatBookingHub : Hub<ISeatBookingClient>
{
    public const string HallGroupName = "hall:default";

    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, HallGroupName);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, HallGroupName);
        await base.OnDisconnectedAsync(exception);
    }
}