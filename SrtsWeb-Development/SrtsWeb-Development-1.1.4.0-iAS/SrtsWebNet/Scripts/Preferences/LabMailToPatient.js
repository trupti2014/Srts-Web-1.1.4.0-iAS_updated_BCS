/// <reference path="../Global/jquery-1.11.1.min.js" />
/// <reference path="../Global/PassFailConfirm.js" />


$(function () {
    SetMailToPatientStatus();

    $('#rblMailToPatient').change(function () {
        var MailToPatient = $('#rblMailToPatient').find('input:checked').val();
        var status = $('#hdfLabMTPStatus').val();
        if (MailToPatient == 'true' & status =='true')
        {
            TurnOnMailToPatient();
            ShowMailToPatient();
            return
        }
        if (MailToPatient == 'false' & status == 'false') {
            TurnOffMailToPatient();
            HideMailToPatient();
            return
        }

        if (MailToPatient == "false") {
            TurnOffMailToPatient();
        }
        else {
            TurnOnMailToPatient();
        }
    });

    $('#rblNoMailToPaientReason').change(function () {
        NoMailReason = $('#rblNoMailToPaientReason').find('input:checked').val();
        $('#spnNoCapabilityReason').hide();
        
        if (NoMailReason == "Other") {
            $('#spnComments').show();
            $('#spnRestartDate').hide();
            $('#spnStopDate').show();
            $('#spnRequired').show();
            $('#txtOtherComments').val("");
            $('#txtOtherComments').focus();
            $('#divRestartDate').hide();
        }
        else {
            $('#txtOtherComments').val("");
            $('#spnComments').hide();
            $('#spnStopDate').show();
            $('#divRestartDate').show();
            $('#spnRestartDate').show();
            $('#spnRequired').show();
            $('#txtRestartDate').val("");
        }
        $('#divCapabilityDateRange').show();     
        $('#txtStartDate').val("");
        $('#divOtherComment').show();
        SetDefaultDates("off");
    });

});


function pageLoad() {
    SetMailToPatientStatus();

    var disabledClinicsCount = $("[id*=chkDisabledList] input").length;
    var allClinicsCount = $("[id*=chkAllClinics] input").lenghth;
    if ($("[id*=chkDisabledList] input").length == 0) {
        $('#divClinicActionRequired').hide();
    }
    else {
        $('#divClinicActionRequired').show();
    }



    $("[id*=chkAllClinics]").bind("change", function (e) {
        if ($(this).is(":checked")) {
            $("[id*=chkClinics] input").prop("checked", true);
        } else {
            $("[id*=chkClinics] input").prop("checked", false);
        }
        DoUpdateClinicsToDisableList(e);
    });

    $("[id*=chkAllDisabledClinics]").bind("change", function (e) {
        if ($(this).is(":checked")) {
            $("[id*=chkDisabledList] input").prop("checked", true);
        } else {
            $("[id*=chkDisabledList] input").prop("checked", false);
        }
        DoUpdateDisabledClinicsList(e);
    });

    $("[id*=chkClinics]").bind("change", function (e) {
        DoUpdateClinicsToDisableList(e);
    });

    $("[id*=chkDisabledList]").bind("change", function (e) {
        DoUpdateDisabledClinicsList(e);
    });

    $('#rblMailToPatient').change(function () {
        MailToPatient = $('#rblMailToPatient').find('input:checked').val();
        if (MailToPatient == "false") {
            TurnOffMailToPatient();
        }
        else {
            TurnOnMailToPatient();
        }
    });
}

function SetMailToPatientStatus() {
    var MailToPatient = $('#hdfLabMTPStatus').val();
    if (MailToPatient == "") {
        MailToPatient = $('#rblMailToPatient').find('input:checked').val();
    }
    if (MailToPatient == "false") {
        HideMailToPatient();
    }
    else {
        ShowMailToPatient();
    }
    DisplayLabMailToPatient();
}

