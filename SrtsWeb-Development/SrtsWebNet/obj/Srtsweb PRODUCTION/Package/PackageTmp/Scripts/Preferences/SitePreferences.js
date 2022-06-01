/// <reference path="../Global/jquery-1.11.1.min.js" />

$(function () {
    var userRole = $('#hdfUserRole').val();
    var siteType = $('#hdfSiteType').val();

  
    resetPanels();
    if (userRole == 'labadmin') {
        showLabPanels();
    }
    if (userRole == 'clinicadmin') {
        showClinicPanels();
    }

    if (userRole == 'mgmtenterprise' && siteType == 'LAB') {
        showLabPanels();
    }
    else if (userRole == 'mgmtenterprise' && (siteType == 'CLINIC' || siteType == 'ADMIN')) {
        showClinicPanels();
    }
    $('#lnkOrders').click(function () {
        hidePanels();
        clearActive();
        $('#pnlOrders').show();
        $("#lstOrders").addClass("active");
        return false;
    });
    $('#lnkFrames').click(function () {
        hidePanels();
        clearActive();
        $('#pnlFrames').show();
        $("#lstFrames").addClass("active");
        return false;
    });
    $('#lnkPrescriptions').click(function () {
        hidePanels();
        clearActive();
        $('#pnlPrescriptions').show();
        $("#lstPrescriptions").addClass("active");
        return false;
    });
    $('#lnkGeneral').click(function () {
        hidePanels();
        clearActive();
        $('#pnlGeneral').show();
        $("#lstGeneral").addClass("active");
        return false;
    });
    $('#lnkLabParameters').click(function () {
        hidePanels();
        clearActive();
        $('#pnlLabParameters').show();
        $("#lstLabParameters").addClass("active");
        return false;
    });
    $('#lnkLabJustifications').click(function () {
        hidePanels();
        clearActive();
        $('#pnlLabJustifications').show();
        $("#lstLabJustifications").addClass("active");
        return false;
    });
    $('#lnkLabMailToPatient').click(function () {
        hidePanels();
        clearActive();
        $('#pnlLabMailToPatient').show();
        $("#lstLabMailToPatient").addClass("active");
        return false;
    });
    $('#lnkRoutingOrders').click(function () {
        hidePanels();
        clearActive();
        $('#pnlRoutingOrders').show();
        $("#lstRoutingOrders").addClass("active");
        return false;
    });
    $('#lnkShipping').click(function () {
        hidePanels();
        clearActive();
        $('#pnlShipping').show();
        $("#lstShipping").addClass("active");
        return false;
    });

});
function resetPanels() {
    hidePanels();
    clearActive();
    hideLinks();
}

function hidePanels() {
    $('#pnlFrames').hide();
    $('#pnlPrescriptions').hide();
    $('#pnlOrders').hide();
    $('#pnlGeneral').hide();
    $('#pnlLabParameters').hide();
    $('#pnlLabJustifications').hide();
    $('#pnlLabMailToPatient').hide();
    $('#pnlRoutingOrders').hide();
    $('#pnlShipping').hide();
};
function hideLinks() {
    $('#lnkPrescriptions').hide();
    $('#lnkFrames').hide();
    $('#lnkOrders').hide();
    $('#lnkLabParameters').hide();
    $('#lnkLabJustifications').hide();
    $('#lnkLabMailToPatient').hide();
    $('#lnkRoutingOrders').hide();
    $('#lnkShipping').hide();
};
function clearActive() {
    $("#lstOrders").removeClass("active");
    $("#lstFrames").removeClass("active");
    $("#lstPrescriptions").removeClass("active");
    $("#lstGeneral").removeClass("active");
    $('#lstLabParameters').removeClass("active");
    $('#lstLabJustifications').removeClass("active");
    $('#lstLabMailToPatient').removeClass("active");
    $('#lstRoutingOrders').removeClass("active");
    $('#lstShipping').removeClass("active");
};
function showLabPanels() {
    $('#lnkGeneral').show();
    $('#lnkLabJustifications').show();
    $('#lnkLabMailToPatient').show();
    $('#lnkRoutingOrders').show();
    $('#lnkLabParameters').show();
    $('#pnlLabParameters').show();
    $('#lnkShipping').show();
    $("#lstLabParameters").addClass("active");
    $('#lnkLabMailToPatient').show();
}
function showClinicPanels() {
    $('#lnkGeneral').show();
    $('#lnkPrescriptions').show();
    $('#lnkFrames').show();
    $('#lnkOrders').show();
    $('#lnkShipping').show();
    $('#pnlOrders').show();
    $("#lstOrders").addClass("active");
}
