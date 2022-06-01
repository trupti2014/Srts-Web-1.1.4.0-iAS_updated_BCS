/// <reference path="jquery-1.11.1.min.js" />

// Validates ID number length based on ID type.
function ValidateIdNumber(idVal, idType) {
    var len = 0;
    var type = "";
    switch (idType) {
        case "DIN":
            len = 10;
            type = "DoD ID";
            break;
        case "SSN":
            len = 9;
            type = "Social Security";
            break;
        case "PIN":
            len = 11;
            type = "Provider ID";
            break;
        case "DBN":
            len = 11;
            type = "DoD Benifits";
            break;
        case "X":
            return "Please select ID Type";
        default:
            // TBD
            break;
    }

    if (idVal.length == len) return "";

    return type + " Number must be " + len + " digits";
}

// This will limit number of characters input to Comment textbox
function textboxMaxCommentSize(txt, maxLen, e, lblID, tbID) {
    var len = txt.value.length;
    var remaining = maxLen;
    //debugger;
    try {
        if ((len > (maxLen - 1)) && ((e.keyCode != 8) && (e.keyCode != 46))) {
            return false;
        }
        else {
            switch (e.keyCode) {
                case 8:  // User hit backspace key
                    if (len > 0) {
                        remaining = maxLen - (len - 1);
                    }
                    else {
                        remaining = maxLen;
                    }
                    break;

                case 46:  // User hit delete key
                    var text = document.getElementById(tbID);
                    if (document.selection != undefined) {
                        var selected = document.selection.createRange();
                        var selectedLength = selected.text.length;

                        if (selectedLength == len) {
                            remaining = maxLen;
                        }
                        else {
                            remaining = maxLen - (len - selectedLength);
                        }
                    }
                    break;

                default:
                    remaining = maxLen - (len + 1);
            }
            if (lblID != undefined) {  // If label parameter was passed in, then show remaining
                document.getElementById(lblID).innerHTML = "(" + remaining + " characters remaining)";
            }
        }
    } catch (e) {
    }
}

// Validates the comment format
function textboxCommentValidation(sender, args) {
    try {
        var input = args.Value;
        var stop = false;
        var len = input.length;
        var item = "";

        // Blacklist
        var badCombos = [
        "''",
        "//",
        "--",
        "' '"
        ];

        if (len > 0) {
            for (var i = 0; i < badCombos.length; i++) {
                item = badCombos[i];
                if (input.indexOf(item) != -1) {
                    args.IsValid = false;
                    stop = true;
                }
            }

            // Whitelist
            if (!stop) {
                if ((input.match(/^[a-zA-Z0-9\s+-.,!?#'\/():]*$/)) != null) {
                    args.IsValid = true;
                }
                else {
                    args.IsValid = false;
                }
            }
        }
    } catch (e) {
    }
}

// Ensures at least one type is selected
function ClientValidateIndTypeCBs(sender, args) {
    args.IsValid = $('#cbProvider').is(':checked') == true || $('#cbTechnician').is(':checked') == true || $('#cbAdministrator').is(':checked') == true || $('#cbPatient').is(':checked') == true;
}

function ValidatePassword(source, arguments) {
    MinTwoUpperCaseLetters(source, arguments);
    MinTwoLowerCaseLetters(source, arguments);
    MinTwoNumbers(source, arguments);
    MinTwoSpecialCharacters(source, arguments);
}

function MinTwoUpperCaseLetters(strToVal) {
    return strToVal.match(/^(.*?[A-Z]){2,}.*$/) != null;
}
function MinTwoLowerCaseLetters(strToVal) {
    return strToVal.match(/^(.*?[a-z]){2,}.*$/) != null;
}
function MinTwoNumbers(strToVal) {
    return strToVal.match(/^(.*?[0-9]){2,}.*$/) != null;
}
function MinTwoSpecialCharacters(strToVal) {
    return strToVal.match(/^(.*?[\&!@#\$%\^\*\(\)]){2,}.*$/) != null;
}