function DoUpdateClinicsToDisableList(e) {
    var lsttoDisable = "";
    $('#ulClinicsDisabled').empty();
    $("#chkClinics input:checked").each(function () {
        if ($(this).is(":checked")) {
            var text = $('label[for="' + $(this).attr('id') + '"]')[0].innerText;
            var $li = $('<li>', {
                id: $(this).attr('id'),
                text: text
            });
            $('#ulClinicsDisabled').append($li);
            lsttoDisable += text.substring(0, 6) + ",";
        }
    });
    var count = $('#ulClinicsDisabled li').length;
    if (count == 0) {
        $('#lblClinicsToDisable').text("");
        $('#divClinicStopDate').hide();
    }
    else {
        $('#divClinicStopDate').show();
        $('#lblClinicsToDisable').attr("style", "color:red;font-size:11px;font-weight:normal");
        $('#lblClinicsToDisable').text("The following " + count + " clinic/s will be notified of a change in their ability to use the 'Lab ship to patient' option.");
        $('#hdfClinicsToDisable').attr('value', lsttoDisable);
        SetDefaultDates("disableclinic");
    }
    e.stopPropagation();
}

function DoUpdateDisabledClinicsList(e){
    var lsttoEnable = "";
    $('#ulClinicsToEnable').empty();
    $("#chkDisabledList input:checked").each(function () {
        if ($(this).is(":checked")) {
            $(this).css('color', 'red');
            var text = $('label[for="' + $(this).attr('id') + '"]')[0].innerText;
            var $li = $('<li>', {
                id: $(this).attr('id'),
                text: text
            });
            $('#ulClinicsToEnable').append($li);
            lsttoEnable += text.substring(0, 6) + ",";
        }
    });
    var count = $('#ulClinicsToEnable li').length;
    if (count == 0) {
        $('#lblClinicsToEnable').text("");
        $('#divClinicRestartDate').hide();
    }
    else
    {
        var resumeDate = new Date();
        $('#txxtClinicRestartDate').text = resumeDate;
        $('#divClinicRestartDate').show();
        $('#lblClinicsToEnable').attr("style", "color:red;font-size:11px;font-weight:normal");
        $('#lblClinicsToEnable').text("*The following " + count + " clinic/s will be notified of their ability use the mail-to-patient option effective immediately or on the resume date if entered.");
        $('#hdfClinicsToEnable').attr('value', lsttoEnable);
    }
    e.stopPropagation();
}

function TurnOffMailToPatient() {
    $('#divNoCapabilityReason').show();
    $('#divClinicEnableDisable').hide();
    $('#divClinicDisableNotified').hide();
    $('#divClinicsToEnable').hide();
    $('#divClinicsToDisable').hide();
    $('#rblNoMailToPaientReason').find('input[value="NoCapacity"]').prop('checked', true);
    $('#spnRequired').show();
    $('#spnRestartDate').show();
    $('#spnStopDate').show();
  //  $("[id*=rblNoMailToPaientReason] input").prop("checked", false);
    $('#divEndDate').show();
    $('#divStartDate').hide();
    $('#txtStartDate').val("");
    $('#txtOtherComments').val("");
    $('#divCapabilityDateRange').show();
    $('#divOtherComment').show();
    $('#txtOtherComments').val("");
    $('#spnNoCapabilityReason').show();
    SetDefaultDates("off");
}

function TurnOnMailToPatient() {
    $('#divNoCapabilityReason').hide();
    $('#divClinicEnableDisable').hide();
    $('#divClinicDisableNotified').hide();
    $('#divClinicsToEnable').hide();
    $('#divClinicsToDisable').hide();
    $("[id*=rblNoMailToPaientReason] input").prop("checked", false);
    $('#divEndDate').hide();
    $('#divStartDate').show();
    $('#txtStartDate').val("");
    $('#divCapabilityDateRange').show();
    $('#divOtherComment').hide();
    $('#txtOtherComments').val("");
    $('#spnNoCapabilityReason').hide
    $('#spnRequired').hide();
    $('#spnRestartDate').hide();
    $('#spnStopDate').hide();
    SetDefaultDates("on");
}

function DisplayLabMailToPatient() {
    $('#lnkLabMailToPatient').trigger("click");
    return;
}

