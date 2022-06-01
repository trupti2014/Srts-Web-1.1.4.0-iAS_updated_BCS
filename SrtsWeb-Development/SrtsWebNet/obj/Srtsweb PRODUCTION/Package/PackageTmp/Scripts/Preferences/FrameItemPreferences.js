/// <reference path="../Global/jquery-1.11.1.min.js" />
/// <reference path="../Global/PassFailConfirm.js" />

function SetSetHts() {
    var v = $('#ddlLens').val();
    if (v.substring(0, 2) == 'SV' || v == 'N' || v == 'X') {
        $('#tbOdSegHt').val('');
        $('#tbOsSegHt').val('');
        $('#tbOdSegHt').attr('disabled', true);
        $('#tbOsSegHt').attr('disabled', true);
    }
    else{
        $('#tbOdSegHt').removeAttr('disabled');
        $('#tbOsSegHt').removeAttr('disabled');
    }

    SegHtVal('tbOdSegHt');
    SegHtVal('tbOsSegHt');
}

function SegHtVal(ctrlId) {
    if ($('#' + ctrlId).is('disabled')) {return Pass(ctrlId, 'divFramePreference', 'divFramePreferenceError'); }

    var val = $('#' + ctrlId).val();
    if (val == '' || (val >= 10 && val <= 35) || val.toLowerCase() == '3b' || val.toLowerCase() == '4b')
        return Pass(ctrlId, 'divFramePreference', 'divFramePreferenceError');
    return Fail(ctrlId, 'divFramePreference', 'divFramePreferenceError', 'Segment height is invalid, valid values are 10 - 35, 3B, or 4B.');
}

// This is used to capture the partial postback and determine if the success dialog should be shown.
Sys.Application.add_load(function () {
    // The success value is set in code behind to 1.
    if ($('#hfSuccessFrames').val() != '1') return;
    
    // The display message is set in code behind.
    var msg = $('#hfMsgFrames').val();

    // Call function to show message.
    displaySrtsMessage('Success!', msg, 'success');

    // Reset the success hidden field to 0 so not to re-show the success dialog on the next partial postback.
    $('#hfSuccessFrames').val('0');
});
