﻿$(function() {
    var code = $(".register-code-panel input");

    function displayMessage(success, message) {
        var alert_div = $(".register-code-panel .alert");
        alert_div.text(message);

        if (success)
            alert_div.removeClass('alert-danger').addClass('alert-success');
        else
            alert_div.removeClass('alert-success').addClass('alert-danger');

        alert_div.removeClass('hidden');
    }

    $(".register-code-panel button").click(function(e) {
        $(".register-code-panel .alert").addClass('hidden');

        if (code.val().length == 0) {
            displayMessage(false, "Enter a code");
            return;
        }

        $.post('/RegisterCode/Register/',
            { code: code.val() },
            function(data) {
                displayMessage(true, "Successfully Added. \n\r Please Reload Page.");
                code.val(' ');

            }).fail(function(xlr, status, error) {
            displayMessage(false, "Could not register!");

        });
    });
})