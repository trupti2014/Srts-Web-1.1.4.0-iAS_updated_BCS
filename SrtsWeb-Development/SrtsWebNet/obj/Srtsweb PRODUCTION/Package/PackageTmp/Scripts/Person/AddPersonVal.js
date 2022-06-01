/// <reference path="../Global/jquery-1.11.1.min.js" />
/// <reference path="../Global/PassFailConfirm.js" />
/// <reference path="../Global/GlobalVal.js" />

function ValidateDate(sender, args) {
    var c = sender.controltovalidate;
    var a = args.Value.trim();

    if (a == '') {
        args.IsValid = true;
        Pass(c, 'divAddPerson', 'divAddPersonMsg');
        return;
    }

    var regex = /^(0[1-9]|1[012])\/(0[1-9]|[12][0-9]|3[01])\/(19|20)\d\d$/;
    args.IsValid = regex.test(a);
    if (!args.IsValid) {
        Fail(c, 'divAddPerson', 'divAddPersonMsg', 'Invalid date format.  Correct format is MM/DD/YYYY.');
        return;
    }

    try {
        //var t = a.split(/\D/);
        var d = new Date(a);
        if (sender.controltovalidate == 'tbDOB') {
            var isFuture = d > new Date();
            if (isFuture) {
                Fail(c, 'divAddPerson', 'divAddPersonMsg', 'DOB cannot be in the future.');
                args.IsValid = false;
                return;
            }
        }
        else {
            var start = new Date();
            var diff = d - start;
            args.IsValid = diff / 1000 / 60 / 60 / 24 / 365 < 2;
            if (!args.IsValid) {
                Fail(c, 'divAddPerson', 'divAddPersonMsg', 'EAD cannot be greater than 2 years from current date.');
                args.IsValid = false;
                return;
            }
        }

        Pass(c, 'divAddPerson', 'divAddPersonMsg');
    } catch (e) {
        args.IsValid = false;
        Fail(c, 'divAddPerson', 'divAddPersonMsg', 'Date format error.');
    }
}

function ValidateName(sender, args) {
    var c = sender.controltovalidate.toString();
    var m = '';
    switch (c) {
        case 'tbLastName':
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
            Pass('tbMiddleName', 'divAddPerson', 'divAddPersonMsg');
        }
        else {
            Fail(c, 'divAddPerson', 'divAddPersonMsg', m + ' is a required field.');
        }
        return;
    }

    var regex = /^[a-zA-Z'\s-]{1,40}$/;
    args.IsValid = regex.test(args.Value.trim());

    if (args.IsValid)
        Pass(c, 'divAddPerson', 'divAddPersonMsg');
    else
        Fail(c, 'divAddPerson', 'divAddPersonMsg', m + ' either contains invalid characters, or is less than 1 or greater than 40 characters.');
}

function ValidateEmail(sender, args) {
    var c = sender.controltovalidate.toString();
    if (args.Value.trim() == '') {
        args.IsValid = true;
        Pass(c, 'divAddPerson', 'divAddPersonMsg');
        return;
    }
    var regex = /^([a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]){1,70}$/;
    args.IsValid = regex.test(args.Value);
    if (!args.IsValid)
        Fail(c, 'divAddPerson', 'divAddPersonMsg', 'Email address is not in correct format.');
    else
        Pass(c, 'divAddPerson', 'divAddPersonMsg');
}

function ValidatePhone(sender, args) {
    var c = sender.controltovalidate.toString();
    if (args.Value.trim() == '') {
        args.IsValid = true;
        Pass(c, 'divAddPerson', 'divAddPersonMsg');
        return;
    }
    var regex = /^[0-9-\-]{7,15}$/;
    args.IsValid = regex.test(args.Value);
    if (!args.IsValid)
        Fail(c, 'divAddPerson', 'divAddPersonMsg', 'Phone number is not in the correct format.');
    else
        Pass(c, 'divAddPerson', 'divAddPersonMsg');
}
function ValidateExtension(sender, args) {
    var c = sender.controltovalidate.toString();

    if (args.Value.trim() == '') {
        args.IsValid = true;
        Pass(c, 'divAddPerson', 'divAddPersonMsg');
        return;
    }
    var regex = /^\d{1,7}$/;
    args.IsValid = regex.test(args.Value);
    if (!args.IsValid)
        Fail(c, 'divAddPerson', 'divAddPersonMsg', 'Extension is not in the correct format.');
    else
        Pass(c, 'divAddPerson', 'divAddPersonMsg');
}

