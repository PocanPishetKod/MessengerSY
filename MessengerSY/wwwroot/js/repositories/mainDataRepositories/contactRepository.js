var contactRepository = new function () {
    this.addContact = function (contact) {
        if (!contact) {
            throw new Error();
        }

        mainData.contacts.push(contact);
    };

    this.setContacts = function (contacts) {
        if (!contacts) {
            throw new Error();
        }

        if (mainData.contacts.length !== 0) {
            mainData.contacts = contacts;
        }
    };

    this.getContactByPhoneNumber = function (phoneNumber) {
        if (!phoneNumber) {
            throw new Error("phoneNumber");
        }

        return mainData.contacts.find(contact => {
            contact.phoneNumber === phoneNumber;
        });
    };
};