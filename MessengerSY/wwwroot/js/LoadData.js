var loadData = new {
    getUserProfileContacts: function () {
        var result;
        $.ajax({
            url: this.constants.getContactsUrl,
            type: "GET",
            dataType: "json",
            async: false,
            beforeSend: function (xhr) {
                messengerSYAuth.setAuthorizationHeader(xhr);
            },
            success: function (response) {
                result = response;
            }
        });

        return result;
    },
    getUserProfileChats: function () {

        var result;
        $.ajax({
            url: this.constants.getUserProfileChatsUrl,
            type: "GET",
            dataType: "json",
            async: false,
            beforeSend: function (xhr) {
                messengerSYAuth.setAuthorizationHeader(xhr);
            },
            success: function (response) {
                result = response;
            }
        });

        return result;
    },
    getChatMessages: function (chatId) {

        var result;
        $.ajax({
            url: this.constants.getChatMessagesUrl,
            type: "GET",
            dataType: "json",
            beforeSend: function (xhr) {
                messengerSYAuth.setAuthorizationHeader(xhr);
            },
            data: { chatid: chatId, count: 1000, startdate: null },
            contentType: "application/json",
            success: function (response) {
                result = response;
            }
        });

        return result;
    },
    constants: {
        getContactsUrl: "/api/contact/getcontacts",
        getUserProfileChatsUrl: "/api/chat/getchats",
        getChatMessagesUrl: "/api/chat/getchatmessages"
    }
};