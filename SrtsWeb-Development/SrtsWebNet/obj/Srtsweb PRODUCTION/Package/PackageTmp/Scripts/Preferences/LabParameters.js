/// <reference path="../Global/jquery-1.11.1.min.js" />

function ParamInput(ctlId) {
    var _text = $('#' + ctlId).val();

    if (!DoInputVal(ctlId)) {
        var msg = "";

        switch (ctlId) {
            case "tbCyl":
                msg = "Cylinder value must be between 0 and -25, in increments of 0.25";
                break;
            case "tbMaxPlus":
                msg = "Max Plus value must be between 0 and +25, in increments of 0.25";
                break;
            case "tbMaxMinus":
                msg = "Max Minus value must be between 0 and -25, in increments of 0.25";
                break;
            default:
                msg = "Value";
                break;
        }

        Fail(ctlId, 'btnSaveParams', 'errorMessage', msg);
        return false;
    }
    else {
        Pass(ctlId, 'btnSaveParams', 'errorMessage');
        if (_text == "") {
            return true;
        }
    }

    var sign = _text.charAt(0);
    var num = parseFloat(_text);

    var mod = Math.abs(num % 1);
    mod = mod.toPrecision(2);
    var modAbs = Math.abs(num);

    if (mod == "0.20" || mod == "0.70") modAbs = modAbs + .05;

    AddSign(sign, modAbs, ctlId);

    return true;
}

//****Ranges*****
//Cyl should be 0 to -25
//Max Plus range should be 0.00 to +25.00
//Max minus range should be -25.00 to 0.00

