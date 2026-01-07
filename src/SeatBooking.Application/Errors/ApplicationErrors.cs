using ErrorOr;

namespace SeatBooking.Application.Errors;

public static class ApplicationErrors
{
    public static class Reservations
    {
        public static Error SeatNotAvailable => Error.Conflict(
            code: "Reservation.SeatAlreadyReserved",
            description: "این صندلی برای رزرو در دسترس نمی باشد");

        public static Error NotFound => Error.NotFound(
            code: "Reservation.NotFound",
            description: "رزرو یافت نشد");

        public static Error InvalidState => Error.Conflict(
            code: "Reservation.InvalidState",
            description: "رزرو نامعتبر است");

        public static Error ReserveForbidden => Error.Forbidden(
            code: "Reservation.ReserveForbidden",
            description: "رزرو این صندلی مجاز نمی باشد");

        public static Error Expired => Error.Conflict(
            code: "Reservation.Expired",
            description: "رزرو منقضی شده است");

        public static Error PaymentFailed => Error.Conflict(
            code: "Reservation.PaymentFailed",
            description: "پرداخت رزرو ناموفق بود");
    }

    public static class Seats
    {
        public static Error SeatNotFound => Error.NotFound(
            code: "Seat.NotFound",
            description: "صندلی یافت نشد");

        public static Error SeatUnavailable => Error.Conflict(
            code: "Seat.Unavailable",
            description: "صندلی در دسترس نیست");
    }

    public static class General
    {
        public static Error InternalError => Error.Failure(
            code: "General.InternalError",
            description: "خطای داخلی رخ داده است");
    }
}