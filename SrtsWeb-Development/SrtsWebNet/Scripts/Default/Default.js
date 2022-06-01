/// <reference path="../Global/jquery-1.11.1.min.js" />
/// <reference path="../Global/jquery-ui.min.js" />

$(function () {
    // script for Did You Know...?
    DoCarousel();
});

function DoDialog() {
    var dialogOpts = {
        autoOpen: false,
        modal: true,
        width: 225,
        height: 300,
        title: 'Available Site Codes',
        dialogClass: 'generic',
        closeOnEscape: true,
        close: function (event, ui) {
            $(this).dialog('destroy');
            $(this).hide();

            //SharedRedirect(window.location.href.substring(0, window.location.href.indexOf(window.location.host, 1) + window.location.host.length) + '/WebForms/default.aspx');
            //window.location.href = h + '/WebForms/default.aspx';
        }
    };
    var dsd = $('#divSitesDialog').dialog(dialogOpts);
    dsd.parent().appendTo($('form:first'));

    dsd.dialog('open');
}

function DoCarousel() {
    var slideIndex = 0;
    var i;
    var x = $('.mySlides');
    for (i = 0; i < x.length; i++) {
        x[i].style.display = 'none';
    }
    slideIndex++;
    if (slideIndex > x.length) { slideIndex = 1 }
    x[slideIndex - 1].style.display = 'block';
    setTimeout(DoCarousel, 7000);
}