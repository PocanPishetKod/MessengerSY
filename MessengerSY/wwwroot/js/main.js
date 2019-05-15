const contactsMenu = "contacts";
const chatsMenu = "chats";
const developDomen = "https://localhost:44320";

function Chat(chatId, title, isGroup, lastMessage, participants) {
    this.chatId = chatId;
    this.title = title;
    this.isGroup = isGroup;
    this.lastMessage = lastMessage;
    this.participants = participants;
    this.messages = null;
    this.typedText = "";
}

function ReceiveChat(chatId, title, isGroup, message, participants) {
    this.chatId = chatId;
    this.title = title;
    this.isGroup = isGroup;
    this.message = message;
    this.participants = participants;
}

function Message(messageId, chatId, sendDate, textContent, sender) {
    this.messageId = messageId;
    this.chatId = chatId;
    this.sendDate = sendDate;
    this.textContent = textContent;
    this.sender = sender;
}

function LastMessage(textContent, sender) {
    this.textContent = textContent;
    this.sender = sender;
}

function UserProfile(userProfileId, phoneNumer, nickname, contactName) {
    this.userProfileId = userProfileId;
    this.phoneNumer = phoneNumer;
    this.nickname = nickname;
    this.contactName = contactName;
}

function Contact(contactId, phoneNumber, contactName) {
    this.contactId = contactId;
    this.phoneNumber = phoneNumber;
    this.contactName = contactName;
}

function ScrollToLastMessage() {
    var element = document.getElementById("messages");
    element.firstElementChild.lastElementChild.scrollIntoView();
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
    isDisableInputMessage: function () {
        if (mainData.currentChat.chatId) {
            return false;
        } else {
            return true;
        }
    }
};

Vue.component('chatdetails',
    {
        props: ["chat"],
        methods: {
            selectChat: function (chat) {
                if (chat && !chat.messages) {
                    mainData.currentChat = chat;
                    LoadChatMessages(chat.chatId);
                    ScrollToLastMessage();
                }
            }
        },
        template: `<div class="row" v-on:click="selectChat(chat)">
                            <div class="col">
                                <div class="card w-100">
                                    <div class="card-body">
                                        <img style="float: left" class="rounded-circle" width="50" height="50" src="images/Безымянный.png" />
                                        <div style="float: left; margin-left: 10%;">
                                            <h5 class="card-title m-0">{{chat.title}}</h5>
                                            <span class="card-link badge badge-secondary">{{chat.lastMessage.sender.contactName}}: {{chat.lastMessage.textContent}}</span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>`
    });

Vue.component('chatlist',
    {
        props: ["chats"],
        template: `<div><chatdetails v-for="chat in chats" :chat="chat"></chatdetails></div>`
    });

Vue.component('contactdetails',
    {
        props: ["contact"],
        template: `<div class="row">
                            <div class="col">
                                <div class="card w-100">
                                    <div class="card-body">
                                        <img style="float: left" class="rounded-circle" width="50" height="50" src="images/Безымянный.png" />
                                        <div style="float: left; margin-left: 10%;">
                                            <h5 class="card-title m-0">{{contact.contactName}}</h5>
                                            <span class="card-link badge badge-secondary">Написать сообщене</span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>`
    });

Vue.component('contactlist',
    {
        props: ["contacts"],
        template: `<div><contactdetails v-for="contact in contacts" :contact="contact"></contactdetails></div>`
    });

Vue.component('messagedetails',
    {
        props: ["message"],
        template: `<div class="row">
                                    <div class="col">
                                        <div class="card">
                                            <div class="card-body">
                                                <h6 class="card-subtitle mb-2 text-muted">{{message.sender.contactName}}:</h6>
                                                <p class="card-text">{{message.textContent}}</p>
                                            </div>
                                        </div>
                                    </div>
                                </div>`
    });

Vue.component('messagelist',
    {
        props: ["messages"],
        template: `<div><messagedetails v-for="message in messages" :message="message"></messagedetails></div>`
    });

new Vue({
    el: "#main",
    data: mainData,
    methods: mainMethods,
    computed: mainComputeds
});

window.onload = function () {
    alert(GetSavedToken());
    //if (!GetSavedToken()) {
    //    document.location.href = "/login.html";
    //}

    _token = GetSavedToken();

    mainData.connection = ConnectToHub();

    LoadUserProfileContacts();
    LoadUserProfilesChats();
};

var _token = "";

function GetMomToken() {
    $.ajax({
        url: developDomen + "/api/account/testgetjwtmom",
        type: "GET",
        dataType: "json",
        async: false,
        success: function (token) {
            _token = token;
        }
    });
}

function GetToken() {
    $.ajax({
        url: developDomen + "/api/account/testgetjwtmy",
        type: "GET",
        dataType: "json",
        async: false,
        success: function (token) {
            _token = token;
        }
    });
}

function SaveToken(token) {
    localStorage.removeItem("token");
    localStorage.setItem("token", token);
}

function GetSavedToken() {
    return localStorage.getItem("jwt");
}

function GetLocalToken() {
    return _token;
}

function SetAuthorizationHeader(xhr) {
    xhr.setRequestHeader("Authorization", "Bearer " + _token);
}

