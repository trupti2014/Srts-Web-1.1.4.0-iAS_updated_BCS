/// <reference path="../jquery-1.11.1.min.js" />
/// <reference path="../Global/PassFailConfirm.js" />
/// <reference path="../Global/GlobalVal.js" />

function ValidateIdNum() {
    var good = true;
    var id = $('[id$=tbIDNumber]').val();
    var idType = $('[id$=ddlIDNumberType] option:selected').val();

    if (id == '') good = Fail('tbIDNumber', 'divAddPatientIdNumber', 'divIdNum', 'ID Number is a required field.');
    else Pass('tbIDNumber', 'divAddPatientIdNumber', 'divIdNum');
    if (!DdlRequriedFieldVal('ddlIDNumberType')) good = Fail('ddlIDNumberType', 'divAddPatientIdNumber', 'divIdNum', 'ID Number Type is a required field.');
    else Pass('ddlIDNumberType', 'divAddPatientIdNumber', 'divIdNum');

    switch (idType) {
        case 'DIN':
            if (id.length != 10 || id.match(/[0-9]{10}/) == null)
                good = Fail('tbIDNumber', 'divAddPatientIdNumber', 'divIdNum', 'DOD ID Number must be 10 digits long.');
            else Pass('tbIDNumber', 'divAddPatientIdNumber', 'divIdNum');
            break;
        case 'SSN':
            if (id.length != 9 || id.match(/[0-9]{9}/) == null)
                good = Fail('tbIDNumber', 'divAddPatientIdNumber', 'divIdNum', 'SSN must be 9 digits long.');
            else Pass('tbIDNumber', 'divAddPatientIdNumber', 'divIdNum');

            break;
        case 'PIN':
        case 'DBN':
            if (id.length != 11 || id.match(/[0-9]{11}/) == null) {
                var t = '';
                if (idType == 'PIN')
                    t = 'Provider ID Number';
                else
                    t = 'Benefits Number';
                good = Fail('tbIDNumber', 'divAddPatientIdNumber', 'divIdNum', t + ' must be 11 digits long.');
            }
            else Pass('tbIDNumber', 'divAddPatientIdNumber', 'divIdNum');
            break;
    }
    return good;
}

