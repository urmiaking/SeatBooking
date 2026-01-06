using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SeatBooking.Application.Abstractions;
using SeatBooking.Core.DependencyInjections;
using SeatBooking.Core.Settings;
using SeatBooking.Domain.SeatAggregate;
using SeatBooking.Infrastructure.Repositories.Abstractions;
using SeatBooking.Infrastructure.Specifications.Seats;

namespace SeatBooking.Application.Seeders;

[ScopedService]
internal sealed class SeatSeeder(ISeatRepository repository, IOptions<SeatBookingSettings> options, ILogger<SeatSeeder> logger) : IDbSeeder
{
    private readonly SeatBookingSettings _settings = options.Value;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var seatsPopulated = await repository.ExistsAsync(new SeatsDefaultSpecification(), cancellationToken);

        if (seatsPopulated)
            return;

        var seats = new List<Seat>();

        for (var i = 1; i <= _settings.MaxSeats; i++)
        {
            var seat = Seat.Create(i);
            seats.Add(seat);
        }

        await repository.CreateRangeAsync(seats, cancellationToken);

        logger.LogInformation("Seeded {Count} seats into the database.", seats.Count);
    }
}