function LoadUserProfileContacts() {
    $.ajax({
        url: developDomen + "/api/contact/getcontacts",
        type: "GET",
        dataType: "json",
        async: false,
        beforeSend: function (xhr) {
            SetAuthorizationHeader(xhr);
        },
        success: function (data) {
            mainData.contacts = data.contacts.map(function (contact) {
                return new Contact(contact.contactId, contact.phoneNumber, contact.contactName);
            });
        }
    });
}

function GetContactByPhoneNumber(phoneNumber) {
    return mainData.contacts.find(function (contact) {
        return contact.phoneNumber === phoneNumber;
    });
}

function LoadUserProfilesChats() {
    $.ajax({
        url: developDomen + "/api/chat/getchats",
        type: "GET",
        dataType: "json",
        async: false,
        beforeSend: function(xhr) {
            SetAuthorizationHeader(xhr);
        },
        success: function (data) {
            mainData.chats = data.chats.map(function (chat) {
                var participants = chat.participants.map(function (participant) {
                    var contact = GetContactByPhoneNumber(participant.phoneNumber);
                    
                    if (!contact) {
                        return new UserProfile(participant.userProfileId,
                            participant.phoneNumber,
                            participant.nickname,
                            participant.nickname === "" || !participant.nickname ? participant.phoneNumber : participant.nickname);
                    }
                    
                    return new UserProfile(participant.userProfileId, participant.phoneNumber, participant.nickname, contact.contactName);
                });

                var sender = participants.find(function (participant) {
                    return participant.userProfileId === chat.lastMessage.sender.userProfileId;
                });

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
        }
    });
}

function LoadChatMessages(chatId) {
    $.ajax({
        url: developDomen + "/api/chat/getchatmessages",
        type: "GET",
        dataType: "json",
        beforeSend: function (xhr) {
            SetAuthorizationHeader(xhr);
        },
        data: { chatid: chatId, count: 1000, startdate: null },
        contentType: "application/json",
        success: function (data) {
            var chat = mainData.chats.find(function (chat) {
                return chat.chatId === chatId;
            });

            chat.messages = data.messages.map(function(message) {
                var contactName = "";
                if (message.sender.userProfileId === mainData.userProfileId) {
                    contactName = "Вы";
                } else {
                    var contact = mainData.contacts.find(function(contact) {
                        return contact.phoneNumber === message.sender.phoneNumber;
                    });

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
        }
    });
}

function ConnectToHub() {
    var connection = new signalR.HubConnectionBuilder()
        .withUrl("hubs/main", { accessTokenFactory: () => GetLocalToken() })
        .build();

    ConfigurateConnection(connection);
    StartConnection(connection);

    return connection;
}

function StartConnection(connection) {
    connection.start().then(function() {
        console.log("started");
    }).catch(function (error) {
        console.log("error");
        console.log(error);
    });
}

function ConfigurateConnection(connection) {
    connection.on("ReceiveMessage",
        (message) => {
            InsertMessage(message);
        });

    connection.on("ReceiveYourMessage",
        (message, messageGUID) => {
            console.log(message);
            InsertYourMessage(message, messageGUID);
        });

    connection.onclose(function () {
        console.log("restart");
        StartConnection(connection);
    });
}

function InsertMessage(message) {
    var chat = mainData.chats.find(function(chat) {
        return chat.chatId === message.chatId;
    });

    var ContainsMessage = chat.messages.find(function(mes) {
        return mes.messageId === message.messageId;
    });

    if (!ContainsMessage) {
        if (message.sender.userProfileId === mainData.userProfileId) {
            message.sender.contactName = "Вы";
        } else {

            var contact = GetContactByPhoneNumber(message.sender.phoneNumber);
            message.sender.contactName = contact
                ? contact.contactName
                : message.sender.nickname
                ? message.sender.nickname
                : message.sender.phoneNumber;
        }

        PushMessageToCurrentChat(message);
        ScrollToLastMessage();
    }
}

function InsertYourMessage(message, messageGUID) {
    var chat = mainData.chats.find(function (chat) {
        return chat.chatId === message.chatId;
    });

    var messageIndex = chat.messages.findIndex(function (message) {
        return message.GUID === messageGUID;
    });

    var sender = new UserProfile(message.sender.userProfileId, message.sender.phoneNumber, message.nickname, "Вы");
    var newMessage = new Message(message.messageId, message.chatId, message.sendDate, message.textContent, sender);

    chat.messages.splice(messageIndex, 1);
    PushMessageToCurrentChat(newMessage);
    ScrollToLastMessage();
}

function PushMessageToCurrentChat(message) {
    mainData.currentChat.messages.push(message);
}

function CreateMessage(chatId, messageText) {
    var message = new Message(0, chatId, "", messageText, new UserProfile(mainData.userProfileId, "", "", "Вы"));
    message.GUID = mainData.messageGUID;
    mainData.messageGUID++;

    return message;
}

function SendMessage(message) {
    mainData.connection.invoke("SendMessage", message.chatId, message.textContent, message.GUID).catch(function (error) {
        console.log("Ошибка");
        console.log(error);
    });
}

function OnSendMessage(event) {
    if (event.keyCode === 13) {
        var message = CreateMessage(mainData.currentChat.chatId, mainData.currentChat.typedText);
        PushMessageToCurrentChat(message);
        ChangeCurrentChatLastMessage(message);
        SendMessage(message);

        mainData.currentChat.typedText = "";
    }
}

function ChangeCurrentChatLastMessage(message) {
    mainData.currentChat.lastMessage.textContent = message.textContent;
    mainData.currentChat.lastMessage.sender = message.sender;
}