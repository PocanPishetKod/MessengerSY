var refresh_token_module = new function () {
    var refreshTokenExists = false;
    const REFRESH_TOKEN = "refresh_token";

    function RefreshTokenModel(refreshToken, refreshTokenId) {
        if (!refreshToken || !refreshTokenId || refreshTokenId <= 0 || refreshToken === "") {
            throw new SyntaxError("invalid refresh_token");
        }

        this.refreshToken = refreshToken;
        this.refreshTokenId = refreshTokenId;
    }

    function BuildRefreshTokenModel(parseObject) {
        if (!parseObject) {
            throw new Error();
        }

        return new RefreshTokenModel(parseObject.refreshToken, parseObject.refreshTokenId);
    }

    this.refreshTokenModel = RefreshTokenModel;

    this.setRefreshToken = function (refreshTokenModel) {
        if (!refreshTokenModel) {
            throw new Error();
        }

        localStorageModule.add(REFRESH_TOKEN, JSON.stringify(refreshTokenModel));
        refreshTokenExists = true;
    };

    this.getRefreshToken = function () {
        if (!refreshTokenExists) {
            return null;
        }

        let refreshTokenModel = JSON.parse(localStorageModule.get(REFRESH_TOKEN));
        return new BuildRefreshTokenModel(refreshTokenModel.refreshToken, refreshTokenModel.refreshTokenId);
    };

    this.removeRefreshToken = function () {
        if (refreshTokenExists) {
            return;
        }

        localStorageModule.remove(REFRESH_TOKEN);
        refreshTokenExists = false;
    };

    this.refreshTokenExists = function () {
        return refreshTokenExists;
    };
};