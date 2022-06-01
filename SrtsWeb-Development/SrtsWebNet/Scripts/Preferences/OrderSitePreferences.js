/// <reference path="../Global/jquery-1.11.1.min.js" />
/// <reference path="../Global/PassFailConfirm.js" />

function ValPriorityPreferences() {
    if ($('#ddlPpPriority').find('option:selected').index() <= 0) { return Pass('ddlPpPriority', 'divPriorityPreferences', 'divPriorityPreferencesError'); }

    if ($('#ddlPpFrame').find('option:selected').index() <= 0) { return Fail('ddlPpFrame', 'divPriorityPreferences', 'divPriorityPreferencesError', 'Frame is required when a priority is selected.'); }

    return Pass('ddlPpFrame', 'divPriorityPreferences', 'divPriorityPreferencesError');
}

// This is used to capture the partial postback and determine if the success dialog should be shown.
Sys.Application.add_load(function () {
    var orderSuccess = $('#hfSuccessOrders').val();
    var msg = $('#hfMsgOrders').val();
    if ((orderSuccess == '' || orderSuccess == '0') && msg != '') {
        displaySrtsMessage('Error!', msg, 'error');
    }
    else if (orderSuccess == '1') {
        // Call function to show message.
        displaySrtsMessage('Success!', msg, 'success');
    }

    // Reset the success hidden field to 0 so not to re-show the success dialog on the next partial postback.
    $('#hfSuccessOrders').val('0');
    $('#hfMsgOrders').val('');
});