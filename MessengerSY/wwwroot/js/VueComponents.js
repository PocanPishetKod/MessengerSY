Vue.component('chatdetails',
    {
        props: ["chat"],
        methods: {
            selectChat: function (chat) {
                if (chat && !chat.messages) {
                    mainData.currentChat = chat;
                    mainMethods.loadChatMessages(chat.chatId);
                    scrollToLastMessage();
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