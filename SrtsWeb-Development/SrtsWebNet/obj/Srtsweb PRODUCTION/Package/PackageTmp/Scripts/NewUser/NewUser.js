/// <reference path="../Global/jquery-1.11.1.min.js" />
/// <reference path="../Global/GlobalVal.js" />
/// <reference path="../Global/PassFailConfirm.js" />

// Ensure there are no invalid characters in the username and password fields
function validateChars(source, arguments) {
    arguments.IsValid = (arguments.Value.match(/^(?=.*[^a-zA-Z0-9\&!@#\$%\^\*\(\)]).*$/) == null);
}

function SearchClick() {
    if (validateID()) 
        return true;
    else
        return false;
}

function validateID() {
    var val = $("#tbIDNumber").val();

    if (val == undefined || val == "") {
        $("#lblIDNumError").text('ID number is required');
        return false;
    }

    if ((val.match(/^\d+$/)) != null && val.length > 8) {
        $("#lblIDNumError").text('');
        return true;
    }
    else {
        $("#lblIDNumError").text('Value must be a 9 or 10 digit number');
        $("#lblName").text('');
        return false;
    }
}

function DoSuccessDialog() {
    var dOpts = {
        autoOpen: false,
        modal: true,
        width: 650,
        height: 350,
        closeOnEscape: false,
        title: 'Successfully Created New User',
        dialogClass: 'generic',
        close: function (event, ui) {
            $(this).dialog('destroy');
            $(this).hide();

            SharedRedirect(window.location.href);
        }
    };

    var d1 = $('#divSuccessDialog').dialog(dOpts);
    d1.dialog('open');
}

// REVISIT HOW TO VALIDATE THE FORM BEFORE SAVING....................................................................................................
//function CanFormSave() {
//    var good = true;
//    if (!DdlRequriedFieldValById('ddlIndividuals')) {
//        good = false;
//        Fail('divError', 'divError', 'divError', 'Individual is a required field.');
//    }
//    else { Pass('divError', 'divError', 'divError'); }
//    if (!DdlRequriedFieldValById('ddlDestinationSiteCodes')) {
//        good = false;
//        Confirm('divError', 'divError', 'divError', 'Destination site code is a required field.');
//    }
//    else { Pass('divError', 'divError', 'divError'); }
//    if ($('#tbEmail').val() == '') {
//        good = false;
//        Confirm('divError', 'divError', 'divError', 'Email is a required field.');
//    }
//    else { Pass('divError', 'divError', 'divError'); }

//    return good;
//}
