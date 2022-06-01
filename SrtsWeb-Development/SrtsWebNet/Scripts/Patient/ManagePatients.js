/// <reference path="../Global/jquery-1.11.1.min.js" />

$(window).load(function () {
    var urlPath = location.pathname.split("/")[3];

    var pageMode = location.pathname.substring((location.pathname.lastIndexOf('/') + 1));
    if (pageMode == "search") {
        window.setTimeout(function () {
            $('[id*=tbLastName]').focus();
        }, 50);
    }
    else {
        window.setTimeout(function () {
            $('[id*=ddlIDNumberType]').focus();
        }, 50);
    }
})