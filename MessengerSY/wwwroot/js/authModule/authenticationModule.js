var authenticationModule = new function () {
    this.authenticate = function (phonenumber) {
        if (!phonenumber || phonenumber.trim() === "") {
            throw new Error();
        }

        ajaxModule.send(endPointHelper.accountEndPoints.authenticationEndPoint(),
            "POST",
            { phoneNumber: phonenumber },
            handlers.autenticationHandlers.successHandler,
            null,
            { 400: handlers.autenticationHandlers.statusCode400Handler, 403: handlers.autenticationHandlers.statusCode403Handler });
    };

    var handlers = new function () {
        this.autenticationHandlers = new function () {
            this.successHandler = function (authenticationData) {
                access_token_module.setAccessToken(authenticationData.jwt);
                refresh_token_module
                    .setRefreshToken(new refresh_token_module.refreshTokenModel(authenticationData.refreshToken.refreshToken,
                        authenticationData.refreshToken.refreshTokenId));
            };

            this.statusCode400Handler = function () {
                alert("Не правильный формат номера");
            };

            this.statusCode403Handler = function () {
                alert("Введенный номер телефона не зарегистрирован");
            };
        };
    };
};