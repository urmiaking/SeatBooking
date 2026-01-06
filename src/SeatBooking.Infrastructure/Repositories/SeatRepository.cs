using SeatBooking.Core.DependencyInjections;
using SeatBooking.Core.Patterns.Repositories;
using SeatBooking.Domain.SeatAggregate;
using SeatBooking.Infrastructure.Repositories.Abstractions;

namespace SeatBooking.Infrastructure.Repositories;

[ScopedService]
internal sealed class SeatRepository(AppDbContext dbContext) : RepositoryBase<Seat>(dbContext), ISeatRepository;