function ValidateAddress(ctl) {
    var good = true;
    var addy1 = $('[id$=tbAddress1]').val();
    var addy2 = $('[id$=tbAddress2]').val();
    var city = $('[id$=tbCity]').val();
    var state = $('[id$=ddlState] option:selected').val();
    var zip = $('[id$=tbZipCode]').val();
    var country = $('[id$=ddlCountry] option:selected').val();
    var type = $('[id$=ddlAddressType] option:selected').val();

    if (ctl == 'tbAddress1' || ctl == 'all') {
        if (addy1 == '') good = Fail('tbAddress1', 'divAddMailingAddress', 'divMailAddress', 'Address1 is a required field.');
        else {
            Pass('tbAddress1', 'divAddMailingAddress', 'divMailAddress');
            if (addy1.match(/^[a-zA-Z0-9'.\s-\/]{1,40}$/) == null) good = Fail('tbAddress1', 'divAddMailingAddress', 'divMailAddress', 'Address1 contains invalid characters.');
            else Pass('tbAddress1', 'divAddMailingAddress', 'divMailAddress');
        }
    }

    if (ctl == 'tbAddress2' || ctl == 'all') {
        if (addy2 != '' && addy2.match(/^[a-zA-Z0-9'.\s-\/]{1,40}$/) == null) good = Fail('tbAddress2', 'divAddMailingAddress', 'divMailAddress', 'Address2 contains invalid characters.');
        else Pass('tbAddress2', 'divAddMailingAddress', 'divMailAddress');
    }

    if (ctl == 'tbCity' || ctl == 'all') {
        if (city == '') good = Fail('tbCity', 'divAddMailingAddress', 'divMailAddress', 'City is a required field.');
        else {
            Pass('tbCity', 'divAddMailingAddress', 'divMailAddress');
            if (city.match(/^[a-zA-Z0-9'.\s-\/]{1,40}$/) == null) good = Fail('tbCity', 'divAddMailingAddress', 'divMailAddress', 'City contains invalid characters.');
            else Pass('tbCity', 'divAddMailingAddress', 'divMailAddress');
        }
    }

    if (ctl == 'ddlState' || ctl == 'all') {
        if (!DdlRequriedFieldVal('ddlState')) good = Fail('ddlState', 'divAddMailingAddress', 'divMailAddress', 'State is a required field.');
        else Pass('ddlState', 'divAddMailingAddress', 'divMailAddress');
        if (ctl == 'ddlState')
            ctl = 'ddlCountry';
    }

    if (ctl == 'ddlCountry' || ctl == 'all') {
        if (!DdlRequriedFieldVal('ddlCountry')) good = Fail('ddlCountry', 'divAddMailingAddress', 'divMailAddress', 'Country is a required field.');
        else Pass('ddlCountry', 'divAddMailingAddress', 'divMailAddress');
    }

    if (ctl == 'tbZipCode' || ctl == 'all') {
        if (zip == '') good = Fail('tbZipCode', 'divAddMailingAddress', 'divMailAddress', 'Zip code is a required field.');
        else {
            Pass('tbZipCode', 'divAddMailingAddress', 'divMailAddress');
            if (zip.match(/^\d{5}(\-\d{4})?$/) == null) good = Fail('tbZipCode', 'divAddMailingAddress', 'divMailAddress', 'Zip code contains invalid characters.');
            else Pass('tbZipCode', 'divAddMailingAddress', 'divMailAddress');
        }
    }

    if (ctl == 'ddlAddressType' || ctl == 'all') {
        if (!DdlRequriedFieldVal('ddlAddressType')) good = Fail('ddlAddressType', 'divAddMailingAddress', 'divMailAddress', 'Address type is a required field.');
        else Pass('ddlAddressType', 'divAddMailingAddress', 'divMailAddress');
    }

    return good;
}

function ValidatePhone(ctl) {
    var good = true;
    var phone = $('[id$=tbPhoneNumber]').val();
    var ext = $('[id$=tbExtension]').val();

    if (ctl == 'tbPhoneNumber' || ctl == 'all') {
        if (phone == '') good = Fail('tbPhoneNumber', 'divAddPhoneNumber', 'divPhoneNum', 'Phone number is a required field.');
        else {
            Pass('tbPhoneNumber', 'divAddPhoneNumber', 'divPhoneNum');
            if (phone.match(/^[0-9-\-]{7,15}$/) == null)
                good = Fail('tbPhoneNumber', 'divAddPhoneNumber', 'divPhoneNum', 'Phone number contains invalid characters.');
            else
                Pass('tbPhoneNumber', 'divAddPhoneNumber', 'divPhoneNum');
        }
    }

    if (ctl == 'tbExtension' || ctl == 'all') {
        if (ext != '' && ext.match(/^\d{1,7}$/) == null)
            good = Fail('tbPhoneNumber', 'divAddPhoneNumber', 'divPhoneNum', 'Phone number contains invalid characters.');
        else
            Pass('tbPhoneNumber', 'divAddPhoneNumber', 'divPhoneNum');
    }

    if (ctl == 'ddlPhoneType' || ctl == 'all') {
        if (!DdlRequriedFieldVal('ddlPhoneType')) good = Fail('ddlPhoneType', 'divAddPhoneNumber', 'divPhoneNum', 'Phone number type is a required field.');
        else Pass('ddlPhoneType', 'divAddPhoneNumber', 'divPhoneNum');
    }

    return good;
}

function ValidateEmail(ctl) {
    var good = true;
    var email = $('[id$=tbEMailAddress]').val();

    if (ctl == 'tbEMailAddress' || ctl == 'all') {
        if (email == '') good = Fail('tbEMailAddress', 'divAddEmailAddress', 'divEmailAddress', 'Email address is a required field.');
        else {
            Pass('tbEMailAddress', 'divAddEmailAddress', 'divEmailAddress');
            if (email.match(/^([a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]){1,70}$/) == null)
                good = Fail('tbEMailAddress', 'divAddEmailAddress', 'divEmailAddress', 'Email address contains invalid characters.');
            else
                Pass('tbEMailAddress', 'divAddEmailAddress', 'divEmailAddress');
        }
    }

    if (ctl == 'ddlEMailType' || ctl == 'all') {
        if (!DdlRequriedFieldVal('ddlEMailType')) good = Fail('ddlEMailType', 'divAddEmailAddress', 'divEmailAddress', 'Email type is a required field.');
        else Pass('ddlEMailType', 'divAddEmailAddress', 'divEmailAddress');
    }

    return good;
}