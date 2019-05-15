var messengerSYAuth = new {
    setAuthorizationHeader: function (xhr) {
        var access_token = this.getAccessToken();
        if (access_token !== this.constants.noAccessToken) {
            xhr.serRequestHeader("Authorization", "Bearer " + access_token);
        }
    },
    saveAuthData: function(authData) {
        localStorage.setItem("access_token", authData.access_token);
        localStorage.setItem("refresh_token", authData.refresh_token);
        localStorage.setItem("refreshTokenId", authData.refreshTokenId);
        localStorage.setItem("userProfileId", authData.userProfileId);
    },
    getAccessToken: function() {
        var access_token = localStorage.getItem("access_token");
        if (access_token) {
            return access_token;
        }

        return this.constants.noAccessToken;
    },
    getRefreshToken: function() {
        var refresh_token = localStorage.getItem("refresh_token");
        if (refresh_token) {
            return refresh_token;
        }

        return this.constants.noRefreshToken;
    },
    getRefreshTokenId: function () {
        var refreshTokenId = localStorage.getItem("refreshTokenId");
        if (refreshTokenId) {
            return refreshTokenId;
        }

        return this.constants.noRefreshTokenId;
    },
    getUserProfileId: function() {
        var userProfileId = localStorage.getItem("userProfileId");
        if (userProfileId) {
            return userProfileId;
        }

        return this.constants.noUserProfileId;
    },
    getAuthData: function () {
        var access_token = this.getAccessToken();
        if (access_token === this.constants.noAccessToken) {
            return this.constants.noAuthData;
        }

        var refresh_token = this.getRefreshToken();
        if (refresh_token === this.constants.noRefreshToken) {
            return this.constants.noAuthData;
        }

        var refreshTokenId = this.getRefreshTokenId();
        if (refreshTokenId === this.constants.noRefreshTokenId) {
            return this.constants.noAuthData;
        }

        var userProfileId = this.getUserProfileId();
        if (userProfileId === this.constants.noUserProfileId) {
            return this.constants.noAuthData;
        }

        return new AuthData(access_token, refresh_token, refreshTokenId, userProfileId);
    },
    isAuthenticate: function() {
        if (this.getAccessToken() !== this.constants.noToken) {
            return true;
        }

        return false;
    },
    registrate: function (phoneNumber) {
        if (!phoneNumber || phoneNumber.trim() === "") {
            return this.constants.incorrectNumber;
        }

        var result;
        $.ajax({
            url: this.constants.registrationEndPoint,
            type: "POST",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify(phoneNumber),
            success: function (response) {
                result = new AuthData(response.jwt,
                    response.refreshToken.refreshToken,
                    response.refreshToken.refreshTokenId,
                    response.refreshToken.userProfileId);
            },
            statusCode: {
                400: function () {
                    alert("Не правильный формат номера");
                },
                409: function () {
                    alert("Похоже вы уже зарегстрированы. Попробуйте войти по введенному номеру телефона");
                }
            }
        });

        return result;
    },
    authenticate: function(phoneNumber) {
        if (!phoneNumber || phoneNumber.trim() === "") {
            return this.constants.incorrectNumber;
        }

        var result;
        $.ajax({
            url: this.constants.authenticationEndPoint,
            type: "POST",
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify(phoneNumber),
            success: function (response) {
                result = new AuthData(response.jwt,
                    response.refreshToken.refreshToken,
                    response.refreshToken.refreshTokenId,
                    response.refreshToken.userProfileId);
            },
            statusCode: {
                400: function () {
                    alert("Не правильный формат номера");
                },
                403: function () {
                    alert("Введенный номер телефона не зарегистрирован");
                }
            }
        });

        return result;
    },
    constants: {
        noAccessToken: "no access_token",
        noRefreshToken: "no refresh_token",
        noUserProfileId: "no userProfileId",
        noRefreshTokenId: "no refreshTokenId",
        noAuthData: "no auth data",
        incorrectNumber: "incorrect number",
        registrationEndPoint: "/api/account/testreg",
        authenticationEndPoint: "/api/account/testauth"
    }
};

function AuthData(access_token, refresh_token, refreshTokenId, userProfileId) {
    this.access_token = access_token;
    this.refresh_token = refresh_token;
    this.refreshTokenId = refreshTokenId;
    this.userProfileId = userProfileId;
}