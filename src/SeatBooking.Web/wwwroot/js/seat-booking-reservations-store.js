(function (global) {
    "use strict";

    const STORAGE_KEY = "seatBooking.activeReservationsBySeatId";
    // structure:
    // {
    //   "<seatId>": { reservationId: "<guid>", createdAtUtc: "<iso>", clientId: "<guid>" }
    // }

    function readAll() {
        try {
            const raw = global.localStorage.getItem(STORAGE_KEY);
            if (!raw) return {};
            const parsed = JSON.parse(raw);
            return parsed && typeof parsed === "object" ? parsed : {};
        } catch {
            return {};
        }
    }

    function writeAll(obj) {
        global.localStorage.setItem(STORAGE_KEY, JSON.stringify(obj));
    }

    function set(seatId, reservationId, clientId, createdAtUtc) {
        const all = readAll();
        all[seatId] = {
            reservationId,
            clientId,
            createdAtUtc: createdAtUtc || new Date().toISOString()
        };
        writeAll(all);
    }

    function get(seatId) {
        const all = readAll();
        return all[seatId] || null;
    }

    function remove(seatId) {
        const all = readAll();
        if (!all[seatId]) return;
        delete all[seatId];
        writeAll(all);
    }

    function clear() {
        global.localStorage.removeItem(STORAGE_KEY);
    }

    global.SeatBookingReservationsStore = {
        set,
        get,
        remove,
        clear
    };
})(window);