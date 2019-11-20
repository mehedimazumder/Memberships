$(function() {
    var pwdLinkHover = $('#pwdLink').hover(onCloseLink);
    var resetPwd = $('#resetPwd').click(onResetPassword);

    function onCloseLink() {
        $('div[data-login-user-area]').removeClass('open');
    };

    function onResetPassword() {
        var email = $('.modal-dialog .reset-email').val();
        var antiforgery = $('[name="__RequestVerificationToken"]').val();
        var url = "/Account/ForgotPasswordConfirmation";

        $.post("/Accout/ForgotPassword",
             {
                _RequestVerificationToken: antiforgery,
                email: email
            },
            function(data) {
                location.href = url;
            }).fail(function(xhr, status, error) {
            location.href = url;

        });
    }
});