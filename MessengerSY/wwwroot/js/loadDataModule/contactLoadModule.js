var contactLoadModule = new function () {
    this.loadAllChats = function () {
        ajaxModule.send(endPointHelper.contactEndPoints.getContactsEndPoint,
            "GET",
            null,
            handlers.loadAllChatsHandlers.successHandler(),
            null,
            null);
    };

    var handlers = new function () {
        this.loadAllChatsHandlers = new function () {
            this.successHandler = new function (responseContacts) {
                var mappedContacts = responseContacts.contacts.map(function (contact) {
                    return new Contact(contact.contactId, contact.phoneNumber, contact.contactName);
                });

                contactRepository.setContacts(mappedContacts);
            };
        };
    };
};