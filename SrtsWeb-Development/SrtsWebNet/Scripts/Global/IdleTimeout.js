/// <reference path="jquery-1.11.1.min.js" />

var idleTimer = null;
var idleState = false;
var idleWait = TimeOut;
var extraWait = 300000;
var loggedout = false;
var dialogOpts = {
    autoOpen: false,
    modal: true,
    width: 550,
    height: 150,
    title: 'Logout Warning',
    dialogClass: 'logoutWarning'
};

(function ($) {
    $(document).ready(function () {
        var d = $("#dialog").dialog(dialogOpts);
        $('*').bind('mousemove scroll', function () {
            if (loggedout) return;
            var p = window.location.pathname.substring(window.location.pathname.lastIndexOf('/') + 1).toLowerCase();
            if (p == "login.aspx" || p == "logout.aspx") return;

            d.dialog('close');
            clearTimeout(idleTimer);
            idleState = false;

            idleTimer = setTimeout(function () {
                // Idle Event
                idleState = true;
                d.html('<br /><br /><p>Due to inactivity you will be logged out in 5 minutes</p>').dialog('open');

                idleTimer = setTimeout(function () {
                    loggedout = true;
                    d.dialog('close');
                    SharedRedirect(window.location.href.substring(0, window.location.href.indexOf(window.location.host, 1) + window.location.host.length) + '/WebForms/Account/Logout.aspx?cs=2');
                    //window.location.href = h + '/WebForms/Account/Logout.aspx?cs=2';
                }, extraWait);
            }, idleWait);
        });

        $("body").trigger("mousemove");
    });
})(jQuery)