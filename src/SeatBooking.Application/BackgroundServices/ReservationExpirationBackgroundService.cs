using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SeatBooking.Application.Abstractions;
using SeatBooking.Core.Settings;

namespace SeatBooking.Application.BackgroundServices;

internal sealed class ReservationExpirationBackgroundService(
    IServiceScopeFactory scopeFactory,
    IOptions<SeatBookingSettings> options,
    ILogger<ReservationExpirationBackgroundService> logger)
    : BackgroundService
{
    private readonly SeatBookingSettings _settings = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var pollInterval = TimeSpan.FromSeconds(10);

        logger.LogInformation(
            "{ReservationExpirationBackgroundService} started. PollInterval={PollIntervalSeconds}s, BookingTimeout={BookingTimeout}.",
            nameof(ReservationExpirationBackgroundService),
            pollInterval.TotalSeconds,
            _settings.BookingTimeoutMinutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();

                var reservationService = scope.ServiceProvider.GetRequiredService<IReservationService>();

                await reservationService.ExpirePendingReservationsAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // graceful shutdown
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while expiring pending reservations.");
            }

            try
            {
                await Task.Delay(pollInterval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // graceful shutdown
            }
        }

        logger.LogInformation($"{nameof(ReservationExpirationBackgroundService)} stopped.");
    }
}