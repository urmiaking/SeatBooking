namespace SeatBooking.Application.DTOs.Payments;

public record ReservationPaymentRequest(Guid ReservationId, Guid ClientId, PaymentOutcome PaymentOutcome);