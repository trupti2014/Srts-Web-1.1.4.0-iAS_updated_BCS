/// <reference path="../Global/jquery-1.11.1.min.js" />
$(document).ready(function () {
    $('#tbPDNear').on("focus", function () {
        $('#rblPDNear').find('input[value="DEFAULT"]').prop('checked', true);
    });


    $('#tbPD').on("focus", function () {
        $('#rblPDDistance').find('input[value="DEFAULT"]').prop('checked', true);
    });



})

function DoRblPDNearChange() {
    // get value of radiobutton pdNear
    var rblPDNear = $('#rblPDNear').find('input:checked').val();
    if (rblPDNear == "REQUIRE") {
        $('#tbPDNear').val("");
    }
}
function DoRblPDDistanceChange() {
    // get value of radiobutton pdDistance
    var rblPDDistance = $('#rblPDDistance').find('input:checked').val();
    if (rblPDDistance == "REQUIRE") {
        $('#tbPD').val("");
    }
}






