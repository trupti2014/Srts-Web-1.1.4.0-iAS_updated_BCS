/// <reference path="../Global/jquery-1.11.1.min.js" />
/// <reference path="../Global/PassFailConfirm.js" />
/// <reference path="../Global/GlobalVal.js" />

function DoComboPdVal(ctlId) {
    // Do combo val
    var max = 82;
    var min = 52;

    var val = $('[id$=' + ctlId + ']').val();

    if (!IsMultifocalRx()) {
        if (!DoDvoNvoPdVal())
            return Fail(ctlId, 'divPrescription', 'errorMessage', 'A PD is required for near and distance vision prescriptions.');
    }
    else {
        if (val == '') return Fail(ctlId, 'divPrescription', 'errorMessage', 'PD is required for near and distance, valid values are between 52 and 82.');
        if (!isFinite(val)) return Fail(ctlId, 'divPrescription', 'errorMessage', 'Value entered is not a valid number, valid values are between 52 and 82.');
        if ((val >= min && val <= max) == false)
            return Fail(ctlId, 'divPrescription', 'errorMessage', 'Value entered is not a valid number, valid values are between 52 and 82.');
    }
    return Pass(ctlId, 'divPrescription', 'errorMessage');
}

function DoMonoPdVal(ctlId) {
    // Do mono val
    var max = 41;
    var min = 26;

    var val = $('[id$=' + ctlId + ']').val();

    if (!IsMultifocalRx()) {
        if (!DoDvoNvoPdVal())
            return Fail(ctlId, 'divPrescription', 'errorMessage', 'A PD is required for near and distance vision prescriptions.');
    }
    else {
        if (val == '') return Fail(ctlId, 'divPrescription', 'errorMessage', 'PD is required for near and distance, valid values are between 26 and 41.');;
        if (!isFinite(val)) return Fail(ctlId, 'divPrescription', 'errorMessage', 'Value entered is not a valid number, valid values are between 26 and 41.');;
        if ((val >= min && val <= max) == false)
            return Fail(ctlId, 'divPrescription', 'errorMessage', 'Value entered is not a valid number, valid values are between 26 and 41.');
    }
    return Pass(ctlId, 'divPrescription', 'errorMessage');
}

function DoDvoNvoPdVal() {
    if (!$('#cbMonoOrComboPd').is(':checked')) {
        if (!($('#tbOdPdDistCombo').val() == '0.00' && $('#tbOdPdNearCombo').val() == '0.00')) return true;
    }
    else
        if (!($('#tbOdPdDistMono').val() == '0.00' && $('#tbOdPdNearMono').val() == '0.00' && $('#tbOsPdDistMono').val() == '0.00' && $('#tbOsPdNearMono').val() == '0.00')) return true;
    return false;
}

function IsMultifocalRx() {
    return !($('#tbOdAdd').val() == '0.00' && $('#tbOsAdd').val() == '0.00');
}

function DoCylSphVal(ctlId) {
    // Do cylinder and sphere val
    var max = 25.00;
    var min = -25.00;

    var val = $('[id$=' + ctlId + ']').val();

    if (val == "") return true;
    if (!isFinite(val)) return false;

    if (val % 1 == 0 || val % .25 == 0) return val >= min && val <= max;

    var mod = Math.abs(val % 1);
    mod = mod.toPrecision(2);

    if (mod == "0.20" || mod == "0.50" || mod == "0.70") {
        return val >= min && val <= max;
    }
    else return false;
}

function DoAxisVal(ctlId, cylId) {
    // Do axis val
    var max = 180;
    var min = 0;
    var cval = $('[id$=' + cylId + ']').val();

    var num = parseInt($('[id$=' + ctlId + ']').val(), 10);

    if (cval == "" || cval == "0.00")
        return true;

    if (isNaN(num)) {
        return false;
    }
    else {
        return num >= min && num <= max;
    }
}

function DoAddVal(ctlId) {
    // Do add val
    var max = 15.00;
    var min = 0;

    var val = $('[id$=' + ctlId + ']').val();

    if (val == "") return true;
    if (!isFinite(val)) return false;

    if (val % 1 == 0 || val % .25 == 0) return val >= min && val <= max;

    var mod = Math.abs(val % 1);
    mod = mod.toPrecision(2);

    if (mod == "0.20" || mod == "0.50" || mod == "0.70") {
        return val >= min && val <= max;
    }
    else return false;
}

function DoPrismVal(ctlId) {
    // Do add val
    var max = 15.00;
    var min = 0;

    var val = $('[id$=' + ctlId + ']').val();

    if (val == "") return true;
    if (!isFinite(val)) return false;

    if (val % 1 == 0 || val % .25 == 0) return val >= min && val <= max;

    var mod = Math.abs(val % 1);
    mod = mod.toPrecision(2);

    if (mod == "0.20" || mod == "0.50" || mod == "0.70") {
        return val >= min && val <= max;
    }
    else return false;
}

