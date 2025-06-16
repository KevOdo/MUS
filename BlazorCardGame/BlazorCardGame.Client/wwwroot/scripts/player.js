window.playerStorage = {
    getId: function () {
        return localStorage.getItem("playerId");
    },
    setId: function (id) {
        localStorage.setItem("playerId", id);
    }
};
