/// <reference path="../Global/jquery-1.11.1.min.js" />

function pageLoad() {

    $('#divInstruction1').text('This is to enter the tracking number for orders mailed directly to a patient.');
    $('#divInstruction2').text('This tracking information will be visible on the order page as well as on the order status page that the patient can access.');

}

function DoRblScan() {
    // get value of radiobutton InputType
    var rblScan = $('#rblInputType').find('input:checked').val();
    if (rblScan == "Scan") {
        document.getElementById("divPanelManual").style.display = "none";
        document.getElementById("divPanelScan").style.display = "block";

    }
    else {
        document.getElementById("divPanelManual").style.display = "block";
        document.getElementById("divPanelScan").style.display = "none";

    }
}

function Expand(ImageID) {
    var open = document.getElementById(ImageID.id);
    if (open.src == "*=right") {
        $(this).closest("tr").after("<tr><td></td><td cospan = '999'>" + $(this).next().html() + "</td></tr>")
        $(this).attr("src", "~/Styles/images/img_arrow_left.png");
        document.getElementById("divPanelEdit").style.display = "flex";
    }
    else {

        $(this).attr("src", "~/Styles/images/img_arrow_right.png");
        $(this).closest("tr").next().remove();
        document.getElementById("divPanelEdit").style.display = "none";
    }

}

function GetKeyCode(evt) {
    var keyCode;
    if (evt.keyCode == 13) {
        if (QuickSearchHasText($('#txtTrackingNumberS').val())) {
            document.getElementById("btnScan").click();
        }
    }
}

function QuickSearchHasText(t) {
    return t.length > 0;
};

// This is used to capture the partial postback and determine if the success dialog should be shown.
Sys.Application.add_load(function () {
    var TrackSuccess = $('#hfSuccessTrack').val();
    var msg = $('#hfMsgTrack').val();
    if ((TrackSuccess == '' || TrackSuccess == '0') && msg != '') {
        displaySrtsMessage('Error!', msg, 'error');
    }
    else if (TrackSuccess == '1') {
        // Call function to show message.
        displaySrtsMessage('Success!', msg, 'success');
    }

    // Reset the success hidden field to 0 so not to re-show the success dialog on the next partial postback.
    $('#hfSuccessTrack').val('0');
    $('#hfMsgTrack').val('');
});


