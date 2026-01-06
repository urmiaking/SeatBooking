using SeatBooking.Core.Patterns.Repositories;
using SeatBooking.Domain.ReservationAggregate;

namespace SeatBooking.Infrastructure.Repositories.Abstractions;

public interface IReservationRepository : IRepository<Reservation>,
    ICreateRepository<Reservation>,
    IUpdateRepository<Reservation>,
    IDeleteRepository<Reservation>;