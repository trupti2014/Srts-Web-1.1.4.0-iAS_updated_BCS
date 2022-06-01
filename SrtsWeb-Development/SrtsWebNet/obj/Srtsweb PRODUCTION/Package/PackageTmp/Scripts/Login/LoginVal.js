/// <reference path="../Global/jquery-1.11.1.min.js" />
/// <reference path="../Global/jquery-ui.min.js" />
/// <reference path="../Global/SrtsCustomValidators.js" />

function validateChars(source, arguments) {
    arguments.IsValid = (arguments.Value.match(/^(?=.*[^a-zA-Z0-9\.\&!@#\$%\^\*\(\)]).*$/) == null);
}

function validatePasswordChars(source, arguments) {
    var c = '';
    var sc = source.controltovalidate;
    if (sc.indexOf('CurrentPassword') > 0)
        c = 'Current Password';
    else if (sc.indexOf('ConfirmNewPassword') > 0)
        c = 'Confirm New Password';
    else if (sc.indexOf('NewPassword') > 0)
        c = 'New Password';

    if (c == '')
        c = 'Password';

    var lbl = '';
    if (!MinTwoLowerCaseLetters(arguments.Value)) {
        lbl += c + ' requires at least 2 lower case letters.<br />';
        arguments.IsValid = false;
    }

    if (!MinTwoNumbers(arguments.Value)) {
        lbl += c + ' requires at least 2 numbers.<br />';
        arguments.IsValid = false;
    }

    if (!MinTwoSpecialCharacters(arguments.Value)) {
        lbl += c + ' requires at least 2 special characters.<br />';
        arguments.IsValid = false;
    }

    if (!MinTwoUpperCaseLetters(arguments.Value)) {
        lbl += c + ' requires at least 2 upper case letters.<br />';
        arguments.IsValid = false;
    }

    if (arguments.Value.match(/^(?=.*[^a-zA-Z0-9\&!@#\$%\^\*\(\)]).*$/) != null) {
        lbl += c + ' contains some invalid characters.<br />';
        arguments.IsValid = false;
    }
    $('#lblChangeError').html(lbl);
}
