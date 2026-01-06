using Mapster;
using SeatBooking.Application.DTOs.Seats;
using SeatBooking.Domain.SeatAggregate;

namespace SeatBooking.Application.Mappings;

internal class SeatMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Seat, GetSeatResponse>()
            .Map(dest => dest.Id, src => src.Id.Value);
    }
}