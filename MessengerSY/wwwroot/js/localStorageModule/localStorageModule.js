var localStorageModule = new function () {
    this.add = function (key, value) {
        if (!key || !value) {
            throw Error("error type");
        }

        if (key.trim() === "" || value.trim() === "") {
            throw new SyntaxError("empty value");
        }

        localStorage.setItem(key, value);
    };

    this.get = function (key) {
        if (!key || key.trim() === "") {
            throw new Error("error type");
        }

        localStorage.getItem(key);
    };

    this.remove = function (key) {
        if (!key || key.trim() === "") {
            throw new Error("error type");
        }

        localStorage.removeItem(key);
    };
};