function DoPrismBaseVal(ctlId, depCtlId) {
    // Do prism base val
    var val = $('[id$=' + ctlId + ']').val().toString().toUpperCase();
    var depVal = $('[id$=' + depCtlId + ']').val();
    var bType = ctlId.substr(4, 1);

    if (val == "" && depVal == "") {
        Pass(ctlId, 'divPrescription', 'errorMessage');
        return true;
    }

    if (bType === "V") {
        if (val === "U" || val === "D") {
            Pass(ctlId, 'divPrescription', 'errorMessage');
            return true;
        }
        Fail(ctlId, 'divPrescription', 'errorMessage', 'V-Base value must be "U" or "D"');
        return false;
    }
    else if (bType === "H") {
        if (val === "I" || val === "O") {
            Pass(ctlId, 'divPrescription', 'errorMessage');
            return true;
        }
        Fail(ctlId, 'divPrescription', 'errorMessage', 'H-Base value must be "I" or "O"');
        return false;
    }
}

// Validates all prism bases
function DoAllPrismsAndBasesVal() {
    var isGood = true;
    //debugger;
    isGood = DoPrismVal('tbOdHPrism');
    isGood = DoPrismVal('tbOdVPrism');
    isGood = DoPrismVal('tbOsHPrism');
    isGood = DoPrismVal('tbOsVPrism');
    isGood = DoPrismBaseVal('tbOdHBase', 'tbOdHPrism');
    isGood = DoPrismBaseVal('tbOdVBase', 'tbOdVPrism');
    isGood = DoPrismBaseVal('tbOsHBase', 'tbOsHPrism');
    isGood = DoPrismBaseVal('tbOsVBase', 'tbOsVPrism');

    return isGood;
}

function DoPrescriptionDateVal(ctlId) {
    var val = $('[id$=' + ctlId + ']').val();
    //var date = Date.parse(val);
    if (val.trim() == '') return Fail(ctlId, 'divPrescription', 'errorMessage', 'Date of Prescription value is blank.');
    var regex = /^(0[1-9]|1[012])\/(0[1-9]|[12][0-9]|3[01])\/(19|20)\d\d$/;
    var isValid = regex.test(val);
    if (!isValid) return Fail(ctlId, 'divPrescription', 'errorMessage', 'Date of Prescription value is invalid or has invalid date format.  Correct format is MM/DD/YYYY.');

    return Pass(ctlId, 'divPrescription', 'errorMessage');
}

function DoIsDateMoreThan10Mos(ctlId) {
    var val = $('[id$=' + ctlId + ']').val();
    var date = Date.parse(val);
    if (val.trim() == '') return Fail(ctlId, 'divPrescription', 'errorMessage', 'Date of Prescription value is blank.');
    var regex = /^(0[1-9]|1[012])\/(0[1-9]|[12][0-9]|3[01])\/(19|20)\d\d$/;
    var isValid = regex.test(val);
    if (!isValid) return Fail(ctlId, 'divPrescription', 'errorMessage', 'Date of Prescription value is invalid or has invalid date format.  Correct format is MM/DD/YYYY.');

    var today = new Date(); // Today's date
    today.setHours(0, 0, 0, 0);
    var tenMonthsAgo = today.getMonth() - 10;
    today.setMonth(tenMonthsAgo);
    if (date < today) {
        confirm('This prescription date is more than 10 months old.  Are you sure you want to enter this date?');
    }
    return Pass(ctlId, 'divPrescription', 'errorMessage');
}

function DoIsNotFutureDate(ctlId) {
    var val = $('[id$=' + ctlId + ']').val();

    if (val.trim() == '') return Fail(ctlId, 'divExamData', 'divExamError', 'Exam date value cannot be blank.');

    //if (isNaN(val)) return Fail(ctlId, 'divExamData', 'divExamError', 'Exam date value is not a valid date.');

    var date = Date.parse(val);
    if (isNaN(date)) return Fail(ctlId, 'divExamData', 'divExamError', 'Exam date value is not a valid date.');

    var today = Date.now();
    if (date <= today)
        return Pass(ctlId, 'divExamData', 'divExamError');
    else
        return Fail(ctlId, 'divExamData', 'divExamError', 'Exam date cannot be a future date.');
}

function DoSegHtVal(ctlId) {
    var val = $('[id$=' + ctlId + ']').val();

    if (val == undefined || val == '') {
        EnableSaveIncompleteReprint771();
        return Fail(ctlId, 'divOrder', 'divOrderError', 'Segment height is required, valid values are 10 - 35, 3B, or 4B.');
    }
    if ((val >= 10 && val <= 35) || val.toLowerCase() == '3b' || val.toLowerCase() == '4b') {
        EnableSaveIncompleteReprint771();
        return Pass(ctlId, 'divOrder', 'divOrderError');
    }

    EnableSaveIncompleteReprint771();
    return Fail(ctlId, 'divOrder', 'divOrderError', 'Segment height is invalid, valid values are 10 - 35, 3B, or 4B.');
}

