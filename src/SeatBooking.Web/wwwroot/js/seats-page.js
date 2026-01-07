(function (global, $) {
    "use strict";

    const state = {
        clientId: null,
        seatsById: new Map(),
        isLoading: false,
        currentReservation: null, // { reservationId, seatId, seatNumber }
        paymentBusy: false
    };

    function setAlert(type, message) {
        const $alert = $("#pageAlert");
        if (!message) {
            $alert.addClass("d-none").removeClass("alert-success alert-danger alert-warning alert-info").text("");
            return;
        }

        $alert
            .removeClass("d-none alert-success alert-danger alert-warning alert-info")
            .addClass(`alert-${type}`)
            .text(message);
    }

    function setPaymentAlert(type, message) {
        const $alert = $("#paymentAlert");
        if (!message) {
            $alert.addClass("d-none").removeClass("alert-success alert-danger alert-warning alert-info").text("");
            return;
        }

        $alert
            .removeClass("d-none alert-success alert-danger alert-warning alert-info")
            .addClass(`alert-${type}`)
            .text(message);
    }

    function seatUi(status) {
        switch (status) {
            case "Available":
                return { btnClass: "btn-outline-success", badgeClass: "text-bg-success", label: "خالی", disabled: false };
            case "Pending":
                return { btnClass: "btn-outline-warning", badgeClass: "text-bg-warning", label: "در حال رزرو", disabled: true };
            case "Reserved":
                return { btnClass: "btn-outline-danger", badgeClass: "text-bg-danger", label: "رزرو شده", disabled: true };
            default:
                return { btnClass: "btn-outline-secondary", badgeClass: "text-bg-secondary", label: status ?? "نامشخص", disabled: true };
        }
    }

    function normalizeSeatStatus(status) {
        if (status === 1 || status === "1") return "Available";
        if (status === 2 || status === "2") return "Pending";
        if (status === 3 || status === "3") return "Reserved";

        if (typeof status === "string") {
            const s = status.trim();
            if (s.toLowerCase() === "available") return "Available";
            if (s.toLowerCase() === "pending") return "Pending";
            if (s.toLowerCase() === "reserved") return "Reserved";
            return s;
        }

        return "Unknown";
    }

    function canCurrentClientPayForSeat(seat) {
        if (seat.status !== "Pending") return false;
        const entry = global.SeatBookingReservationsStore.get(seat.id);
        return !!(entry && entry.clientId === state.clientId && entry.reservationId);
    }

    function createSeatCard(seat) {
        const normalizedStatus = normalizeSeatStatus(seat.status);
        const ui = seatUi(normalizedStatus);

        const allowClick = normalizedStatus === "Available" || canCurrentClientPayForSeat({ ...seat, status: normalizedStatus });
        const isDisabled = !allowClick;

        const $col = $("<div/>", { class: "col", "data-seat-id": seat.id });

        const $btn = $("<button/>", {
            type: "button",
            class: `btn w-100 seat-card ${ui.btnClass}`,
            disabled: isDisabled,
            "data-seat-id": seat.id
        });

        const $row = $("<div/>", { class: "d-flex align-items-center justify-content-between" });
        const $title = $("<div/>", { class: "fw-semibold", text: `صندلی ${seat.number}` });
        const $badge = $("<span/>", { class: `badge ${ui.badgeClass}`, text: ui.label });

        if (normalizedStatus === "Pending" && allowClick) {
            $badge.text("در انتظار پرداخت");
        }

        $row.append($title, $badge);
        $btn.append($row);
        $col.append($btn);

        return $col;
    }

    function upsertSeatCard(seat) {
        seat.status = normalizeSeatStatus(seat.status);

        if (seat.status === "Available" || seat.status === "Reserved") {
            global.SeatBookingReservationsStore.remove(seat.id);
        }

        state.seatsById.set(seat.id, seat);

        const $existing = $(`#seatsGrid [data-seat-id="${seat.id}"]`);
        const $newCard = createSeatCard(seat);

        if ($existing.length) $existing.replaceWith($newCard);
        else $("#seatsGrid").append($newCard);
    }

    function renderSeats(seats) {
        const $grid = $("#seatsGrid");
        $grid.empty();

        state.seatsById.clear();
        seats
            .slice()
            .sort((a, b) => a.number - b.number)
            .forEach(seat => {
                seat.status = normalizeSeatStatus(seat.status);
                state.seatsById.set(seat.id, seat);
                $grid.append(createSeatCard(seat));
            });
    }

    async function refreshSeats() {
        if (state.isLoading) return;
        state.isLoading = true;

        try {
            const seats = await global.SeatBookingApi.getSeats();
            renderSeats(seats);
            setAlert(null, null);
        } catch (xhr) {
            const msg = xhr?.responseJSON?.title || xhr?.statusText || "خطا در دریافت لیست صندلی‌ها.";
            setAlert("danger", msg);
        } finally {
            state.isLoading = false;
        }
    }

    function openPaymentModal(reservationId, seatId, seatNumber) {
        state.currentReservation = { reservationId, seatId, seatNumber };

        $("#paymentSeatTitle").text(`پرداخت برای صندلی ${seatNumber}`);
        setPaymentAlert(null, null);

        const modalEl = document.getElementById("paymentModal");
        const modal = bootstrap.Modal.getOrCreateInstance(modalEl);
        modal.show();
    }

    function closePaymentModal() {
        const modalEl = document.getElementById("paymentModal");
        const modal = bootstrap.Modal.getOrCreateInstance(modalEl);
        modal.hide();
    }

    function setPaymentBusy(isBusy) {
        state.paymentBusy = isBusy;

        $("#paymentModal button[data-pay]").prop("disabled", isBusy);
        $("#paymentModal .btn-close").prop("disabled", isBusy);
    }

    async function onSeatClicked(seatId) {
        const seat = state.seatsById.get(seatId);
        if (!seat) return;

        if (seat.status === "Pending") {
            const entry = global.SeatBookingReservationsStore.get(seatId);
            if (entry && entry.clientId === state.clientId) {
                openPaymentModal(entry.reservationId, seatId, seat.number);
            }
            return;
        }

        if (seat.status !== "Available") return;

        try {
            setAlert("info", "در حال شروع رزرو...");

            const result = await global.SeatBookingApi.startReservation(seatId, state.clientId);
            const reservationId = result.reservationId ?? result;

            global.SeatBookingReservationsStore.set(seatId, reservationId, state.clientId, result.createdAtUtc);

            setAlert("success", "رزرو موقت انجام شد. پرداخت را تکمیل کنید.");
            openPaymentModal(reservationId, seatId, seat.number);

            await refreshSeats();
        } catch (xhr) {
            const msg = xhr?.responseJSON?.title || xhr?.statusText || "خطا در شروع رزرو.";
            setAlert("danger", msg);
            await refreshSeats();
        }
    }

    async function onPayClicked(paymentOutcome) {
        if (state.paymentBusy) return;
        if (!state.currentReservation?.reservationId) return;

        setPaymentBusy(true);
        setPaymentAlert("info", "در حال پردازش پرداخت...");

        try {
            const { reservationId } = state.currentReservation;

            const res = await global.SeatBookingApi.processPayment(
                reservationId,
                state.clientId,
                paymentOutcome
            );

            if (res.status === "Succeeded") {
                global.SeatBookingReservationsStore.remove(state.currentReservation.seatId);

                setPaymentAlert("success", "پرداخت موفق بود. صندلی رزرو شد.");
                await refreshSeats();
                setTimeout(closePaymentModal, 700);
            } else {
                setPaymentAlert("warning", "پرداخت ناموفق بود. می‌توانید دوباره تلاش کنید (تا قبل از پایان زمان رزرو).");
            }
        } catch (xhr) {
            const msg = xhr?.responseJSON?.title || xhr?.statusText || "خطا در انجام پرداخت.";
            setPaymentAlert("danger", msg);
        } finally {
            setPaymentBusy(false);
        }
    }

    function wireDomEvents() {
        $("#seatsGrid").on("click", "button[data-seat-id]", function () {
            const seatId = $(this).attr("data-seat-id");
            if (!seatId) return;
            onSeatClicked(seatId);
        });

        $("#paymentModal").on("click", "button[data-pay]", function () {
            const outcome = $(this).attr("data-pay");
            onPayClicked(outcome);
        });

        document.getElementById("paymentModal").addEventListener("hidden.bs.modal", function () {
            state.currentReservation = null;
            setPaymentAlert(null, null);
            setPaymentBusy(false);
        });
    }

    async function startRealtime() {
        const connection = global.SeatBookingRealtime.createConnection();

        connection.on("SeatUpdated", seat => {
            upsertSeatCard(seat);
        });

        connection.on("SeatsUpdated", seats => {
            if (!Array.isArray(seats))
                return;
            seats.forEach(s => upsertSeatCard(s));
        });

        await global.SeatBookingRealtime.start(connection);
    }

    async function init() {
        state.clientId = global.SeatBookingClientId.getOrCreateClientId();
        wireDomEvents();

        await refreshSeats();
        await startRealtime();
    }

    $(init);
})(window, jQuery);