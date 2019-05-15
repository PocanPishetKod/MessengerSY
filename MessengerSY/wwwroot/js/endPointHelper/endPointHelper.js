var endPointHelper = new function () {
    this.accountEndPoints = new function () {
        this.authenticationEndPoint = function () {
            return "/api/account/testauth";
        };

        this.registrationEndPoint = function () {
            return "/api/account/testreg";
        };
    };

    this.contactEndPoints = new function () {
        this.getContactsEndPoint = function () {
            return "/api/contact/getcontacts";
        };
    };

    this.chatEndPoints = new function () {
        this.getChatsEndPoint = function () {
            return "/api/chat/getchats";
        };

        this.getChatMessagesEndPoint = function () {
            return "/api/chat/getchatmessages";
        };
    };

    this.hubEndPoints = new function () {
        this.getMainHubEndPoint = function () {
            return "hubs/main";
        };
    };
};