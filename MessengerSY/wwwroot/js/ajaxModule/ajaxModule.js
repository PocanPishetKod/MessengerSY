var ajaxModule = new function () {
    this.send = function (url, method, data, success, error, statusCodes) {
        $.ajax({
            url: url,
            method: method,
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify(data),
            success: success,
            error: error,
            statusCode: statusCodes,
            beforeSend: setAuthorizationHeader(xhr)
        });
    };

    function setAuthorizationHeader(xhr) {
        if (access_token_module.accessTokenExists) {
            let access_token = access_token_module.getAccessToken();
            xhr.setRequestHeader("Authorization", "Bearer " + access_token);
        }
    }
};