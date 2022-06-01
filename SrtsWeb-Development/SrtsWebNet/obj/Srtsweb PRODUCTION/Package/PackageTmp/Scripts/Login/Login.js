/// <reference path="../Global/jquery-1.11.1.min.js" />
/// <reference path="../Global/jquery-ui.min.js" />

$(function () {
    sessionStorage.clear();
});

function pageLoad() {
    getActiveLoginTab();
}

function DoSecurityDialog() {
    var dOpts = {
        autoOpen: false,
        modal: true,
        width: 800,
        //height: 620,
        closeOnEscape: false,
        title: 'Security Acknowledgement',
        dialogClass: 'generic',
        open: function (event, ui) {
            $(".ui-dialog-titlebar-close", ui.dialog | ui).hide();

            $('#divSecurityMessage').focus();
            //$('[id$=bSecurityMessage]').focus();
        }
    };

    var d1 = $('#divSecurityMessage').dialog(dOpts);
    d1.dialog('open');
}

function DisableAfterClick(myButton) {
    if (typeof (Page_ClientValidate) == 'function') {
        if (Page_ClientValidate() == false)
        { return false; }
    }

    if (myButton.getAttribute('type') == 'button') {
        myButton.disabled = true;
    }
    return true;
}

function SetTabFocus() {
    var tabCntr = $(LoginTabs);

    if (tabCntr[0].control.get_activeTabIndex() == 1) {
        window.setTimeout(function () {
            document.getElementById("MainContent_Public_tbcLogin_tbpUserName_LoginUser_UserName").focus();
        }, 0);
    }
}

// gets current login tab 
function getActiveLoginTab() {
    // identify login tab container
    var tabCntr = $(LoginTabs);

    // if current active tab is Cac, get registration code
    if (tabCntr[0].control.get_activeTabIndex() == 0) {
        getCacRegistrationCode();
    }
}

// gets current Cac Registration Code 
function getCacRegistrationCode() {
    var CacRegistrationCode = $('[id$=hdfCacRegistrationCode]').val();
    if (CacRegistrationCode == "7") {
        document.getElementById('modalAlert').style.display = 'block';
    }
}
