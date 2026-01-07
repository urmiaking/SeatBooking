using Microsoft.AspNetCore.SignalR;
using SeatBooking.API.Hubs;
using SeatBooking.Application.Abstractions;
using SeatBooking.Application.DTOs.Seats;
using SeatBooking.Core.DependencyInjections;

namespace SeatBooking.API.Services;

[ScopedService]
internal sealed class SeatBookingNotifier(
    IHubContext<SeatBookingHub, ISeatBookingClient> hubContext)
    : ISeatBookingNotifier
{
    public Task SeatsUpdatedAsync(IReadOnlyList<GetSeatResponse> seats, CancellationToken cancellationToken = default)
    {
        return hubContext.Clients
            .Group(SeatBookingHub.HallGroupName)
            .SeatsUpdated(seats);
    }

    public Task SeatUpdatedAsync(GetSeatResponse seat, CancellationToken cancellationToken = default)
    {
        return hubContext.Clients
            .Group(SeatBookingHub.HallGroupName)
            .SeatUpdated(seat);
    }
}