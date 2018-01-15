var ajax = {

    setDebugMode: function() {
        this.debugMode = true;
    },

    get: function(requestUrl, data, loaderImageId, onSuccess, onComplete) {

        if (loaderImageId) {
            $("#" + loaderImageId).css('display', 'inline');
        }

        var completeFunction = function() {
            if (onComplete) {
                onComplete();
            }
            if (loaderImageId) {
                $("#" + loaderImageId).css('display', 'none');
            }
        };

        $.ajax({
            type: "GET",
            url: requestUrl,
            data: data,
            context: this,
            success: function(response) {
                onSuccess(response);
                completeFunction();
            },
            error: function(response) {
                alert("Ajax Request Failed");
                this.showError(response);
                completeFunction();
            }
        });
    },

    post: function(requestUrl, data, loaderImageId, onSuccess, onComplete) {

        if (loaderImageId) {
            $("#" + loaderImageId).css('display', 'inline');
        }

        var completeFunction = function() {
            if (onComplete) {
                onComplete();
            }
            if (loaderImageId) {
                $("#" + loaderImageId).css('display', 'none');
            }
        };

        $.ajax({
            type: "POST",
            url: requestUrl,
            context: this,
            data: data,
            success: function(response) {
                onSuccess(response);
                completeFunction();
            },
            error: function(response) {
                alert("Ajax Request Failed");
                this.showError(response);
                completeFunction();
            }
        });
    },

    showError: function(requestObject) {
        if (this.debugMode) {
            alert(requestObject.responseText);
            return requestObject;
        }
    }
};