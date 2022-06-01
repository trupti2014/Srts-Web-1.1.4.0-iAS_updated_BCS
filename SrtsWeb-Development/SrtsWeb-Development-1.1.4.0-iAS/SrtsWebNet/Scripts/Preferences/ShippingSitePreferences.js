/// <reference path="../Global/jquery-1.11.1.min.js" />
/// <reference path="../Global/PassFailConfirm.js" />

// This is used to capture the partial postback and determine if the success dialog should be shown.
Sys.Application.add_load(function () {
    // The success value is set in code behind to 1.
    if ($('#hfSuccessShipping').val() != '1') return;
    
    // The display message is set in code behind.
    var msg = $('#hfMsgShipping').val();

    // Call function to show message.
    displaySrtsMessage('Success!', msg, 'success');

    // Reset the success hidden field to 0 so not to re-show the success dialog on the next partial postback.
    $('#hfSuccessShipping').val('0');
});
