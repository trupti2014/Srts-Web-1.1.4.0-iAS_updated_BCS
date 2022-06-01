/// <reference path="../Global/jquery-1.11.1.min.js" />

$(document)
    .on('change', '[id*=rblClinicUnit]', function () { DoClinicUnitAddress($('[id*=rblClinicUnit] input:checked').val()); });
function DoClinicUnitAddress(addressType) {
    if (addressType == 'C') {
        $('[id*=divClinicCode]').show();
        $('[id*=divUnitAddress]').hide();
    }
    else {
        $('[id*=divClinicCode]').hide();
        $('[id*=divUnitAddress]').show();
        $('[id*=tbAddress1]').focus();
    }
}

function MoveUp() {
    if ($('[id*=lbColumns] > option:selected').index() == 0) return;
    $('[id*=lbColumns] > option:selected').insertBefore($('[id*=lbColumns] > option:selected').prev());
}

function MoveDown() {
    if ($('[id*=lbColumns] > option:selected').index() == $('[id*=lbColumns] > option').length - 1) return;
    $('[id*=lbColumns] > option:selected').insertAfter($('[id*=lbColumns] > option:selected').next());
}