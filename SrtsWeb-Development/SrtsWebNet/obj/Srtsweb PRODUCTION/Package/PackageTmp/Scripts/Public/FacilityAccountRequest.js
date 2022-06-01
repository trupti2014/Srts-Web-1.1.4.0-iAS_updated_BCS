/// <reference path="../Global/jquery-1.11.1.min.js" />

function getlblRemainingID() {
    return 'lblRemaining';
}

function gettbCommentID() {
    return 'txtComments';
}

$(function () { }).on('change', $('#ddlState'), function () {
    var s = $('#ddlState').find('option:selected').val();
    if (s == 'UN' || s == '0' || s == 'AA' || s == 'AE' || s == 'AP') return;
    $('#ddlCountry').val('US');
});
