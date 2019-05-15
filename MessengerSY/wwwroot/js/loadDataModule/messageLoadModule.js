var messageLoadModule = new function () {
    this.loadChatMessages = function (chatId) {
        if (chatId <= 0) {
            throw new Error("chatId");
        }

        ajaxModule.send(endPointHelper.chatEndPoints.getChatMessagesEndPoint,
            "GET",
            { chatid: chatId, count: 1000, startdate: null },
            handlers.loadChatMessages.successHandler,
            null,
            null);
    };

    var handlers = new function () {
        this.loadChatMessages = new function () {
            this.successHandler = function (chatMessages) {
                let mapMessages = chatMessages.messages.map(function (message) {
                    let sender = null;

                    if (message.send.userProfileId === userProfileRepository.getUserProfileId()) {
                        sender = userProfileRepository.getUserProfile();
                    } else {
                        sender = chatRepository.getChatParticipant(chatMessages.chatId, message.sender.userProfileId);
                    }

                    return new Message(message.messageId, message.chatId, message.sendDate, message.textContent, sender);
                });

                chatRepository.setChatMessages(chatMessages.chatId, mapMessages);
            };
        };
    };
};