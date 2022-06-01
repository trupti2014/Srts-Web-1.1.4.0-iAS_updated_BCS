$(document).ready(function () {
    resetPanels();
    showLabPanels();
    //var bQuickSearch = document.getElementById("bQuickSearch");


    //document.getElementById("tbID")
    //.addEventListener("keypress", function (event) {
    //   // event.preventDefault();
    //    if (event.keyCode === 13) {
    //       // alert("text entered");
    //        bQuickSearch = document.getElementById("bQuickSearch");
    //        bQuickSearch.click();
    //    }
    //});


    $('#lnkPatientInfo').click(function () {
        hidePanels();
        clearActive();
        $('#pnlPatientInfo').show();
        $("#lstPatientInfo").addClass("active");
        return false;
    });
    $('#lnkOrderInfo').click(function () {
        hidePanels();
        clearActive();
        $('#pnlOrderInfo').show();
        $("#lstOrderInfo").addClass("active");
        return false;
    });
    $('#lnkOrderStatus').click(function () {
        hidePanels();
        clearActive();
        $('#pnlOrderStatusHistory').show();
        $("#lstOrderStatus").addClass("active");
        return false;
    });
    $('#lnkPrescription').click(function () {
        hidePanels();
        clearActive();
        $('#pnlOrderPrescription').show();
        $("#lstPrescription").addClass("active");
        return false;
    });

});


function GetKeyCode(evt) {
    var keyCode;
    if (evt.keyCode == 13) {
        if (QuickSearchHasText($('#tbID').val())) {
            document.getElementById("btnEnter").click();
        }
    }
}

function resetPanels() {
    hidePanels();
    clearActive();
    hideLinks();
}
function hidePanels() {
    $('#pnlPatientInfo').hide();
    $('#pnlOrderInfo').hide();
    $('#pnlOrderStatusHistory').hide();
    $('#pnlOrderPrescription').hide();
};
function hideLinks() {
    $('#lnkPatientInfo').hide();
    $('#lnkOrderInfo').hide();
    $('#lnkOrderStatus').hide();
    $('#lnkPrescription').hide();
};
function clearActive() {
    $("#lstPatientInfo").removeClass("active");
    $("#lstOrderInfo").removeClass("active");
    $("#lstOrderStatus").removeClass("active");
    $("#lstPrescription").removeClass("active");
};
function showLabPanels() {
    $('#lnkPatientInfo').show();
    $('#lnkPrescription').show();
    $('#lnkOrderInfo').show();
    $('#lnkOrderStatus').show();

    $('#pnlPatientInfo').show();
    $("#lstPatientInfo").addClass("active");
};


function PerformFunction(funcName, para1, para2) {
    window[funcName](para1, para2);
};

function IsFunctionValid(name, isValid) {
    switch (name) {
        case 'clear':
            if (isValid != "true") {
                alert("The grid is empty!");
                return false;
            }
            break;
        default:
            return true;
    }
};
function QuickSearchHasText(t) {
    return t.length > 0;
};

function SearchNameHasText(n) {
    return n.length > 0;
};
