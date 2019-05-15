var chatLoadModule = new function () {
    this.loadAllChats = function () {
        ajaxModule.send(endPointHelper.chatEndPoints.getChatsEndPoint(),
            "GET",
            null,
            handlers.loadChatsHandlers.successHandler,
            null,
            null);
    };

    var handlers = new function () {
        this.loadChatsHandlers = new function () {
            this.successHandler = function (responseChats) {
                var mappedChats = responseChats.chats.map(function (chat) {
                    let sender = null;
                    let chatTitle = chat.chatTitle;

                    let participants = chat.participants.map(function (participant) {
                        let contact = contactRepository.getContactByPhoneNumber(participant.phoneNumber);
                        let userProfile = new UserProfile(participant.userProfileId,
                            participant.phoneNumber,
                            participant.nickname,
                            contact ? contact.contactName : participant.nickname ? participant.nickname : participant.phoneNumber);

                        if (participant.userProfileId === chat.lastMessage.sender.userProfileId); {
                            sender = userProfile;
                        }

                        if (!chat.isGroup) {
                            if (participant.userProfileId !== userProfileRepository.getUserProfileId()) {
                                chatTitle = userProfile.contactName;
                            }
                        }

                        return userProfile;
                    });

                    let lastMessage = new LastMessage(chat.lastMessage.textContent, sender);

                    return new Chat(chat.chatId, chatTitle, chat.isGroup, lastMessage, participants);
                });

                chatRepository.setChats(mappedChats);
            };
        };
    };
};