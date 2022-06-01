/// <reference path="../Global/jquery-1.11.1.min.js" />

$(function () {
 
    //resetPanels();

    $('#lstClinicAdmins').click(function () {
        hidePanels();
        clearActive();
        $('#pnlClinicAdmins').show();
        $("#lstClinicAdmins").addClass("active");
        return false;
    });
    $('#lstLabAdmins').click(function () {
        hidePanels();
        clearActive();
        $('#pnlLabAdmins').show();
        $("#lstLabAdmins").addClass("active");
        return false;
    });


});
function resetPanels() {
    hidePanels();
    clearActive();
    hideLinks();
}

function hidePanels() {
    $('#pnlClinicAdmins').hide();
    $('#pnlLabAdmins').hide();
}

function hideLinks() {
    $('#lnkClinicAdmins').hide();
    $('#lnkLabAdmins').hide();
}

function clearActive() {
    $("#lstClinicAdmins").removeClass("active");
    $("#lstLabAdmins").removeClass("active");
}

