/// <reference path="../Global/jquery-1.11.1.min.js" />
/// <reference path="PersonDetailVal.js" />
/// <reference path="../Global/jquery-ui.min.js" />

var logname = '';
var switchlevel = '';
var thisLog = '';
var currentLogName = '';


$(document).ready(LoadEvents);

function LoadEvents() {
    $('#btnSaveNewLevel').hide();
    logPanels.hide();

    if (hdfLogName.value != '') {
        logname = hdfLogName.value;

        var panel = GetLogPanel(logname);
        $("#" + panel).show();
        document.getElementById('logLevelTitle').innerHTML = " - " + logname;

    }

    $('.log > .logName > a').click(function () {
        thisLog = $(this);
        logPanels.hide();
        GetSelectedLog();
    });
}

function GetSelectedLog() {
    logPanels.hide();
    $('.log > .logName > a').removeClass('active');
    document.getElementById('logLevelTitle').innerHTML = "";

    thisLog.parent().next().show();
    thisLog.addClass('active');

    logname = thisLog.text();
    document.getElementById('logLevelTitle').innerHTML = " - " + logname;
    return false;
}
function ShowSaveSwitchLevel() {
    $('#btnSaveNewLevel').show();
}
function SaveTraceSwitchLevel() {
    switchlevel = $('#rblLoginSwitchLevel').find('input:checked').val();
    UpdateLogLevel(logname, switchlevel);
    $('#rblLoginSwitchLevel').find('input').removeAttr('checked');
    $('#btnSaveNewLevel').hide();

}
function UpdateLogLevel(logname, switchlevel) {
    hdfLogName.value = logname;
    PageMethods.SaveLogLevel(logname, switchlevel, LoadEvents);
}


function GetLogPanel(logname) {
    var panel;
    switch (logname) {
        case "Admin Events":
            panel = "pnlAdmin";
            break;
        case "Login Events":
            panel = "pnlLogin";
            break;
        case "Clinic Management Events":
            panel = "pnlClinicManagement";
            break;
        case "Clinic Orders Events":
            panel = "pnlClinicOrders";
            break;
        case "Lab Management Events":
            panel = "pnlLabManagement";
            break;
        case "Lab Orders Events":
            panel = "pnlLabOrders";
            break;
        case "Exam Events":
            panel = "pnlExam";
            break;
        case "Prescription Events":
            panel = "pnlPrescription";
            break;
        case "Person Events":
            panel = "pnlPerson";
            break;
        default:
            panel = "pnlAdmin";
    }
    return panel;
}