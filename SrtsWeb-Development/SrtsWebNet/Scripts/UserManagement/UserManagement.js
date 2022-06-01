/// <reference path="../Global/jquery-1.11.1.min.js" />

$(function () {
    $('[id$=cbApprove]').change(function () {
        DoApproveDisapprove();
    });
});

function DoDialog() {
    var dialogOpts = {
        autoOpen: false,
        modal: true,
        width: 550,
        height: 225,
        title: 'Select individual to link to profile.',
        dialogClass: 'generic',
        closeOnEscape: true,
        close: function (event, ui) {
            $(this).dialog('destroy');
            $(this).hide();

            window.location.href = location.href;
        }
    };
    var dsd = $('#divIndividualLinkDialog').dialog(dialogOpts);
    dsd.parent().appendTo($('form:first'));

    dsd.dialog('open');
}

function DoSetSiteDialog() {
    var siteDialogOpts = {
        autoOpen: false,
        modal: true,
        width: 550,
        height: 155,
        title: 'Select site code to link to profile.',
        dialogClass: 'generic',
        closeOnEscape: true,
        close: function (event, ui) {
            $(this).dialog('destroy');
            $(this).hide();

            window.location.href = location.href;
        }
    };
    var dscd = $('#divSetSiteCode').dialog(siteDialogOpts);
    dscd.parent().appendTo($('form:first'));

    dscd.dialog('open');
}

function ConfirmLastSite(){
    if ($('[id$=lbAssgSiteCode] > option').length == 1)
        return confirm('Do you want to remove the last assigned site?');
}

function ConfirmLastRole() {
    if ($('[id$=lbAssigned] > option').length == 1)
        return confirm('Do you want to remove the last assigned role?');
}

function DoApproveDisapprove(){
    $('[id$=btnSubmit]').removeProp('disabled');
}