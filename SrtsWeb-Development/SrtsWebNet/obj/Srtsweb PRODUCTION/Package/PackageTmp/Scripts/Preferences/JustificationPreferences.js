/// <reference path="../Global/jquery-1.11.1.min.js" />

//$(function () {
//    $('#tbRedirect').change(function (e) {
//        $('#bDeleteRedirect').toggle($(this).val() != '');
//    });
//    $('#tbReject').change(function (e) {
//        $('#bDeleteReject').toggle($(this).val() != '');
//    });
//});

//function pageLoad() {
//    if ($('#tbRedirect').val() == '')
//        $('#bDeleteRedirect').toggle();

//    if ($('#tbReject').val() == '')
//        $('#bDeleteReject').toggle();
//}

//function ToggleDeleteButtons(isReject) {
//    if (isReject) { }
//    else { }
//}

// This is used to capture the partial postback and determine if the success dialog should be shown.
Sys.Application.add_load(function () {
    //// The success value is set in code behind to 1.
    if ($('#hfSuccessLabJust').val() != '1') return;

    // The display message is set in code behind.
    var msg = $('#hfMsgLabJust').val();

    // Call function to show message.
    displaySrtsMessage('Success!', msg, 'success');

    // Reset the success hidden field to 0 so not to re-show the success dialog on the next partial postback.
    $('#hfSuccessLabJust').val('0');
    $('#hfMsgLabJust').val('');
});
