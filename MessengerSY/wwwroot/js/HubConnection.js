var hubConnectionManager = {
    connection: {},
    buildConnection: function (access_token) {
        return connection = new signalR.HubConnectionBuilder()
            .withUrl("hubs/main", { accessTokenFactory: () => access_token })
            .build();
    },
    startConnection: function(connection) {
        connection.start().then(function () {
            console.log("started");
        }).catch(function (error) {

        });
    },
    setClientMethods: function(connection) {
        connection.on("ReceiveMessage",
            (message) => {
                mainMethods.insertMessage(message);
            });

        connection.on("ReceiveYourMessage",
            (message, messageGUID) => {
                mainMethods.insertYourMessage(message, messageGUID);
            });

        connection.onclose(function () {
            console.log("restart");
            hubConnectionManager.startConnection(connection);
        });
    }
};

var hubMethods = {
    sendMessage: function (message) {
        hubConnectionManager.connection.invoke("SendMessage", message.chatId, message.textContent, message.GUID).catch(function (error) {
            console.log("Ошибка");
            console.log(error);
        });
    }
}