function ValidateWholeAddress(sender, args) {
    // Should validate the address
    var skipVal = $('#tbAddress1').val() == '' && $('#tbCity').val() == '' && $('#tbZipCode').val() == '' && !DdlRequriedFieldValById('ddlState');

    if (skipVal) { args.IsValid = true; return; }
    args.IsValid = false;

    sender.innerText += ValidateAddress1('tbAddress1');
    sender.innerText += ValidateAddress2('tbAddress2');
    sender.innerText += ValidateCity('tbCity')
    sender.innerText += ValidateState('ddlState');
    sender.innerText += ValidateCountry('ddlCountry');
    sender.innerText += ValidateZip('tbZipCode');
}
function ValidateAddress1(c) {
    var v = $('#' + c).val();

    if (v == '') {
        $('#' + c).addClass('redBorder');
        return 'Address 1 is a required field.\n\n';
    }

    var regex = /^[a-zA-Z0-9'.\s-\/]{1,40}$/;
    if (!regex.test(v)) {
        $('#' + c).addClass('redBorder');
        return 'Address 1 contains invalid characters.\n\n';
    }
    else {
        $('#' + c).removeClass('redBorder');
        return '';
    }
}
function ValidateAddress2(c) {
    var v = $('#' + c).val();

    var regex = /^[a-zA-Z0-9'.\s-\/]{1,40}$/;
    if (!regex.test(v)) {
        $('#' + c).addClass('redBorder');
        return 'Address 2 contains invalid characters.\n\n';
    }
    else {
        $('#' + c).removeClass('redBorder');
        return '';
    }
}
function ValidateCity(c) {
    var v = $('#' + c).val();

    if (v == '') {
        $('#' + c).addClass('redBorder');
        return 'City is a required field.\n\n';
    }

    var regex = /^[a-zA-Z0-9'.\s-\/]{1,40}$/;
    if (!regex.test(v)) {
        $('#' + c).addClass('redBorder');
        return 'City contains invalid characters.\n\n';
    }
    else {
        $('#' + c).removeClass('redBorder');
        return '';
    }
}
function ValidateState(c) {
    var v = $('#' + c).find('option:selected').val();

    if (v == 'X') {
        $('#' + c).addClass('redBorder');
        return 'State is a required field.\n\n';
    }
    else {
        $('#' + c).removeClass('redBorder');
        return '';
    }
}
function ValidateCountry(c) {
    var v = $('#' + c).find('option:selected').val();

    if (v == 'X') {
        $('#' + c).addClass('redBorder');
        return 'Country is a required field.\n\n';
    }
    else {
        $('#' + c).removeClass('redBorder');
        return '';
    }
}
function ValidateZip(c) {
    var v = $('#' + c).val();

    if (v == '') {
        $('#' + c).addClass('redBorder');
        return 'Zip code is a required field.\n\n';
    }

    var regex = /^\d{5}(\-\d{4})?$/;
    if (regex.test(v)) {
        $('#' + c).addClass('redBorder');
        return 'Zip code contains invalid characters.\n\n';
    }
    else {
        $('#' + c).removeClass('redBorder');
        return '';
    }
}

function ValidateBos(sender, args) {
    if (DdlRequriedFieldValControl(sender, args))
        Pass(sender.controltovalidate, 'divAddPerson', 'divAddPersonMsg');
    else
        Fail(sender.controltovalidate, 'divAddPerson', 'divAddPersonMsg', 'Branch of Service is Required.');
}
function ValidateStatus(sender, args) {
    if (DdlRequriedFieldValControl(sender, args))
        Pass(sender.controltovalidate, 'divAddPerson', 'divAddPersonMsg');
    else
        Fail(sender.controltovalidate, 'divAddPerson', 'divAddPersonMsg', 'Status is Required.');
}
function ValidateGrade(sender, args) {
    if (DdlRequriedFieldValControl(sender, args))
        Pass(sender.controltovalidate, 'divAddPerson', 'divAddPersonMsg');
    else
        Fail(sender.controltovalidate, 'divAddPerson', 'divAddPersonMsg', 'Grade is Required.');
}

function canSave() {
    if (!$('#cbTechnician').is(':checked') || $('#cbProvider').is(':checked') || $('#cbAdministrator').is(':checked')) return false;
    return true;
}