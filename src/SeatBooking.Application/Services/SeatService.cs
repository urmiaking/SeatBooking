using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using SeatBooking.Application.Abstractions;
using SeatBooking.Application.DTOs.Seats;
using SeatBooking.Core.DependencyInjections;
using SeatBooking.Infrastructure.Repositories.Abstractions;
using SeatBooking.Infrastructure.Specifications.Seats;

namespace SeatBooking.Application.Services;
 
[ScopedService]
internal sealed class SeatService(ISeatRepository repository, IMapper mapper) : ISeatService
{
    public async Task<IReadOnlyList<GetSeatResponse>> GetSeatsAsync(CancellationToken cancellationToken = default)
    {
        var seats = await repository
            .Get(new SeatsDefaultSpecification())
            .ToListAsync(cancellationToken);

        return mapper.Map<IReadOnlyList<GetSeatResponse>>(seats);
    }
}