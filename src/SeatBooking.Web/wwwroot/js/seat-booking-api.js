(function (global, $) {
    "use strict";

    const endpoints = {
        seats: "/api/seats",
        startReservation: "/api/reservations/start",
        processPayment: "/api/reservations/process-payment",
        resetReservations: "/api/reservations/reset"
    };

    function ajaxJson(method, url, body) {
        return $.ajax({
            method: method,
            url: url,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: body ? JSON.stringify(body) : undefined
        });
    }

    function getSeats() {
        return ajaxJson("GET", endpoints.seats);
    }

    function startReservation(seatId, clientId) {
        return ajaxJson("POST", endpoints.startReservation, {
            seatId: seatId,
            clientId: clientId
        });
    }

    function processPayment(reservationId, clientId, paymentOutcome) {
        return ajaxJson("POST", endpoints.processPayment, {
            reservationId: reservationId,
            clientId: clientId,
            paymentOutcome: paymentOutcome
        });
    }

    function resetReservations() {
        return ajaxJson("POST", endpoints.resetReservations);
    }

    global.SeatBookingApi = {
        getSeats,
        startReservation,
        processPayment,
        resetReservations
    };
})(window, jQuery);