function ShowMailToPatient() {
    var status = $('#hdfLabMTPStatus').val();
    $('#lnkLabMailToPatient').trigger("click");
    $('#rblMailToPatient').find('input[value=' + status + ']').prop('checked', true);
    $('#divClinicEnableDisable').show();
    $('#divClinicDisableNotified').show();
    $('#divClinicsToEnable').show();
    $('#divClinicsToDisable').show();
    $('#divCapabilityDateRange').hide();
    $('#divNoCapabilityReason').hide();
    $('#spnNoCapabilityReason').hide();
    $("[id*=chkClinics] input").prop("checked", false);
    $("[id*=chkDisabledList] input").prop("checked", false);
    $('#ulClinicsToEnable').empty();
    $('#ulClinicsDisabled').empty();
    $('#lblClinicsToDisable').empty();
    $('#lblClinicsToEnable').empty();


    var count = $("[id*=chkClinics_Disabled] input").length;
    if (count == 0) {
        $('#lblClinicDisableNotified').attr("style", "color:#000");
        $('#lblClinicDisableNotified').text("*There are currently no disabled clinics for this lab.");
        $('#spnchkAllClinics_Disabled').hide();
    }
    else {
        $('#lblClinicDisableNotified').attr("style", "color:#000");
        $('#lblClinicDisableNotified').text("The following " + count + " clinics have been notified that an action is required in order to restore their ability to use the mail-to-patient option. <span style='color:blue'>Select a clinic to re-enable.</span>");
        $('#spnchkAllClinics_Disabled').show();
    }
}

function HideMailToPatient() {
    NoMailReason = $('#rblNoMailToPaientReason').find('input:checked').val();
    $('#divClinicEnableDisable').hide();
    $('#divClinicDisableNotified').hide();
    $('#divClinicsToEnable').hide();
    $('#divClinicsToDisable').hide();
    $('#divNoCapabilityReason').show();
    $('#spnNoCapabilityReason').hide();
    $('#divCapabilityDateRange').show();
    $('#divEndDate').show();
    if (NoMailReason == "Other") {
        $('#divRestartDate').hide();
    }
    else {
        $('#divRestartDate').show();
    }
    $('#divStartDate').hide();
}

function ValidateComments() {
    IsLabMailToPatient = $('#rblMailToPatient').find('input:checked').val();
    NoMailReason = $('#rblNoMailToPaientReason').find('input:checked').val();
    if (IsLabMailToPatient != "true") {
        if (NoMailReason == "Other") {
            if ($('#txtOtherComments').val() == "") {
                alert("Comments are required if 'Other' is selected.");
                $('#spnComments').show();
                $('#txtOtherComments').focus();
                return false;
            }
            else {
                return true;
            }
        }
        else {
            return true;
        }
    }
    else {
        return true;
    }
}


function SetDefaultDates(status) {
    var today = getTodaysDate();
    if (status == 'off') {
        // set default stop date
        var defaultstopdate = getResultDate(new Date(today));
        document.getElementById('txtStopDate').value = getResultDate(new Date(today));

        // set default start (resume/restart) date
        document.getElementById('txtRestartDate').value = getResultDate(new Date(defaultstopdate));
        return
    }
    if (status == 'on') {
        // set default start date
        document.getElementById('txtStartDate').value = getResultDate(new Date(today));
        return
    }
    if (status == 'disableclinic') {
        // set default clinic stop date
        document.getElementById('txtClinicStopDate').value = getResultDate(new Date(today));
        return;
    }
}


function ValidateStopDate(e) {
   var selecteddate = $("#txtStopDate").val();
    var today = getTodaysDate();
    var defaultdate = getResultDate(new Date(today));
    if (selecteddate == "") {
        $('#spnRequired').show
        document.getElementById('txtRestartDate').value = getResultDate(new Date(selecteddate));
        $("#txtStopDate").focus;
    }
   
    if (selecteddate != "") {
        if (new Date(selecteddate) < new Date(today) || new Date(selecteddate) < new Date(defaultdate)) {
            document.getElementById('txtStopDate').value = defaultdate;
            alert("Availability End Date must be at least 3 business days from today.")
            return;
        }
        var restartdate = getResultDate(new Date(selecteddate));
        document.getElementById('txtRestartDate').value = restartdate;
        $('#spnRequired').hide();
        return;

    }
}