function DoMaxPairVal(ctlId) {
    var mp = $('[id$=hfMaxPair]').val();
    var op = $('[id$=' + ctlId + ']').val();

    if (mp == '') {
        EnableSaveIncompleteReprint771();
        return true;
    }
    if (!isFinite(op)) {
        EnableSaveIncompleteReprint771();
        return Fail(ctlId, 'divOrder', 'divOrderError', 'Pairs requires a numeric value');
    }

    var max = parseInt(mp, 10);
    var pair = parseInt(op, 10);

    if (max >= pair) {
        EnableSaveIncompleteReprint771();
        return Pass(ctlId, 'divOrder', 'divOrderError');
    }
    EnableSaveIncompleteReprint771();
    return Fail(ctlId, 'divOrder', 'divOrderError', 'Pairs requested has exceeded the max pairs allowable of ' + max);
}

function DoMatVal(ctlId) {
    var mat = $("#ddlMaterial").val();
    if (mat == "PLAS") return Pass(ctlId, 'divOrder', 'divOrderError');

    var val = $.trim($('[id$=' + ctlId + ']').val());
    if (val == undefined || val == "" || val.length < 5) return Fail(ctlId, 'divOrder', 'divOrderError', 'Material justification of at least 5 characters is required.');

    return Pass(ctlId, 'divOrder', 'divOrderError');
}

function DoGenericCommentVal(ctlId, commentType) {
    var val = $.trim($('[id$=' + ctlId + ']').val());
    if (val == undefined || val == "" || val.length < 5) return Fail(ctlId, 'divOrder', 'divOrderError', commentType + ' justification of at least 5 characters is required.');

    return Pass(ctlId, 'divOrder', 'divOrderError');
}

function DoCoatingVal(ctlId) {

    var checkedItem = false;
    $("[id*=ddlCoating] input:checked").each(function () {
        checkedItem = true;
    });

    if (checkedItem) {
        var val = $.trim($('[id$=' + ctlId + ']').val());
        if (val == undefined || val == "" || val.length < 3) return Fail(ctlId, 'divOrder', 'divOrderError', 'Coating justification of at least 3 characters is required.');
    }
    return Pass(ctlId, 'divOrder', 'divOrderError');
}

function DoEmailAddressVal(ctlId) {
     if ($('[id$=cbEmailPatient]').is(':checked')) {
        var val = $.trim($('[id$=' + ctlId + ']').val());
        var regex = /^([a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]){1,70}$/;
        if (regex.test(val) == 'false' || val == "") return Fail(ctlId, 'divOrder', 'divOrderError', 'Email address is required or invalid.');
    }
    else {
        return Pass(ctlId, 'divOrder', 'divOrderError');
    }
    return Pass(ctlId, 'divOrder', 'divOrderError');
}

function DoRejectRedirectVal() {
    var stat = $('#rblRejectRedirect').find('input:checked').val();
    var justMsg = 'Reject/Redirect justification of at least 5 characters is required.';

    switch (stat) {
        case 'redirect':
            if (!DdlRequriedFieldVal('ddlRedirectLab'))
                return Fail('ddlRedirectLab', 'divOrder', 'divOrderError', 'A lab is required to redirect an order.');
            break;
        case 'hfs':
            if ($('#cbCheckInHfsOrder').is(':checked')) return true;
            if (!DoStatusDateVal()) return false;

            var sReason = $('#ddlStockReason').val();

            switch (sReason.toLowerCase()) {
                case 'x':
                    return Fail('ddlStockReason', 'divOrder', 'divOrderError', 'An out of stock reason is required.');
                    break;
                case 'o':
                    justMsg = 'A comment is required when \'Other\' is the selected out of stock reason.';
                    break;
            }
    }

    if ($.trim($('[id$=tbLabJust]').val()) == '' || $.trim($('[id$=tbLabJust]').val()).length < 5)
        return Fail('tbLabJust', 'divOrder', 'divOrderError', justMsg);

    Pass('ddlRedirectLab', 'divOrder', 'divOrderError');
    Pass('tbLabJust', 'divOrder', 'divOrderError');
    $('[id$=bLabStatusChange]').focus();
    return true;
}

function DoStatusDateVal() {
    if ($('#tbHfsDate').val() != '') {
        $('#hfStockDate').val($('#tbHfsDate').val());
        return Pass('tbHfsDate', 'divOrder', 'divOrderError');
    }
    else
        return Fail('tbHfsDate', 'divOrder', 'divOrderError', 'An anticipated return to stock date is required.');

}