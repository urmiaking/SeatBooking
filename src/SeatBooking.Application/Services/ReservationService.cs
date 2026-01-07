using ErrorOr;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SeatBooking.Application.Abstractions;
using SeatBooking.Application.DTOs.Payments;
using SeatBooking.Application.DTOs.Reservations;
using SeatBooking.Application.DTOs.Seats;
using SeatBooking.Application.Errors;
using SeatBooking.Application.Validators.Reservations;
using SeatBooking.Core.DependencyInjections;
using SeatBooking.Core.Exceptions;
using SeatBooking.Core.Settings;
using SeatBooking.Domain.ReservationAggregate;
using SeatBooking.Domain.SeatAggregate;
using SeatBooking.Infrastructure.Repositories.Abstractions;
using SeatBooking.Infrastructure.Specifications.Reservations;
using SeatBooking.Infrastructure.Specifications.Seats;
using System.Data;

namespace SeatBooking.Application.Services;

[ScopedService]
internal sealed class ReservationService(
    IReservationRepository reservationRepository,
    ISeatRepository seatRepository,
    IPaymentService paymentService,
    ISeatBookingNotifier notifier,
    IMapper mapper,
    ILogger<ReservationService> logger,
    IOptions<SeatBookingSettings> options,
    StartReservationRequestValidator startReservationValidator,
    ReservationPaymentRequestValidator paymentRequestValidator) : IReservationService
{
    private readonly SeatBookingSettings _settings = options.Value;

    public async Task<ErrorOr<StartReservationResponse>> StartReservationAsync(StartReservationRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await startReservationValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .ConvertAll(error => Error.Validation(
                    code: error.PropertyName,
                    description: error.ErrorMessage));

            return errors;
        }

        await using var dbTransaction =
            await reservationRepository.BeginTransactionAsync(isolationLevel: IsolationLevel.ReadCommitted,
                cancellationToken: cancellationToken);
        {
            try
            {
                var seatId = new SeatId(request.SeatId);
                var clientId = new ClientId(request.ClientId);

                var seat = await seatRepository
                    .Get(new SeatsByIdSpecification(seatId))
                    .SingleOrDefaultAsync(cancellationToken);

                if (seat is null)
                    return ApplicationErrors.Seats.SeatNotFound;

                seat.MarkAsPending();

                await seatRepository.UpdateAsync(seat, cancellationToken);

                var reservation = Reservation.Start(seatId, clientId, DateTime.UtcNow);

                await reservationRepository.CreateAsync(reservation, cancellationToken);

                await dbTransaction.CommitAsync(cancellationToken);

                var seatDto = mapper.Map<GetSeatResponse>(seat);
                try
                {
                    await notifier.SeatUpdatedAsync(seatDto, cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to notify clients for seat {SeatId}", seatDto.Id);
                }

                return new StartReservationResponse(reservation.Id.Value);
            }
            catch (DbUpdateConcurrencyException e)
            {
                logger.LogError(e, "Concurrency conflict while starting reservation for seat {SeatId}", request.SeatId);
                await dbTransaction.RollbackAsync(cancellationToken);
                return ApplicationErrors.Reservations.SeatNotAvailable;
            }
            catch (DomainException e)
            {
                logger.LogError(e.Message, e);
                await dbTransaction.RollbackAsync(cancellationToken);
                return Error.Conflict(description: e.Message);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message, e);
                await dbTransaction.RollbackAsync(cancellationToken);
                return ApplicationErrors.General.InternalError;
            }
        }
    }

    public async Task<ErrorOr<ReservationPaymentResponse>> ProcessPaymentAsync(ReservationPaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await paymentRequestValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .ConvertAll(error => Error.Validation(
                    code: error.PropertyName,
                    description: error.ErrorMessage));
            return errors;
        }

        await using var dbTransaction = await reservationRepository.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        {
            try
            {
                var reservationId = new ReservationId(request.ReservationId);
                var clientId = new ClientId(request.ClientId);

                var reservation = await reservationRepository
                    .Get(new ReservationsByIdSpecification(reservationId))
                    .FirstOrDefaultAsync(cancellationToken);

                if (reservation is null)
                    return ApplicationErrors.Reservations.NotFound;

                if (reservation.ClientId != clientId)
                    return ApplicationErrors.Reservations.ReserveForbidden;

                if (reservation.Status is ReservationStatus.Completed)
                    return ApplicationErrors.Reservations.AlreadyPaid;

                if (reservation.Status is ReservationStatus.Expired)
                    return ApplicationErrors.Reservations.Expired;

                var seat = await seatRepository
                    .Get(new SeatsByIdSpecification(reservation.SeatId))
                    .FirstOrDefaultAsync(cancellationToken);

                if (seat is null)
                    return ApplicationErrors.Seats.SeatNotFound;

                if (seat.Status is not SeatStatus.Pending)
                    return ApplicationErrors.Seats.SeatUnavailable;

                var paymentResult = await paymentService.PayAsync(reservationId, request.PaymentOutcome, cancellationToken);

                if (paymentResult != PaymentStatus.Succeeded)
                    return new ReservationPaymentResponse(paymentResult);

                reservation.CompletePayment();
                seat.Reserve();

                await seatRepository.UpdateAsync(seat, cancellationToken);
                await reservationRepository.UpdateAsync(reservation, cancellationToken);

                await dbTransaction.CommitAsync(cancellationToken);

                var seatDto = mapper.Map<GetSeatResponse>(seat);

                try
                {
                    await notifier.SeatUpdatedAsync(seatDto, cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to notify clients for seat {SeatId}", seatDto.Id);
                }

                return new ReservationPaymentResponse(paymentResult);
            }
            catch (DomainException e)
            {
                logger.LogError(e.Message, e);
                await dbTransaction.RollbackAsync(cancellationToken);
                return Error.Conflict(description: e.Message);
            }
            catch (DbUpdateConcurrencyException e)
            {
                logger.LogError(e, "Concurrency conflict while processing payment for reservation {ReservationId}", request.ReservationId);
                await dbTransaction.RollbackAsync(cancellationToken);
                return ApplicationErrors.Reservations.PaymentFailed;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message, e);
                await dbTransaction.RollbackAsync(cancellationToken);
                return ApplicationErrors.General.InternalError;
            }
        }
    }

    public async Task ExpirePendingReservationsAsync(CancellationToken cancellationToken = default)
    {
        var nowUtc = DateTime.UtcNow;
        var expirationWindow = TimeSpan.FromMinutes(_settings.BookingTimeoutMinutes);

        var expiredReservations = await reservationRepository
            .Get(new ExpiredReservationsSpecification(nowUtc, expirationWindow))
            .ToListAsync(cancellationToken);

        if (expiredReservations.Count == 0)
            return;

        foreach (var r in expiredReservations)
            r.Expire(nowUtc, expirationWindow);

        var seatIds = expiredReservations
            .Select(x => x.SeatId)
            .Distinct()
            .ToList();

        var seats = await seatRepository
            .Get(new SeatsByIdsSpecification(seatIds))
            .ToListAsync(cancellationToken);

        if (seats.Count != seatIds.Count)
        {
            logger.LogWarning(
                "Some seats were not found while expiring reservations. Expected={ExpectedCount} Found={FoundCount}",
                seatIds.Count,
                seats.Count);
        }

        foreach (var seat in seats)
            seat.Release();

        await seatRepository.UpdateRangeAsync(seats, cancellationToken);
        await reservationRepository.UpdateRangeAsync(expiredReservations, cancellationToken);

        var seatDtoList = mapper.Map<List<GetSeatResponse>>(seats);

        try
        {
            await notifier.SeatsUpdatedAsync(seatDtoList, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to notify clients for seats update");
        }

        logger.LogInformation(
            "Expired {ExpiredCount} reservations and released {SeatCount} seats.",
            expiredReservations.Count,
            seats.Count);
    }

    public async Task ResetReservationsAsync(CancellationToken cancellationToken = default)
    {
        await using var dbTransaction = await reservationRepository.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        {
            try
            {
                var reservations = await reservationRepository
                    .Get(new ReservationsDefaultSpecification())
                    .ToListAsync(cancellationToken);

                await reservationRepository.DeleteRangeAsync(reservations, cancellationToken);

                var seats = await seatRepository
                    .Get(new SeatsDefaultSpecification())
                    .ToListAsync(cancellationToken);

                foreach (var seat in seats) 
                    seat.Release();

                await seatRepository.UpdateRangeAsync(seats, cancellationToken);

                await dbTransaction.CommitAsync(cancellationToken);

                var seatDtoList = mapper.Map<List<GetSeatResponse>>(seats);

                try
                {
                    await notifier.SeatsUpdatedAsync(seatDtoList, cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to notify clients for seats reset");
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message, e);
                await dbTransaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}