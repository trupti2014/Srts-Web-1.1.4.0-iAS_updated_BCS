/// <reference path="../Global/jquery-1.11.1.min.js" />


// This is used to capture the partial postback and determine if the success dialog should be shown.
Sys.Application.add_load(function () {
    var AddressSuccess = $('#hfSuccessGE').val();
    var msg = $('#hfMsgGE').val();
    if ((AddressSuccess == '' || AddressSuccess == '0') && msg != '') {
        displaySrtsMessage('Error!', msg, 'error');
    }
    else if (AddressSuccess == '1') {
        // Call function to show message.
        displaySrtsMessage('Success!', msg, 'success');
    }

    // Reset the success hidden field to 0 so not to re-show the success dialog on the next partial postback.
    $('#hfSuccessGE').val('0');
    $('#hfMsgGE').val('');
});

