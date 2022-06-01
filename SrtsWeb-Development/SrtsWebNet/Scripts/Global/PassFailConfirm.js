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
        $('[id*=' + errorCtlCntrId + ']' + ' :input[type=submit]').removeAttr('disabled');
        return true;
    }
    else {
        $('[id*=' + msgCtlId + ']').show();
        if (errorCtlCntrId == '') return false;
        $('[id*=' + errorCtlCntrId + ']' + ' :input[type=submit]').attr('disabled', true);
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
    $('[id*=' + errorCtlCntrId + ']' + ' :input[type=submit]').attr('disabled', true);
    return false;
}

// Used for combo boxes to get alternate container for red border
function GetAltCntrID(errorCtlId) {
    var cntrId = '';

    switch (errorCtlId) {
        case "ddlColor":
            cntrId = 'tdColor';
            break;
        case "ddlEye":
            cntrId = 'tdEye';
            break;
        case "ddlBridge":
            cntrId = 'tdBridge';
            break;
        case "ddlTemple":
            cntrId = 'tdTemple';
            break;
        case "ddlLens":
            cntrId = 'tdLens';
            break;
        case "ddlTint":
            cntrId = 'tdTint';
            break;
        case "ddlMaterial":
            cntrId = 'tdMaterial';
            break;
        case "ddlProdLab":
            cntrId = 'tdProdLab';
            break;
        default:
            cntrId = '';
            break;
    }
    return cntrId
}

function InErrorState() {
    var tmp = sessionStorage.getItem('error');

    if (tmp == null)
        return false;
    if (tmp != undefined || tmp != '')
        return tmp != '{}';

    return false;
}

function Confirm(confirmMsg, msgCtlId, isFail, persistMsg) {
    //debugger;
    if (isFail) {
        $('[id*=' + msgCtlId + ']').removeClass('successMessage');
        $('[id*=' + msgCtlId + ']').addClass('failMessage');
    }
    else {
        $('[id*=' + msgCtlId + ']').removeClass('failMessage');
        $('[id*=' + msgCtlId + ']').addClass('successMessage');
    }

    $('[id*=' + msgCtlId + ']').text(confirmMsg);
    $('[id*=' + msgCtlId + ']').show();
    if (persistMsg) return;
    $('[id*=' + msgCtlId + ']').delay(5000).fadeOut();
    return;
}

function ClearMsgs(errorCtlCntrId, msgCtlId) {
    sessionStorage.clear();
    $('[id*=' + msgCtlId + ']').html('');
    $('[id*=' + msgCtlId + ']').hide();
    $('[id*=' + errorCtlCntrId + ']' + ' :input[type=submit]').removeAttr('disabled');
    $('.redBorder').removeClass();
}