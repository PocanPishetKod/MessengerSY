var registrationModule = new function () {
    this.registrate = function (phonenumber) {
        if (!phonenumber || phonenumber.trim() === "") {
            throw new Error();
        }

        ajaxModule.send(endPointHelper.accountEndPoints.registrationEndPoint(),
            "POST",
            { phoneNumber: phonenumber },
            handlers.registrationHandlers.successHandler,
            null,
            { 400: handlers.registrationHandlers.statusCode400Handler, 409: handlers.registrationHandlers.statusCode409Handler });
    };

    var handlers = new function () {
        this.registrationHandlers = new function () {
            this.successHandler = function (registrationData) {
                access_token_module.setAccessToken(registrationData.jwt);
                refresh_token_module
                    .setRefreshToken(new refresh_token_module.refreshTokenModel(registrationData.refreshToken.refreshToken,
                        registrationData.refreshToken.refreshTokenId));
            };

            this.statusCode400Handler = function () {
                alert("Не правильный формат номера");
            };

            this.statusCode409Handler = function () {
                alert("Похоже вы уже зарегстрированы. Попробуйте войти по введенному номеру телефона");
            };
        };
    };
};