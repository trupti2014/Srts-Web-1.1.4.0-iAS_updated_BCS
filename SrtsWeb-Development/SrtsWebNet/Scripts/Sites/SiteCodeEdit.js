/// <reference path="../Global/jquery-1.11.1.min.js" />

//////////////////////////////////////////////
/// 7/7/2017 - This code has been moved to 
/// scripts/preferences/LabParameters.js
/////////////////////////////////////////////

    if (!DoInputVal(ctlId)) {
        var msg = "";

        switch (ctlId) {
            case "tbCyl":
                msg = "Cylincder value must be between 0 and -25, in increments of 0.25";
                break;
            case "tbMaxPlus":
                msg = "Plus value must be between 0 and +25, in increments of 0.25";
                break;
            case "tbMaxMinus":
                msg = "Minus value must be between 0 and -25, in increments of 0.25";
                break;
            default:
                msg = "Value";
                break;
        }

        Fail(ctlId, 'divLabParams', 'errorMessage', msg);
        return false;
    }
    else {
        Pass(ctlId, 'divLabParams', 'errorMessage');
        if (_text == "") {
            return true;
        }
    }

//function ParamInput(ctlId) {
//    var _text = $('#' + ctlId).val();

//    if (!DoInputVal(ctlId)) {
//        var msg = "";

//        switch (ctlId) {
//            case "tbCyl":
//                msg = "Cylincder value must be between 0 and -25, in increments of 0.25";
//                break;
//            case "tbMaxPlus":
//                msg = "Plus value must be between 0 and +25, in increments of 0.25";
//                break;
//            case "tbMaxMinus":
//                msg = "Minus value must be between 0 and -25, in increments of 0.25";
//                break;
//            default:
//                msg = "Value";
//                break;
//        }

//        Fail(ctlId, 'divLabParams', 'errorMessage', msg);
//        return false;
//    }
//    else {
//        Pass(ctlId, 'divLabParams', 'errorMessage');
//        if (_text == "") {
//            return true;
//        }
//    }

//    var sign = _text.charAt(0);
//    var num = parseFloat(_text);

//    var mod = Math.abs(num % 1);
//    mod = mod.toPrecision(2);
//    var modAbs = Math.abs(num);

//    if (mod == "0.20" || mod == "0.70") modAbs = modAbs + .05;

//    AddSign(sign, modAbs, ctlId);

//    return true;
//}

////****Ranges*****
////Cyl should be 0 to -25
////Max Plus range should be 0.00 to +25.00
////Max minus range should be -25.00 to 0.00

//function DoInputVal(ctlId) {

//    var min;
//    var max;

//    switch (ctlId) {
//        case "tbCyl":
//            min = -25.00;
//            max = 0.00;
//            break;
//        case "tbMaxPlus":
//            min = 0.00;
//            max = 25.00;
//            break;
//        case "tbMaxMinus":
//            min = -25.00;
//            max = 0.00;
//            break;
//        default:
//            min = -25.00;
//            max = 25.00;
//            break;
//    }

//    var val = $('#' + ctlId).val();

//    if (val == "") return true;
//    if (!isFinite(val)) return false;

//    if (val % 1 == 0 || val % .25 == 0) return val >= min && val <= max;

//    var mod = Math.abs(val % 1);
//    mod = mod.toPrecision(2);

//    if (mod == "0.20" || mod == "0.50" || mod == "0.70") {
//        return val >= min && val <= max;
//    }
//    else return false;
//}

//function AddSign(sign, num, ctlId) {
//    switch (sign) {
//        case "+":
//            $('#' + ctlId).val('+' + num.toFixed(2));
//            break;
//        case "-":
//            $('#' + ctlId).val('-' + num.toFixed(2));
//            break;
//        case "0":
//            $('#' + ctlId).val(num.toFixed(2));
//            break;
//        default:
//            $('#' + ctlId).val('+' + num.toFixed(2));
//            break;
//    }
//}

//function IsGoodToSave() {

//    var isGood = false;

//    if ($('#ddlMatType').prop('selectedIndex') == 0) {
//        alert("'Material' selection is required");
//        return isGood;
//    }
//    if ($('#ddlIsStocked').prop('selectedIndex') == 0) {
//        alert("'In Stock' selection is required");
//        return isGood;
//    }
//    if ($('#tbCyl').val() == "") {
//        alert("'Cylinder' value is required");
//        return isGood;
//    }
//    if ($('#tbMaxPlus').val() == "") {
//        alert("'Max Plus' value is required");
//        return isGood;
//    }
//    if ($('#tbMaxMinus').val() == "") {
//        alert("'Max Minus' value is required");
//        return isGood;
//    }
//    isGood = true;
//    return isGood;
//}
