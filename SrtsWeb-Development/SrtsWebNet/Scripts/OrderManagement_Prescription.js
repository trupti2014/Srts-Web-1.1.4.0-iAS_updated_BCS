/// <reference path="jquery-1.11.1.min.js"/>
/// <reference path="jquery-ui.min.js"/>

// Used for the 'Cylinder' and 'Sphere' inputs
function CylSphInput(_text, id) {
    var sign = _text.value.charAt(0);
    var num = parseFloat(_text.value);
    var max = 25.00;
    var min = -25.00;

    if (_text.value == "") {
        $('#' + id).removeClass('error');
        $("#errorMessage").hide();
        return;
    }

    if (isNaN(num) || num > max || num < min) {
        $('#' + id).addClass('error');
        $("#errorMessage").text("Value must be between -25 and +25, in increments of 0.25");
        $("#errorMessage").show();
        return;
    }
    $('#' + id).removeClass('error');
    $("#errorMessage").hide();

    if (num % 1 == 0 || num % .25 == 0) {
        AddSign(sign, num, id);
        return;
    }

    var mod = Math.abs(num % 1);
    var mod = mod.toPrecision(2);

    switch (mod) {
        case "0.20":
        case "0.70":
            num = num + .05;
            break;
        case "0.50":
            break;
        default:
            $('#' + id).addClass('error');
            $("#errorMessage").text("Value must be between -25 and +25, in increments of 0.25");
            $("#errorMessage").show();
            return;
    }
    AddSign(sign, num, id);
}

// Used for the 'Axis' inputs
function AxisInput(_text, id) {
    debugger;
    var num = parseInt(_text.value);
    var paddedNum = "";
    var max = 180;
    var min = 0;

    if (_text.value == "") {
        $('#' + id).removeClass('error');
        $("#errorMessage").hide();
        return;
    }

    if (isNaN(num) || num > max || num < min) {
        $('#' + id).addClass('error');
        $("#errorMessage").text("Value must be between 1 and 180");
        $("#errorMessage").show();
        return;
    }

    $('#' + id).removeClass('error');
    $("#errorMessage").hide();
    paddedNum = String("000" + num).slice(-3);
    $('#' + id).val(paddedNum);
}

// Used for the 'Add' and 'Prism' imputs
function AddPrismInput(_text, id) {
    var num = parseFloat(_text.value);
    var max = 15.00;
    var min = 0;

    if (_text.value == "") {
        $('#' + id).removeClass('error');
        $("#errorMessage").hide();
        return;
    }

    if (isNaN(num) || num > max || num < min) {
        $('#' + id).addClass('error');
        $("#errorMessage").text("Value must be between 0.25 and 15, in increments of 0.25");
        $("#errorMessage").show();
        return;
    }
    $('#' + id).removeClass('error');
    $("#errorMessage").hide();

    if (num % 1 == 0 || num % .25 == 0) {
        $('#' + id).val('+' + num.toFixed(2));
        return;
    }

    var mod = Math.abs(num % 1);
    var mod = mod.toPrecision(2);

    switch (mod) {
        case "0.20":
        case "0.70":
            num = num + .05;
            break;
        case "0.50":
            break;
        default:
            $('#' + id).addClass('error');
            $("#errorMessage").text("Value must be between 0.25 and 15, in increments of 0.25");
            $("#errorMessage").show();
            return;
    }
    $('#' + id).val('+' + num.toFixed(2));
    return;
}

function AddSign(_sign, _num, _id) {
    switch (_sign) {
        case "+":
            $('#' + _id).val('+' + _num.toFixed(2));
            return;
        case "-":
            $('#' + _id).val(_num.toFixed(2));
            return;
        default:
            $('#' + _id).val((_num * -1).toFixed(2));
            return;
    }
}

//*********************************************************************
function SegHtInput(_Id) {
    // 10-35, 3B, 4B
    $('#' + _Id).removeClass('error');
    $("#errorMessage").hide();

    var v = $('#' + _Id).val();

    if (v == '') return true;

    if (isNaN(v)) {
        if (v.toUpperCase() == '3B' || v.toUpperCase() == '4B') return true;
    }
    else if (v >= 10 && v <= 35) return true;

    $('#' + _Id).addClass('error');
    $("#errorMessage").text("Seg Height valid values are 10 - 35 or 3B or 4B");
    $("#errorMessage").show();
    return false;
}

function CopySegHt(source, dest) {
    $('#' + dest).val($('#' + source).val());
}