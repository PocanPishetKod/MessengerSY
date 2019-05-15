var payloadModule = new function () {
    var payload = null;

    function Payload(userProfileId, nickname, phonenumber, refreshTokenId) {
        if (!userProfileId || !nickname || !phonenumber || !refreshTokenId) {
            throw new Error();
        }

        this.userProfileId = userProfileId;
        this.nickname = nickname;
        this.phonenumber = phonenumber;
        this.refreshTokenId = refreshTokenId;
    }

    this.getPayload = function (jwt) {
        if (!jwt || typeof jwt !== "string" || jwt.trim() === "") {
            throw new Error();
        }

        if (!payload) {
            let decodePayload = jwtModule.decode(jwt);
            payload = Payload(decodePayload.userProfileId, decodePayload.nickname,
                decodePayload.phonenumber, decodePayload.refreshTokenId);
        }

        return payload;
    };
};