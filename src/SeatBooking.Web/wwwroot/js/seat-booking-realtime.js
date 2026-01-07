(function (global) {
    "use strict";

    function createConnection() {
        if (!global.signalR) {
            throw new Error("SignalR client is not loaded. Make sure signalr.min.js is included.");
        }

        return new global.signalR.HubConnectionBuilder()
            .withUrl("/hubs/seat-booking")
            .withAutomaticReconnect([0, 2000, 5000, 10000])
            .configureLogging(global.signalR.LogLevel.Warning)
            .build();
    }

    async function start(connection) {
        try {
            await connection.start();
            return true;
        } catch (e) {
            setTimeout(() => start(connection), 3000);
            return false;
        }
    }

    global.SeatBookingRealtime = {
        createConnection,
        start
    };
})(window);