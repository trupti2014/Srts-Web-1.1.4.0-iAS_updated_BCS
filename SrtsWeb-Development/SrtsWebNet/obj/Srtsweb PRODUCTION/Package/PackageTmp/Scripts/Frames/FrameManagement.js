/// <reference path="../Global/jquery-1.11.1.min.js" />

function DoToggle(a, b) {
    var i = $('#' + b);
    var current = i.attr("src");
    var swap = i.attr("data-swap");
    i.attr('src', swap).attr('data-swap', current);
    $('#' + a).toggle();
};

// This rule states that a FOC cannot be an insert at the same time.
$(function () {
    $('#cbIsFoc').on('click', function () {
        $('#cbIsInsert').prop('checked', false);
    });
    $('#cbIsInsert').on('click', function () {
        $('#cbIsFoc').prop('checked', false);
    });
});
