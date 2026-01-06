namespace SeatBooking.Application.Abstractions;

public interface IDbSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}