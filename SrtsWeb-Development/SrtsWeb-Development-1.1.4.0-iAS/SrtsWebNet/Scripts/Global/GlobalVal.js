/// <reference path="jquery-1.11.1.min.js" />

// This function works with the asp:customvalidator.  Assuming that 'X' is the default value for a ddl, the function will return true if a value is selected.
function DdlRequriedFieldValControl(source, arguments) {
    var val = arguments.Value;
    if (val == 'X') {
        arguments.IsValid = false;
        return false;
    }
    arguments.IsValid = true;
    return true;
}

function CbxRequiredFieldValCOntrol(source, arguments) {
    var val = $('#' + ctrlId).find('option:selected').val();
    if (val == undefined || val == '' || val == 'X') {
        arguments.IsValid = false;
        return false;
    }
    arguments.IsValid = true;
    return true;
}

function ComboBoxRequiredFieldVal(ctlId) {
    var val = $('[id$=' + ctlId + ']').find('input:hidden').val();

    var ctrl = $('[id$=' + ctlId + ']').find('ul').children('li');

    if (ctrl.length == 1 && val == 0) {
        var len = ctrl.length; //.children('li').length;
        var t = ctrl[len - 1].innerText;
        if (t.toLowerCase() == '-select-') return false;
        return true;
    }
    if (val == undefined || val == '' || val == 0) {
        return false;
    }
    return true;
}
function ComboBoxRequiredFieldValById(ctlId) {
    var val = $('#' + ctlId).find('input:hidden').val();

    var ctrl = $('#' + ctlId).find('ul').children('li');

    if (ctrl.length == 1 && val == 0) {
        var len = ctrl.length; //.children('li').length;
        var t = ctrl[len - 1].innerText;
        if (t.toLowerCase() == '-select-') return false;
        return true;
    }
    if (val == undefined || val == '' || val == 0) {
        return false;
    }
    return true;
}

function DdlRequriedFieldVal(ctrlId) {
    var val = $('[id$=' + ctrlId + ']').find('option:selected').val();
    if (val == undefined || val == '' || val == 'X') {
        return false;
    }
    return true;
}
function DdlRequriedFieldValById(ctrlId) {
    var val = $('#' + ctrlId).find('option:selected').val();
    if (val == undefined || val == '' || val == 'X') {
        return false;
    }
    return true;
}

function DdlIsNullOrEmptyFieldVal(ctrlId) {
    var val = $('[id*=' + ctrlId + ']').find(' option:selected').val();
    if (val == undefined || val == '') {
        return true;
    }
    return false;
}
function DdlIsNullOrEmptyFieldValById(ctrlId) {
    var val = $('#' + ctrlId).find(' option:selected').val();
    if (val == undefined || val == '') {
        return true;
    }
    return false;
}

function UrlVal(newUrl) {
    var url = parseURL(newUrl);
    var urlHostname = url.hostname.trim();

    if (urlHostname == '') {
        return true;
    }
    else {
        if (urlHostname.toUpperCase() == location.hostname.trim().toUpperCase()) {
            return true;
        }
        else
            return false;
    }
}
function parseURL(url) {
    var a = document.createElement('a');
    a.href = url;
    return {
        source: url,
        protocol: a.protocol.replace(':', ''),
        hostname: a.hostname,
        host: a.host,
        port: a.port,
        query: a.search,
        params: (function () {
            var ret = {},
                seg = a.search.replace(/^\?/, '').split('&'),
                len = seg.length, i = 0, s;
            for (; i < len; i++) {
                if (!seg[i]) { continue; }
                s = seg[i].split('=');
                ret[s[0]] = s[1];
            }
            return ret;
        })(),
        file: (a.pathname.match(/\/([^\/?#]+)$/i) || [, ''])[1],
        hash: a.hash.replace('#', ''),
        path: a.pathname.replace(/^([^\/])/, '/$1'),
        relative: (a.href.match(/tps?:\/\/[^\/]+(.+)/) || [, ''])[1],
        segments: a.pathname.replace(/^\//, '').split('/')
    };
}
