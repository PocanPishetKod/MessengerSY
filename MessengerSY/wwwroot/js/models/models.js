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

function CurrentChat(chatId, messages, typedText) {
    this.chatId = chatId;
    this.messages = messages;
    this.typedText = typedText;
}