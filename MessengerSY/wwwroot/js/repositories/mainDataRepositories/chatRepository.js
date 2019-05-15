var chatRepository = new function () {
    this.setChats = function (chats) {
        if (!chats) {
            throw new Error("chats");
        }

        if (mainData.chats.length !== 0) {
            mainData.chats = chats;
        }
    };

    this.getChatById = function (chatId) {
        if (chatId <= 0) {
            return new Error("chatId");
        }

        return mainData.chats.find(function (chat) {
            return chat.chatId === chatId;
        });
    };

    this.getChatParticipant = function (chatId, userProfileId) {
        if (chatId <= 0 || userProfileId <= 0) {
            return Error();
        }

        let chat = chatRepository.getChatById(chatId);
        let participant = null;
        if (chat) {
            participant = chat.participants.find(function (participant) {
                return participant.userProfileId === userProfileId;
            });
        }

        return participant;
    };

    this.setChatMessages = function (chatId, messages) {
        if (chatId <= 0 || !messages) {
            return new Error();
        }

        var chat = chatRepository.getChatById(chatId);
        if (chat) {
            chat.messages = messages;
        }
    };

    this.getChatMessageIndexByGUID = function (chatId, GUID) {
        let chat = chatRepository.getChatById(chatId);
        let messageIndex = null;
        if (chat) {
            messageIndex = chat.messages.findIndex(function (message) {
                if (message.GUID) {
                    return message.GUID === GUID;
                }

                return false;
            });
        }

        return messageIndex;
    };

    this.addMessage = function (chatId, message) {
        if (chatId <= 0) {
            return new Error("chatId");
        }

        if (!message) {
            return new Error("message");
        }

        var chat = chatRepository.getChatById(chatId);
        if (chat) {
            chat.messages.push(message);
        }
    };
    // Если сообщения с таким GUID нет, то, наверное, это значит, что пользователь удалил сообщение и поэтому нужно отправить запрос на сервер на удаление этого сообщения
    this.replaceMessage = function (chatId, GUID, message) {
        if (chatId <= 0) {
            return new Error("chatId");
        }

        if (GUID <= 0) {
            return Error("GUID");
        }

        if (!message) {
            return new Error("message");
        }

        var chat = chatRepository.getChatById(chatId);
        if (chat) {
            let chatMessageIndex = chatRepository.getChatMessageIndexByGUID(chatId, GUID);
            if (chatMessageIndex) {
                chat.messages.splice(chatMessageIndex, 1, message);

                return true;
            }
        }

        return false;
    };
};