function ValidateClinicStopDate() {
    var selecteddate = $("#txtClinicStopDate").val();
    var today = getTodaysDate();
    var defaultdate = getResultDate(new Date(today));
    if (selecteddate == "") {
        document.getElementById('txtClinicStopDate').value = defaultdate;
        alert("Stop Date is required and must be at least 3 business days from today.")
        $('#spnClinicStoptDate').attr("style", "color:red");
        return;
    }
    if (selecteddate != "") {
        if (new Date(selecteddate) < new Date(today) || new Date(selecteddate) < new Date(defaultdate)) {
            document.getElementById('txtClinicStopDate').value = defaultdate;
            alert("Stop Date must be at least 3 business days from today.")
            $('#spnClinicStoptDate').attr("style", "color:red");
            return;
        }
        else {
            $('#spnClinicStoptDate').attr("style", "color:transparent");
            return;
        }
    }
}


function ValidateRestartDate(e) {
    var selecteddate = $("#txtRestartDate").val();
    var stopdate = $("#txtStopDate").val();
    var today = getTodaysDate();
    var defaultdate = getResultDate(new Date(stopdate));
    if (selecteddate == "") {
        $('#spnRequired').show
        document.getElementById('txtRestartDate').value = getResultDate(new Date(selecteddate));
        $("#txtStopDate").focus;
    }
    if (selecteddate != "") {
        if (new Date(selecteddate) < new Date(today) || new Date(selecteddate) < new Date(defaultdate)) {
            document.getElementById('txtRestartDate').value = defaultdate;
            alert("Resume Date cannot be prior to today's date and must be at least 3 business days from Availability End Date.")
            return;
        }
        var restartdate = getResultDate(new Date(stopdate));
        $('#spnRequired').hide();
        return;
    }
    else {
        $('#spnRequired').show();
        alert("Resume date is required.");
        document.getElementById('txtRestartDate').value = getResultDate(new Date(today));
        $("#txtRestartDate").focus;
    };
}

function ValidateClinicRestartDate(e) {
    var selecteddate = $("#txtClinicRestartDate").val();
    var CurrentDate = new Date().setHours(0,0,0,0);
    //CurrentDate.setHours(0,0,0,0)
    selecteddate = new Date(selecteddate);


    if (selecteddate != "") {

        if (selecteddate < CurrentDate) {
            document.getElementById('txtClinicRestartDate').value = "";
            alert("Resume Date cannot be prior to today's date.")
            return;
        }
    }
}


function ValidateStartDate(e) {
    var selecteddate = $("#txtStartDate").val();
    var today = getTodaysDate().setHours(0,0,0,0);
    if (selecteddate != "") {
        if (new Date(selecteddate) < new Date(today)) {
            document.getElementById('txtStartDate').value = today;
            alert("Start Date cannot be prior to today's date.")
            return;
        }
    }
}



