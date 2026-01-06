using SeatBooking.Core.Patterns.Repositories;
using SeatBooking.Domain.SeatAggregate;

namespace SeatBooking.Infrastructure.Repositories.Abstractions;

public interface ISeatRepository : IRepository<Seat>,
    ICreateRepository<Seat>,
    IUpdateRepository<Seat>,
    IDeleteRepository<Seat>;