function DoInputVal(ctlId) {

    var min;
    var max;

    switch (ctlId) {
        case "tbCyl":
            min = -25.00;
            max = 0.00;
            break;
        case "tbMaxPlus":
            min = 0.00;
            max = 25.00;
            break;
        case "tbMaxMinus":
            min = -25.00;
            max = 0.00;
            break;
        case "tbMaxPrism":
            min = 0.00;
            max = 15.00;
        default:
            min = -25.00;
            max = 25.00;
            break;
    }

    var val = $('#' + ctlId).val();

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

function ValLabParam(ctlId)
{
    var min;
    var max;
    var val = $('#' + ctlId).val();
    var msg = "";
    switch (ctlId) {
        case "tbMaxDecentrationPlus":
            min = 0.00;
            max = 15.00; 
            msg = "Max Decentration - Plus value must be between 0 and 15";
            break;
        case "tbMaxDecentrationMinus":
            min = -15.00;
            max = 0.00;
            msg = "Max Decentration - Minus  value must be between 0 and -15";
            break;
        case "tbMaxPrism":
            msg = "Prism value must be between 0.25 and 15, in increments of 0.25";
            break;
        default:
            msg = "Value";
            break;
    }

    if (val == "" || ctlId != "tbMaxPrism" && val >= min && val <= max ||  (ctlId == "tbMaxPrism" && DoInputVal(ctlId)))
    {
        Pass(ctlId, 'btnSaveLabParams', 'errorMessage');
        return true;
    }
    else
    {
        Fail(ctlId, 'btnSaveLabParams', 'errorMessage', msg);
        return false;
    }


}

function AddSign(sign, num, ctlId) {
    switch (sign) {
        case "+":
            $('#' + ctlId).val('+' + num.toFixed(2));
            break;
        case "-":
            $('#' + ctlId).val('-' + num.toFixed(2));
            break;
        case "0":
            $('#' + ctlId).val(num.toFixed(2));
            break;
        default:
            $('#' + ctlId).val('+' + num.toFixed(2));
            break;
    }
}

function IsGoodToSave() {

    var isGood = false;

    if ($('[id*=ddlMatType]').val() == "X") {
        alert("'Material' selection is required");
        return isGood;
    }
    if ($('[id*=ddlIsStocked]').val() == "-Select-") {
        alert("'In Stock' selection is required");
        return isGood;
    }
    if ($('#tbCyl').val() == "") {
        alert("'Cylinder' value is required");
        return isGood;
    }
    if ($('#tbMaxPlus').val() == "") {
        alert("'Max Plus' value is required");
        return isGood;
    }
    if ($('#tbMaxMinus').val() == "") {
        alert("'Max Minus' value is required");
        return isGood;
    }
    if ($('[id*=ddlCapabilityType]').val() == "-Select-") {
        alert("'Type' selection is required");
        return isGood;
    }
    isGood = true;
    return isGood;
}

///////////////////


function Pass(errorCtlId, errorCtlCntrId, msgCtlId) {
    var tmp = sessionStorage.getItem('error');
    var errs;
    sessionStorage.clear();

    if (tmp == undefined || tmp == '') return true;

    errs = JSON.parse(tmp);

    delete errs[errorCtlId];

    sessionStorage['error'] = JSON.stringify(errs);

    // Create combined message
    var msg = '';
    for (var k in errs)
        msg += errs[k] + '<br />';

    // New for combo boxes
    var comboBoxCntrId = GetAltCntrID(errorCtlId);

    if (comboBoxCntrId != '') {
        $('[id*=' + comboBoxCntrId + ']').removeClass('redBorder');
    }

    if (errorCtlId != '')
        $('[id*=' + errorCtlId + ']').removeClass('redBorder');
    $('[id*=' + msgCtlId + ']').html(msg);
    if (msg === '') {
        $('[id*=' + msgCtlId + ']').hide();
        if (errorCtlCntrId == '') return true;
        $('[id*=' + errorCtlCntrId + ']').removeAttr('disabled');
        return true;
    }
    else {
        $('[id*=' + msgCtlId + ']').show();
        if (errorCtlCntrId == '') return false;
        $('[id*=' + errorCtlCntrId + ']').attr('disabled', true);
        return false;
    }
}

function Fail(errorCtlId, errorCtlCntrId, msgCtlId, errorMsg) {
    var tmp = sessionStorage.getItem('error');
    var errs = {};
    sessionStorage.clear();
    if (tmp != undefined && tmp != '')
        errs = JSON.parse(tmp);

    if (errs.hasOwnProperty(errorCtlId)) {
        delete errs[errorCtlId];
    }
    errs[errorCtlId] = errorMsg;

    //if (errs == undefined)
    //    errs[errorCtlId] = errorMsg;
    //else if (errs.hasOwnProperty(errorCtlId)) {
    //    delete errs[errorCtlId];
    //    errs[errorCtlId] = errorMsg;
    //}

    sessionStorage['error'] = JSON.stringify(errs);

    // Create combined message
    var msg = '';
    for (var k in errs)
        msg += errs[k] + '<br />';

    try {
        $('[id*=' + errorCtlId + ']').addClass('redBorder');

        // New for combo boxes
        var comboBoxCntrId = GetAltCntrID(errorCtlId);

        if (comboBoxCntrId != '') {
            $('[id*=' + comboBoxCntrId + ']').addClass('redBorder');
        }
    } catch (err) { /*NOTHING*/ }
    $('[id*=' + msgCtlId + ']').html(msg);
    $('[id*=' + msgCtlId + ']').show();
    if (errorCtlCntrId == '') return false;
    $('[id*=' + errorCtlCntrId+']').attr('disabled', true);
    return false;
}
///////////////////////////

// This is used to capture the partial postback and determine if the success dialog should be shown.
Sys.Application.add_load(function () {
    //// The success value is set in code behind to 1.
    if ($('#hfSuccessLabPref').val() != '1') return;

    // The display message is set in code behind.
    var msg = $('#hfMsgLabPref').val();

    // Call function to show message.
    displaySrtsMessage('Success!', msg, 'success');

    // Reset the success hidden field to 0 so not to re-show the success dialog on the next partial postback.
    $('#hfSuccessLabPref').val('0');
});
