var mainData = new function () {
    this.contacts = [];
    this.chats = [];
    this.userProfile = {};
    this.currentMenu = "";
    this.currentChat = { chatId: 0, messages: [], typedText: "" };
    this.messageGUID = 1;
};