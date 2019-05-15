var authStateModule = new function () {
    this.isAuthenticate = function () {
        return access_token_module.accessTokenExists();
    };
};