using Microsoft.AspNetCore.Mvc;
using SeatBooking.Application.Abstractions;

namespace SeatBooking.API.Controllers;

[Route("api/[controller]")]
public class SeatsController(ISeatService service) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetSeats(CancellationToken cancellationToken)
    {
        var seats = await service.GetSeatsAsync(cancellationToken);
        return Ok(seats);
    }
}