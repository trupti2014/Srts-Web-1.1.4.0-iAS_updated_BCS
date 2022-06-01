/// <reference path="jquery-1.11.1.min.js" />

//$(function () {
//    $('form').on('submit', function () {
        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(DisableBtn);
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EnableBtn);

        function DisableBtn(sender, arg) {
            $('input:submit, button').prop('disabled', true);
        };

        function EnableBtn(sender, arg) {
            $('input:submit, button').removeProp('disabled');
        };
//    });
//});