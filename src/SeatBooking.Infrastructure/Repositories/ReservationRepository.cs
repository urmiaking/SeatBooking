using SeatBooking.Core.DependencyInjections;
using SeatBooking.Core.Patterns.Repositories;
using SeatBooking.Domain.ReservationAggregate;
using SeatBooking.Infrastructure.Repositories.Abstractions;

namespace SeatBooking.Infrastructure.Repositories;

[ScopedService]
internal sealed class ReservationRepository(AppDbContext dbContext) : RepositoryBase<Reservation>(dbContext), IReservationRepository;