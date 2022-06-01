/// <reference path="jquery-1.11.1.min.js" />


/*
In order for the replace all to work you must add the a css class name 'ascii' to the following controls.  This is used strictly to access this group of controls and NOT for styling.
 - Address1
 - Address2
 - City
*/

function HideAllDivs() {
    $('#divA').hide();
    $('#divE').hide();
    $('#divI').hide();
    $('#divO').hide();
    $('#divU').hide();
    $('#divY').hide();
    $('#divN').hide();
    $('#divMisc').hide();
    $('#divButtons').hide();
}

function DoDialog() {
    var dialogOpts = {
        autoOpen: false,
        modal: false,
        width: 250,
        height: 300,
        title: 'ASCII Character Chooser',
        dialogClass: 'generic',
        open: function () {
            var s = $('#selLetterGroup');
            s.val('X');
            HideAllDivs();
            s.focus();
        },
        close: function () {
            $(this).dialog('destroy');
            $(this).hide();
        }
    };
    var d = $('#divAsciiChooser').dialog(dialogOpts);
    d.parent().appendTo($('form:first'));
    d.dialog('open');
}

function getInputSelection(el) {
    var start = 0, end = 0, normalizedValue, range,
        textInputRange, len, endRange;

    if (typeof el.selectionStart == "number" && typeof el.selectionEnd == "number") {
        start = el.selectionStart;
        end = el.selectionEnd;
    }
    else {
        range = document.selection.createRange();

        if (range) {
            len = el.val().length;
            normalizedValue = el.val().replace(/\r\n/g, "\n");

            // Create a working TextRange that lives only in the input
            textInputRange = el[0].createTextRange();
            textInputRange.moveToBookmark(range.getBookmark());

            // Check if the start and end of the selection are at the very end
            // of the input, since moveStart/moveEnd doesn't return what we want
            // in those cases
            endRange = el[0].createTextRange();
            endRange.collapse(false);

            if (textInputRange.compareEndPoints("StartToEnd", endRange) > -1) {
                start = end = len;
            }
            else {
                start = -textInputRange.moveStart("character", -len);
                start += normalizedValue.slice(0, start).split("\n").length - 1;

                if (textInputRange.compareEndPoints("EndToEnd", endRange) > -1) {
                    end = len;
                }
                else {
                    end = -textInputRange.moveEnd("character", -len);
                    end += normalizedValue.slice(0, end).split("\n").length - 1;
                }
            }
        }
        document.selection.clear();
    }

    return {
        start: start,
        end: end
    };
}

function replaceSelectedText(el, text) {
    var sel = getInputSelection(el);
    if (sel.start == 0 && sel.end == 0) return;
    var val = el.val();
    el.val(val.slice(0, sel.start) + String.fromCharCode(text) + val.slice(sel.end));
}

function DoReplace() {
    var selLetter = $('#selLetterGroup').find('option:selected').val();
    var selAscii = $('#rblLetter' + selLetter).find('input:checked').val();

    replaceSelectedText($('#' + document.selection.createRange().parentElement().id), selAscii.substring(1));
}

function DoReplaceAll() {
    var selLetter = $('#selLetterGroup').find('option:selected').val();
    var selValue = $('#rblLetter' + selLetter).find('input:checked').val();
    var selAscii;

    if (selValue.indexOf('u') == 0 && selValue.indexOf('l') == 0) {
        selAscii = selValue;
        selValue = 'u';
    }
    else {
        selAscii = selValue.substring(1);
        selValue = selValue.substring(0, 1);
    }

    var r = new RegExp(selValue == 'u' ? selLetter : selLetter.toLowerCase(), 'g');

    $('.ascii').val(
        function (i, v) {
            return v.replace(r, String.fromCharCode(selAscii));
        });
}

function DoDdlChange() {
    var a = $('#selLetterGroup').val();
    HideAllDivs();
    if (a == 'X') return;
    $('#div' + a).show();
    $('#divButtons').show();

    if (a == 'Misc')
        $('#bReplaceAll').hide();
    else
        $('#bReplaceAll').show();
}