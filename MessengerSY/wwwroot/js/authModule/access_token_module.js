var access_token_module = new function () {
    var accessTokenExists = false;
    const ACCESS_TOKEN = "access_token";

    this.setAccessToken = function (access_token) {
        if (!access_token || access_token.trim() === "") {
            throw new Error();
        }

        if (accessTokenExists) {
            return;
        }

        localStorageModule.add(ACCESS_TOKEN, access_token);
        accessTokenExists = true;
    };

    this.getAccessToken = function () {
        if (!accessTokenExists) {
            return null;
        }

        return localStorageModule.get(ACCESS_TOKEN);
    };

    this.updateAccessToken = function (jwt) {
        if (accessTokenExists) {
            throw new Error();
        }

        localStorageModule.add(ACCESS_TOKEN, jwt);
    };

    this.removeAccessToken = function () {
        if (!accessTokenExists) {
            return;
        }

        localStorageModule.remove(ACCESS_TOKEN);
    };

    this.accessTokenExists = function () {
        return accessTokenExists;
    };
};