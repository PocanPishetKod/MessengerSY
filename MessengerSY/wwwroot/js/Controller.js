const contactsMenu = "contacts";
const chatsMenu = "chats";

window.onload = function () {
    if (!messengerSYAuth.isAuthenticate()) {
        document.location.href = "/Login.html";
    }

    var vueMain = VueMain();
    var connection = hubConnectionManager.buildConnection(messengerSYAuth.getAccessToken());
    hubConnectionManager.setClientMethods(connection);
    hubConnectionManager.startConnection(connection);
    hubConnectionManager.connection = connection;

    vueMain.loadUserProfileContacts();
    vueMain.loadUserProfileChats();
};

function scrollToLastMessage() {
    var element = document.getElementById("messages");
    element.firstElementChild.lastElementChild.scrollIntoView();
}

function VueMain() {
    return new Vue({
        el: "#main",
        data: mainData,
        methods: mainMethods,
        computed: mainComputeds
    });
}

var mainData = {
    connection: {},
    messageGUID: 1,
    userProfileId: 5,
    currentMenu: chatsMenu,
    chats: [],
    contacts: [],
    currentChat: { messages: [], typedText: "" }
};

var mainMethods = {
    setContactsMenu: function() {
        mainData.currentMenu = contactsMenu;
    },
    setChatsMenu: function() {
        mainData.currentMenu = chatsMenu;
    },
    loadUserProfileContacts: function() {
        var contacts = loadData.getUserProfileContacts().contacts;
        mainData.contacts = contacts.map(function(contact) {
            return new Contact(contact.contactId, contact.phoneNumber, contact.contactName);
        });
    },
    loadUserProfileChats: function() {
        var chats = loadData.getUserProfileChats().chats;
        mainData.chats = chatBuilder.buildChats(chats);
    },
    loadChatMessages: function(chatId) {
        var chatMessages = loadData.getChatMessages(chatId);
        var chat = chatBuilder.findChat(chatId);
        chat.messages = chatBuilder.buildChatMessages(chatMessages.messages);
    },
    insertYourMessage: function (message, messageGUID) {
        var chat = chatBuilder.findChat(message.chatId);

        var messageIndex = chat.messages.findIndex(function (message) {
            return message.GUID === messageGUID;
        });

        var sender = new UserProfile(message.sender.userProfileId, message.sender.phoneNumber, message.nickname, "Вы");
        var newMessage = new Message(message.messageId, message.chatId, message.sendDate, message.textContent, sender);

        chat.messages.splice(messageIndex, 1);
        this.pushMessageToCurrentChat(newMessage);
        scrollToLastMessage();
    },
    pushMessageToCurrentChat: function(message) {
        mainData.currentChat.messages.push(message);
    },
    insertMessage: function (message) {
        var chat = chatBuilder.findChat(message.chatId);

        var containsMessage = chat.messages.find(function (mes) {
            return mes.messageId === message.messageId;
        });

        if (!containsMessage) {
            if (message.sender.userProfileId === mainData.userProfileId) {
                message.sender.contactName = "Вы";
            } else {

                var contact = chatBuilder.findContact(message.sender.phoneNumber);
                message.sender.contactName = contact
                    ? contact.contactName
                    : message.sender.nickname
                    ? message.sender.nickname
                    : message.sender.phoneNumber;
            }

            pushMessageToCurrentChat(message);
            scrollToLastMessage();
        }
    },
    onSendMessage: function(event) {
        if (event.keyCode === 13) {
            var message = this.createMessage(mainData.currentChat.chatId, mainData.currentChat.typedText);
            this.pushMessageToCurrentChat(message);
            this.changeCurrentChatLastMessage(message);
            hubMethods.sendMessage(message);

            mainData.currentChat.typedText = "";
        }
    },
    createMessage: function(chatId, messageText) {
        var message = new Message(0, chatId, "", messageText, new UserProfile(mainData.userProfileId, "", "", "Вы"));
        message.GUID = mainData.messageGUID;
        mainData.messageGUID++;

        return message;
    },
    changeCurrentChatLastMessage: function(message) {
        mainData.currentChat.lastMessage.textContent = message.textContent;
        mainData.currentChat.lastMessage.sender = message.sender;
    }
};

var mainComputeds = {
    isContactsMenu: function() {
        if (mainData.currentMenu === contactsMenu) {
            return true;
        }

        return false;
    },
    isChatsMenu: function() {
        if (mainData.currentMenu === chatsMenu) {
            return true;
        }

        return false;
    },
    isDisableInputMessage: function() {
        if (mainData.currentChat.chatId) {
            return false;
        } else {
            return true;
        }
    }
};

//Нужно дописать билдеры
var chatBuilder = {
    buildChats: function (chats) {
        return chats.map(function (chat) {
            var participants = mainMethods.buildChatParticipants(chat);

            var sender = chatBuilder.findLastMessageSender(chat, participants);
            if (sender.userProfileId === mainData.userProfileId) {
                sender.contactName = "Вы";
            }

            var lastMessage = new LastMessage(chat.lastMessage.textContent, sender);

            var title = chat.title;
            if (!chat.isGroup) {
                title = participants.find(function (participant) {
                    return participant.userProfileId !== mainData.userProfileId;
                }).contactName;
            }

            return new Chat(chat.chatId, title, chat.isGroup, lastMessage, participants);
        });
    },
    buildChatParticipants: function (chat) {
        return chat.participants.map(function (participant) {
            var contact = GetContactByPhoneNumber(participant.phoneNumber);

            if (!contact) {
                return new UserProfile(participant.userProfileId,
                    participant.phoneNumber,
                    participant.nickname,
                    participant.nickname === "" || !participant.nickname ? participant.phoneNumber : participant.nickname);
            }

            return new UserProfile(participant.userProfileId, participant.phoneNumber, participant.nickname, contact.contactName);
        });
    },
    buildChatMessages: function (chatMessages) {
        return chatMessages.map(function (message) {
            var contactName = "";
            if (message.sender.userProfileId === mainData.userProfileId) {
                contactName = "Вы";
            } else {
                var contact = chatBuilder.findContact(message.sender.phoneNumber);

                if (contact) {
                    contactName = contact.contactName;
                } else {
                    contactName = message.sender.nickname === "" || !message.sender.nickname
                        ? message.sender.phoneNumber
                        : message.sender.nickname;
                }
            }

            var sender = new UserProfile(message.sender.userProfileId,
                message.sender.phoneNumber,
                message.sender.nickname,
                contactName);

            return new Message(message.messageId, message.chatId, message.sendDate, message.textContent, sender);
        });
    },
    findLastMessageSender: function (chat, participants) {
        return participants.find(function (participant) {
            return participant.userProfileId === chat.lastMessage.sender.userProfileId;
        });
    },
    findChat: function (chatId) {
        return mainData.chats.find(function (chat) {
            return chat.chatId === chatId;
        });
    },
    findContact: function(phoneNumber) {
        return mainData.contacts.find(function (contact) {
            return contact.phoneNumber === phoneNumber;
        });
    }
};