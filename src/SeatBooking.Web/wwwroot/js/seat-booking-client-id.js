(function (global) {
    "use strict";

    const STORAGE_KEY = "seatBooking.clientId";

    function generateGuid() {
        if (global.crypto && global.crypto.randomUUID) {
            return global.crypto.randomUUID();
        }

        const s4 = () => Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1);
        return `${s4()}${s4()}-${s4()}-${s4()}-${s4()}-${s4()}${s4()}${s4()}`;
    }

    function getOrCreateClientId() {
        let clientId = global.localStorage.getItem(STORAGE_KEY);
        if (!clientId) {
            clientId = generateGuid();
            global.localStorage.setItem(STORAGE_KEY, clientId);
        }
        return clientId;
    }

    global.SeatBookingClientId = {
        getOrCreateClientId
    };
})(window);