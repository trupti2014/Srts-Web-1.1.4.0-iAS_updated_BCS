/// <reference path="../Global/jquery-1.11.1.min.js" />
/// <reference path="../Global/PassFailConfirm.js" />
/// <reference path="../Global/GlobalVal.js" />
/// <reference path="../Global/SharedAddress.js" />

/*  Person Add  */
function ValidateName(sender, args) {
    var c = sender.controltovalidate.toString(); // txtLastName, tbFirstName, tbMiddleName
    var m = '';
    switch (c) {
        case 'txtLastName':
            m = 'Last Name';
            break;
        case 'tbFirstName':
            m = 'First Name';
            break;
        case 'tbMiddleName':
            m = 'Middle Name';
            break;
    }

    if (args.Value.trim() == '') {
        args.IsValid = c == 'tbMiddleName';
        if (args.IsValid) {
            Pass('tbMiddleName', 'divPersonalData', 'divPersonalDataMsg');
        }
        else {
            Fail(c, 'divPersonalData', 'divPersonalDataMsg', m + ' is a required field.');
        }
        return;
    }

    var regex = /^[a-zA-Z'\s-]{1,40}$/;
    args.IsValid = regex.test(args.Value.trim());

    if (args.IsValid)
        Pass(c, 'divPersonalData', 'divPersonalDataMsg');
    else
        Fail(c, 'divPersonalData', 'divPersonalDataMsg', m + ' either contains invalid characters, or is less than 1 or greater than 40 characters.');
}

function ValidateDate(sender, args) {
    var c = sender.controltovalidate;
    var a = args.Value.trim();
    var msgCtr = c == 'tbEADExpires' ? 'divServiceDataMsg' : 'divPersonalDataMsg';
    var ctr = c == 'tbEADExpires' ? 'divServiceData' : 'divPersonalData';

    if (a == '') {
        args.IsValid = true;
        Pass(c, ctr, msgCtr);
        return;
    }

    var regex = /^(0[1-9]|1[012])\/(0[1-9]|[12][0-9]|3[01])\/(19|20)\d\d$/;
    args.IsValid = regex.test(a);
    if (!args.IsValid) {
        Fail(c, ctr, msgCtr, 'Correct format is MM/DD/YYYY.');
        args.IsValid = false;
        return;
    }
    try {
        //var t = a.split(/\D/);
        var d = new Date(a);
        if (sender.controltovalidate == 'tbDOB') {
            var isFuture = d > new Date();
            if (isFuture) {
                Fail(c, ctr, msgCtr, 'DOB cannot be in the future.');
                args.IsValid = false;
                return;
            }
        }
        else {
            var start = new Date();
            var diff = d - start;
            args.IsValid = diff / 1000 / 60 / 60 / 24 / 365 < 2;
            if (!args.IsValid) {
                Fail(c, ctr, msgCtr, 'EAD cannot be greater than 2 years from current date.');
                args.IsValid = false;
                return;
            }
        }

        Pass(c, ctr, msgCtr);
    } catch (e) {
        args.IsValid = false;
        Fail(c, ctr, msgCtr, 'Date format error.');
    }
}

function IdExists() {
    if ($('#tbSsn').val() != '' || $('#tbDin').val() != '' || $('#tbDbn').val() != '' || $('#tbPin').val() != '')
        Pass('', 'divIdNumber', 'divIdNumMsg');
    else
        Fail('', 'divIdNumber', 'divIdNumMsg', 'At least one ID number is required');
}
function ValidateSsn(sender, args) {
    IdExists();

    if (args.Value.trim() == '') {
        args.IsValid = true;
        Pass('tbSsn', 'divIdNumber', 'divIdNumMsg');
        return;
    }
    var regex = /^[0-9]{9}$/;
    args.IsValid = regex.test(args.Value);
    if (!args.IsValid)
        Fail('tbSsn', 'divIdNumber', 'divIdNumMsg', 'SSN must contain only 9 digits');
    else
        Pass('tbSsn', 'divIdNumber', 'divIdNumMsg');
}
function ValidateDin(sender, args) {
    IdExists();

    if (args.Value.trim() == '') {
        args.IsValid = true;
        Pass('tbDin', 'divIdNumber', 'divIdNumMsg');
        return;
    }
    var regex = /^[0-9]{10}$/;
    args.IsValid = regex.test(args.Value);
    if (!args.IsValid)
        Fail('tbDin', 'divIdNumber', 'divIdNumMsg', 'DOD ID number must contain only 10 digits.');
    else
        Pass('tbDin', 'divIdNumber', 'divIdNumMsg');
}
function ValidatePinDbn(sender, args) {
    IdExists();

    var c = sender.controltovalidate.toString() == 'tbDbn' ? 'tbDbn' : 'tbPin';
    var t = sender.controltovalidate.toString() == 'tbDbn' ? 'DOD Benefit ID' : 'Provider ID';

    if (args.Value.trim() == '') {
        args.IsValid = true;
        Pass(c, 'divIdNumber', 'divIdNumMsg');
        return;
    }
    var regex = /^[0-9]{11}$/;
    args.IsValid = regex.test(args.Value);
    if (!args.IsValid)
        Fail(c, 'divIdNumber', 'divIdNumMsg', t + ' number must contain only 11 digits.');
    else
        Pass(c, 'divIdNumber', 'divIdNumMsg');
}

function ValidateEmail(sender, args) {
    var c = sender.controltovalidate.toString();
    if (args.Value.trim() == '') {
        args.IsValid = true;
        Pass(c, 'divEmailAddresses', 'divEmailMsg');
        return;
    }
    var regex = /^([a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]){1,70}$/;
    args.IsValid = regex.test(args.Value);
    if (!args.IsValid)
        Fail(c, 'divEmailAddresses', 'divEmailMsg', 'Email address is not in correct format');
    else
        Pass(c, 'divEmailAddresses', 'divEmailMsg');
}

function ValidatePhone(sender, args) {
    var c = sender.controltovalidate.toString();
    if (args.Value.trim() == '') {
        args.IsValid = true;
        Pass(c, 'divPhoneNumbers', 'divPhoneNumMsg');
        return;
    }
    var regex = /^[0-9-\-]{7,15}$/;
    args.IsValid = regex.test(args.Value);
    if (!args.IsValid)
        Fail(c, 'divPhoneNumbers', 'divPhoneNumMsg', 'Phone number is not in the correct format');
    else
        Pass(c, 'divPhoneNumbers', 'divPhoneNumMsg');
}
function ValidateExtension(sender, args) {
    var c = sender.controltovalidate.toString();

    if (args.Value.trim() == '') {
        args.IsValid = true;
        Pass(c, 'divPhoneNumbers', 'divPhoneNumMsg');
        return;
    }
    var regex = /^\d{1,7}$/;
    args.IsValid = regex.test(args.Value);
    if (!args.IsValid)
        Fail(c, 'divPhoneNumbers', 'divPhoneNumMsg', 'Extension is not in the correct format');
    else
        Pass(c, 'divPhoneNumbers', 'divPhoneNumMsg');
}

//function ShouldPrimaryAddressValidate() {
//    if ($('#tbPrimaryAddress1').val() == '' &&
//        $('#tbPrimaryCity').val() == '' &&
//        $('#ddlPrimaryState').val() == 'X' &&
//        $('#ddlPrimaryCountry').val() == 'X' &&
//        $('#tbPrimaryZipCode').val() == '') {
//        ClearMsgs('divAddresses', 'divAddressMsg');
//        return false;
//    }
//    else {
//        if ($('#tbPrimaryAddress1').val() == '') { Fail('tbPrimaryAddress1', 'divAddresses', 'divAddressMsg', 'Address 1 is a requred field'); }
//        if ($('#tbPrimaryCity').val() == '') { Fail('tbPrimaryCity', 'divAddresses', 'divAddressMsg', 'City is a required field'); }
//        if ($('#ddlPrimaryState').val() == 'X') { Fail('ddlPrimaryState', 'divAddresses', 'divAddressMsg', 'State is a required field'); }
//        if ($('#ddlPrimaryCountry').val() == 'X') { Fail('ddlPrimaryCountry', 'divAddresses', 'divAddressMsg', 'Country is a required field'); }
//        if ($('#tbPrimaryZipCode').val() == '') { Fail('tbPrimaryZipCode', 'divAddresses', 'divAddressMsg', 'Zip code is a required field'); }
//    }
//    return true;
//}
//function ShouldSecondaryAddressValidate() {
//    if ($('#tbSecondaryAddress1').val() == '' &&
//        $('#tbSecondaryCity').val() == '' &&
//        $('#ddlSecondaryState').val() == 'X' &&
//        $('#ddlSecondaryCountry').val() == 'X' &&
//        $('#tbSecondaryZipCode').val() == '') {
//        ClearMsgs('divAddresses', 'divAddressMsg');
//        return false;
//    }
//    else {
//        if ($('#tbSecondaryAddress1').val() == '') { Fail('tbSecondaryAddress1', 'divAddresses', 'divAddressMsg', 'Address 1 is a requred field'); }
//        if ($('#tbSecondaryCity').val() == '') { Fail('tbSecondaryCity', 'divAddresses', 'divAddressMsg', 'City is a required field'); }
//        if ($('#ddlSecondaryState').val() == 'X') { Fail('ddlSecondaryState', 'divAddresses', 'divAddressMsg', 'State is a required field'); }
//        if ($('#ddlSecondaryCountry').val() == 'X') { Fail('ddlSecondaryCountry', 'divAddresses', 'divAddressMsg', 'Country is a required field'); }
//        if ($('#tbSecondaryZipCode').val() == '') { Fail('tbSecondaryZipCode', 'divAddresses', 'divAddressMsg', 'Zip code is a required field'); }
//    }
//    return true;
//}

//function ValidateAddress1(sender, args) {
//    var c = sender.controltovalidate.toString();
//    if (c.indexOf('Primary') == 0) {
//        if (!ShouldSecondaryAddressValidate()) return;
//    }
//    else {
//        if (!ShouldPrimaryAddressValidate()) return;
//    }
//    var regex = /^[a-zA-Z0-9'.\s-\/]{1,40}$/;
//    args.IsValid = regex.test(args.Value);
//    if (!args.IsValid)
//        Fail(c, 'divAddresses', 'divAddressMsg', 'Address 1 contains invalid characters');
//    else
//        Pass(c, 'divAddresses', 'divAddressMsg');
//}
//function ValidateAddress2(sender, args) {
//    var c = sender.controltovalidate.toString();
//    if (c.indexOf('Primary') == 0) {
//        if (!ShouldSecondaryAddressValidate()) return;
//    }
//    else {
//        if (!ShouldPrimaryAddressValidate()) return;
//    }
//    if (args.Value == '') {
//        Pass(c, 'divAddresses', 'divAddressMsg');
//        args.IsValid = true;
//        return;
//    }
//    var regex = /^[a-zA-Z0-9'.\s-\/]{1,40}$/;
//    args.IsValid = regex.test(args.Value);
//    if (!args.IsValid)
//        Fail(c, 'divAddresses', 'divAddressMsg', 'Address 2 contains invalid characters');
//    else
//        Pass(c, 'divAddresses', 'divAddressMsg');
//}
//function ValidateCity(sender, args) {
//    var c = sender.controltovalidate.toString();
//    if (c.indexOf('Primary') == 0) {
//        if (!ShouldSecondaryAddressValidate()) return;
//    }
//    else {
//        if (!ShouldPrimaryAddressValidate()) return;
//    }
//    var regex = /^[a-zA-Z0-9'.\s-\/]{1,40}$/;
//    args.IsValid = regex.test(args.Value);
//    if (!args.IsValid)
//        Fail(c, 'divAddresses', 'divAddressMsg', 'City contains invalid characters');
//    else
//        Pass(c, 'divAddresses', 'divAddressMsg');
//}
//function ValidateState(sender, args) {
//    var c = sender.controltovalidate.toString();
//    if (c.indexOf('Primary') == 0) {
//        if (!ShouldSecondaryAddressValidate()) return;
//    }
//    else {
//        if (!ShouldPrimaryAddressValidate()) return;
//    }
//    args.IsValid = args.Value != 'X';
//    if (!args.IsValid)
//        Fail(c, 'divAddresses', 'divAddressMsg', 'State is a required field');
//    else
//        Pass(c, 'divAddresses', 'divAddressMsg');
//}
//function ValidateCountry(sender, args) {
//    var c = sender.controltovalidate.toString();
//    if (c.indexOf('Primary') == 0) {
//        if (!ShouldSecondaryAddressValidate()) return;
//    }
//    else {
//        if (!ShouldPrimaryAddressValidate()) return;
//    }
//    args.IsValid = args.Value != 'X';
//    if (!args.IsValid)
//        Fail(c, 'divAddresses', 'divAddressMsg', 'Country is a required field');
//    else
//        Pass(c, 'divAddresses', 'divAddressMsg');
//}
//function ValidateZip(sender, args) {
//    var c = sender.controltovalidate.toString();
//    if (c.indexOf('Primary') == 0) {
//        if (!ShouldSecondaryAddressValidate()) return;
//    }
//    else {
//        if (!ShouldPrimaryAddressValidate()) return;
//    }
//    var regex = /^\d{5}(\-\d{4})?$/;
//    args.IsValid = regex.test(args.Value);
//    if (!args.IsValid)
//        Fail(c, 'divAddresses', 'divAddressMsg', 'Zip code contains invalid characters');
//    else
//        Pass(c, 'divAddresses', 'divAddressMsg');
//}

function ValidateIndividualType() {
    if ($('[id$=cbPatient]').is(':checked')) {
        return true;
    }
    return $('[id$=cbProvider]').is(':checked') || $('[id$=cbTechnician]').is(':checked') || $('[id$=cbAdministrator]').is(':checked');//$('[id$=cbPatient]').is(':checked') || 
}

function CanSubmit(c) {
    return !InErrorState();
}