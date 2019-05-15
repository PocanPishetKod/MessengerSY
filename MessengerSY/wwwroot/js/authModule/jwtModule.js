var jwtModule = new function () {
    this.decode = function (jwt) {
        if (!jwt || jwt.trim() === "") {
            throw new Error();
        }

        return JSON.parse(atob(jwt.split(".")[1]));
    };
};