var userProfileRepository = new function () {
    this.setUserProfile = function (userProfile) {
        if (!userProfile) {
            throw new Error("userProfile");
        }

        mainData.userProfile = userProfile;
    };

    this.getUserProfile = function () {
        return mainData.userProfile;
    };

    this.getUserProfileId = function () {
        return mainData.userProfile.userProfileId;
    };
};