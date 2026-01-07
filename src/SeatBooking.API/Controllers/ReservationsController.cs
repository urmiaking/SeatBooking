using Microsoft.AspNetCore.Mvc;
using SeatBooking.Application.Abstractions;
using SeatBooking.Application.DTOs.Payments;
using SeatBooking.Application.DTOs.Reservations;

namespace SeatBooking.API.Controllers;

[Route("api/[controller]")]
public class ReservationsController(IReservationService service) : ApiControllerBase
{
    [HttpPost("start")]
    public async Task<IActionResult> StartReservationAsync(StartReservationRequest request, CancellationToken cancellationToken)
    {
        var response = await service.StartReservationAsync(request, cancellationToken);
        return response.Match(Ok, Problem);
    }

    [HttpPost("process-payment")]
    public async Task<IActionResult> ProcessPaymentAsync(ReservationPaymentRequest request, CancellationToken cancellationToken)
    {
        var response = await service.ProcessPaymentAsync(request, cancellationToken);
        return response.Match(Ok, Problem);
    }
}