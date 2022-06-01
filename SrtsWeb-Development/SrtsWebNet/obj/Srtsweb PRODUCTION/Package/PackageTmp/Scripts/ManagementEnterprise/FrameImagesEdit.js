/// <reference path="../Global/jquery-1.11.1.min.js" />


// This is used to capture the partial postback and determine if the success dialog should be shown.
Sys.Application.add_load(function () {
    var ImageSuccess = $('#hfeSuccessFrames').val();
    var msg = $('#hfeMsgFrames').val();
    if ((ImageSuccess == '' || ImageSuccess == '0') && msg != '') {
        displaySrtsMessage('Error!', msg, 'error');
    }
    else if (ImageSuccess == '1') {
        // Call function to show message.
        displaySrtsMessage('Success!', msg, 'success');
    }

    // Reset the success hidden field to 0 so not to re-show the success dialog on the next partial postback.
    $('#hfeSuccessFrames').val('0');
    $('#hfeMsgFrames').val('');
});

