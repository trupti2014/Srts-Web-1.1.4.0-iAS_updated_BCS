/* DO DIALOG OPERATIONS */
function DoAddressDialog(isAdd) {
    var title = '';
    if (isAdd == true)
        title = 'Add Address';
    else
        title = 'Edit Address';

    var do1 = {
        autoOpen: false,
        modal: true,
        width: 500,
        height: 585,
        title: title,
        dialogClass: 'generic',
        open: function () {
            $('[id$=tbAddress1]').focus();
        },
        close: function () {
            $(this).dialog('destroy');
            $(this).hide
        }
    };
    var d1 = $('[id$=divAddMailingAddress]').dialog(do1);
    d1.parent().appendTo($('form:first'));
    d1.dialog('open');
}
function DoIdNumberDialog(isAdd) {
    var title = '';
    if (isAdd == true)
        title = 'Add ID Number';
    else
        title = 'Edit ID Number';

    var do2 = {
        autoOpen: false,
        modal: true,
        width: 500,
        height: 305,
        title: title,
        dialogClass: 'generic',
        open: function () {
            $('[id$=tbIDNumber]').focus();
        },
        close: function () {
            $(this).dialog('destroy');
            $(this).hide();
        }
    };
    var d2 = $('[id$=divAddPatientIdNumber]').dialog(do2);
    d2.parent().appendTo($('form:first'));
    d2.dialog('open');
}
function DoPhoneDialog(isAdd) {
    var title = '';
    if (isAdd == true)
        title = 'Add Phone Number';
    else
        title = 'Edit Phone Number';

    var do3 = {
        autoOpen: false,
        modal: true,
        width: 500,
        height: 325,
        title: title,
        dialogClass: 'generic',
        open: function () {
            $('[id$=tbPhoneNumber]').focus();
        },
        close: function () {
            $(this).dialog('destroy');
            $(this).hide();

            //ClearMsgs('', '');
        }
    };
    var d3 = $('[id$=divAddPhoneNumber]').dialog(do3);
    d3.parent().appendTo($('form:first'));
    d3.dialog('open');
}
function DoEmailDialog(isAdd) {
    var title = '';
    if (isAdd == true)
        title = 'Add Email Address';
    else
        title = 'Edit Email Address';

    var do4 = {
        autoOpen: false,
        modal: true,
        width: 500,
        height: 295,
        title: title,
        dialogClass: 'generic',
        open: function () {
            $('[id$=tbEMailAddress]').focus();
        },
        close: function () {
            $(this).dialog('destroy');
            $(this).hide();

            //ClearMsgs('', '');
        }
    };
    var d4 = $('[id$=divAddEmailAddress]').dialog(do4);
    d4.parent().appendTo($('form:first'));
    d4.dialog('open');
}

function ClearUIID() {
    $find("idValidator").hide();
    $find("typeValidator").hide();
    $get("tbIDNumber").value = "";
    $get("ddlIDNumberType").value = "";
}

function IsValidID() {
    var textbox = $get("tbIDNumber");
    if (textbox.value == "") {
        return false;
    }
    return true;
}

function IsValidIDType() {
    var textbox = $get("ddlIDNumberType");
    if (textbox.value == "") {
        return false;
    }
    return true;
}

function ClearUIAddr() {
    $find("addr1Validator").hide();
    $find("addr1ImpValidator").hide();
    $find("addr2ImpValidator").hide();
    $find("cityValidator").hide();
    $find("zipValidator").hide();
    $find("stateValidator").hide();
    $find("countryValidator").hide();
    $find("addrTypeValidator").hide();
    $get("tbAddress1").value = "";
    $get("tbAddress2").value = "";
    $get("tbCity").value = "";
    $get("tbZipCode").value = "";
    $get("ddlState").value = "";
    $get("ddlCountry").value = "";
    $get("ddlAddressType").value = "";
}

function IsValidAddr1() {
    var textbox = $get("tbAddress1");
    if (textbox.value == "") {
        return false;
    }
    return true;
}

function IsValidAddr1Imp() {
    var textbox = $get("tbAddress1");
    if (textbox.value == "") {
        return false;
    }
    return true;
}

function IsValidAddr2Imp() {
    var textbox = $get("tbAddress2");
    if (textbox.value == "") {
        return false;
    }
    return true;
}

function IsValidCity() {
    var textbox = $get("tbCity");
    if (textbox.value == "") {
        return false;
    }
    return true;
}

function IsValidState() {
    var textbox = $get("ddlState");
    if (textbox.value == "") {
        return false;
    }
    return true;
}

function IsValidCountry() {
    var textbox = $get("ddlCountry");
    if (textbox.value == "") {
        return false;
    }
    return true;
}

function IsValidAddrType() {
    var textbox = $get("ddlAddressType");
    if (textbox.value == "") {
        return false;
    }
    return true;
}

function ClearUIPhone() {
    $find("areaCodeValidator").hide();
    $find("phoneValidator").hide();
    $find("phoneTypeValidator").hide();
    $get("tbAreaCode").value = "";
    $get("tbPhoneNumber").value = "";
    $get("ddlPhoneType").value = "";
}

function IsValidAreaCode() {
    var textbox = $get("tbAreaCode");
    if (textbox.value == "") {
        return false;
    }
    return true;
}

function IsValidPhone() {
    var textbox = $get("tbPhoneNumber");
    if (textbox.value == "") {
        return false;
    }
    return true;
}

function IsValidPhoneType() {
    var textbox = $get("ddlPhoneType");
    if (textbox.value == "") {
        return false;
    }
    return true;
}

function ClearUIEmail() {
    $find("emailValidator").hide();
    $find("emailTypeValidator").hide();
    $get("tbEmailAddress").value = "";
    $get("ddlEmailType").value = "";
}

function IsValidEmail() {
    var textbox = $get("tbEmailAddress");
    if (textbox.value == "") {
        return false;
    }
    return true;
}

function IsValidEmailType() {
    var textbox = $get("ddlEmailType");
    if (textbox.value == "") {
        return false;
    }
    return true;
}

function ClearUIIndividualType()
{
    $find("indTypeValidator").hide();
    $get("ddlIndType").value = "";
}

function IsValidEmailType() {
    var textbox = $get("ddlIndType");
    if (textbox.value == "") {
        return false;
    }
    return true;
}

function ClearPopup() {
    if (IsValidID() && IsValidIDType()) {
        $find('modalwithinput').hide();
        ClearUIID();
    }
    if (IsValidAddr1() && IsValidCity() && IsValidZip && IsValidState && IsValidCountry && IsValidAddrType && IsValidAddr2Imp() && IsValidAddr1Imp) {
        $find('modalwithinput1').hide();
        ClearUIAddr();
    }
    if (IsValidAreaCode() && IsValidPhone() && IsValidPhoneType()) {
        $find('modalwithinput2').hide();
        ClearUIPhone();
    }
    if (IsValidEmail() && IsValidEmailType()) {
        $find('modalwithinput3').hide();
        ClearUIEmail();
    }
    if (IsValidEmail() && IsValidEmailType()) {
        $find('modalwithinput4').hide();
        ClearUIEmail();
    }
}