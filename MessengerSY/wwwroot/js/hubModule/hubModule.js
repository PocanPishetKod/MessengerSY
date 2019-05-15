var hubModule = new function () {
    var hub = null;

    this.start = function (access_token) {
        if (!access_token || access_token === "") {
            return new Error("access_token");
        }

        hub = buildHub(access_token);
        setHandlers(hub);
        startHub(hub);
    };

    function startHub(connection) {
        connection.start().then(function () {
            console.log("started");
        }).catch(function (error) {
            console.log("error");
        });
    }

    function setHandlers(connection) {
        connection.on("ReceiveMessage", handlers.receiveMessageHandler);

        connection.on("ReceiveYourMessage", handlers.receiveYourMessage);

        connection.onclose(function () {
            console.log("restart");
            startHub(connection);
        });
    }

    function buildHub(access_token) {
        var connection = new signalR.HubConnectionBuilder()
            .withUrl(endPointHelper.hubEndPoints.getMainHubEndPoint(), { accessTokenFactory: () => access_token })
            .build();

        return connection;
    }

    var handlers = new function () {
        this.receiveMessageHandler = function (message) {
            let sender = chatRepository.getChatParticipant(message.chatId, message.sender.userProfileId);
            chatRepository.addMessage(message.chatId,
                new Message(message.messageId, message.chatId, message.sendDate, message.textContent, sender));
        };

        // replaceMessage возвращает true или false
        this.receiveYourMessage = function (message, GUID) {
            let sender = chatRepository.getChatParticipant(message.chatId, message.sender.userProfileId);
            chatRepository.replaceMessage(message.chatId, GUID,
                new Message(message.messageId, message.chatId, message.sendDate, message.textContent, sender));
        };
    };

    this.methods = new function () {
        this.sendMessage = function (message) {
            hubConnectionManager.connection.invoke("SendMessage", message.chatId, message.textContent, message.GUID).catch(function (error) {
                console.log("Ошибка");
                console.log(error);
            });
        };
    };
};