function getResultDate(date) {
    var numBusinessDays = 3;
    var holidays = {
        /* 
          Holidays are based off the 2019 calendar.
          Months are zero-based. 
          0 = Jan, 
          1 = Feb,
          2 = Mar,
          3 = Apr,
          4 = May,
          5 = June,
          6 = July,
          7 = Aug,
          8 = Sept,
          9 = Oct,
          10 = Nov,
          11 = Dec 
        */
        0: [1, 21], //Jan 1st -> New Years, Jan 21st -> MLK day
        1: [18], //Feb 18th -> George's Bday
        4: [27], //May 27th -> Memorial Day
        6: [4], //July 4th -> Independence Day
        8: [2], //Sept 2nd -> Labor Day
        9: [14], //Oct 14th -> Columbus Day
        10: [11, 28], //Nov11th -> Veteran's Day and Nov 28th -> Thanksgiving
        11: [25] //Dec 25th -> Christmas
    };

    var saturday = 6;
    var sunday = 0;

    var dayOfWeek = null;
    var dayOfMonth = null;
    var month = null;
    var isWeekday = null;
    var monthHolidays = null;
    var isHoliday = null;
    var dayCounter = -1;


    while (numBusinessDays > 0) {
        dayOfWeek = date.getDay();
        dayOfMonth = date.getDate();
        month = date.getMonth();
        isWeekday = dayOfWeek !== saturday && dayOfWeek !== sunday;
        monthHolidays = holidays[month];
        isHoliday = monthHolidays ? monthHolidays.indexOf(dayOfMonth) > -1 : false;
        //console log days skipped for clarity
        if (isWeekday) {
            if (isHoliday) {
               // console.log("Skipping " + formatDate(date) + " because it's a holiday and does not count towards the business days.");
            }
            else {
                dayCounter++;
                if (dayCounter > 0) {
                   // console.log("Skipping business day " + dayCounter + ", which is " + formatDate(date) + ".");
                }
            }
        }
        if (!isWeekday) {
           // console.log("Skipping " + formatDate(date) + " because it's a weekend, and does not count towards the business days.")
        };
        //remove any days that are weekends or holidays
        if (isWeekday && !isHoliday)--numBusinessDays;
        //adjust the ending date accordingly.
        date.setDate(dayOfMonth + 1);
    }

    //check to adjust ending date if it ends on a weekend or holiday.
    switch (dayOfWeek + 1) {
        //check to see if ending date is a weekend, if so push to next business day.
        case 6:
           // console.log("WARNING: The final business day was a Saturday. Changing it to the next business day.");
            date.setDate(dayOfMonth + 3);
            break;
        case 0:
           // console.log("WARNING: The final business day was a Sunday. Changing it to the next business day.");
            date.setDate(dayOfMonth + 2);
            break;
        //ending date is not a weekend...
        default:
            //double check to see if the ending day is a holiday. If so, add one more day
            month = date.getMonth();
            dayOfMonth = date.getDate();
            monthHolidays = holidays[month];
            isHoliday = monthHolidays ? monthHolidays.indexOf(dayOfMonth) > -1 : false;
            //it's a holiday!
            if (isHoliday) {
               // console.log("The final business day was " + formatDate(date) + ".");
                date.setDate(dayOfMonth + 1);
                ////console.log("WARNING: That day is a holiday! Therefore, changing the ending date to " + formatDate(date) + ".");
            }
            //not a holiday
            else {
               // console.log("The final business day was " + formatDate(date) + ".")
            }
            break;
    }
    return formatDate(date)
}

function formatDate(date) {
    var dd = date.getDate();
    var mm = date.getMonth() + 1;
    var y = date.getFullYear();
    var FormattedEndDate = mm + '/' + dd + '/' + y;
    return FormattedEndDate;
}

//get today's date
function getTodaysDate() {
    var todayDate = new Date();
    return todayDate;
}



// This is used to capture the partial postback and determine if the success dialog should be shown.
Sys.Application.add_load(function () {
    //// The success value is set in code behind to 1.
    if ($('#hdfNotificationValue').val() != '1') return;

    // The display message is set in code behind.
    var msg = $('#hdfNotificationMessage').val();

    // Call function to show message.
    displaySrtsMessage('Success!', msg, 'success');

    //// Reset the success hidden field to 0 so not to re-show the success dialog on the next partial postback.
    $('#hdfNotificationValue').val('0');

    //var msg = $('#hdfNotificationMessage').val();
    //if ($('#hdfNotificationValue').val() == 'error') {
    //    // Call function to show error message.
    //    displaySrtsMessage('Error!', msg, 'error');
    //    return;
    //}
    //else if ($('#hdfNotificationValue').val() == 'success') {
    //    // Call function to show success message.
    //    displaySrtsMessage('Success!', msg, 'success');
    //}

    ////// Reset the success hidden field to 0 so not to re-show the success dialog on the next partial postback.
    //$('#hdfNotificationValue').val('0');
});

///// begin  my objects ////////////////////////////////////

////////  MailToPatientEntity 
///
/// To use... var entity = new MailToPatientEneity('000003','000145','0','NoCapacity', etc.....)
////////////////////////////////////////////////////////////////////////////////////////////////
function MailToPatientEntity(sitecode, clinicsitecode, clinicactionrequired,
                    statusreason, iscapabilityon, statusreason, comments,
                    startdate, stopdate, anticipatedrestartdate) {
    this.sitecode = sitecode;
    this.clinicsitecode = clinicsitecode;
    this.clinicactionrequired = clinicactionrequired;
    this.statusreason = statusreason
    this.iscapabilityon = iscapabilityon;
    this.statusreason = statusreason;
    this.comments = comments;
    this.startdate = startdate;
    this.stopdate = stopdate;
    this.anticipatedrestartdate = anticipatedrestartdate;
}



////// end objects /////////////////////////////