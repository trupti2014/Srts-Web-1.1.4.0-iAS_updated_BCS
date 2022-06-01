// Moved this functioanlity to the jsValadators.js file
// Need to delete this file during cleanup

//// This will limit number of characters input to Comment textbox
//function textboxMaxCommentSize(txt, maxLen, e, lblID, tbID) {
//    var len = txt.value.length;
//    var remaining = maxLen;
//    //debugger;
//    try {
//        if ((len > (maxLen - 1)) && ((e.keyCode != 8) && (e.keyCode != 46))) {
//            return false;
//        }
//        else {
//            switch (e.keyCode) {
//                case 8:  // User hit backspace key
//                    if (len > 0) {
//                        remaining = maxLen - (len - 1);
//                    }
//                    else {
//                        remaining = maxLen;
//                    }
//                    break;

//                case 46:  // User hit delete key
//                    var text = document.getElementById(tbID);
//                    if (document.selection != undefined) {
//                        var selected = document.selection.createRange();
//                        var selectedLength = selected.text.length;

//                        if (selectedLength == len) {
//                            remaining = maxLen;
//                        }
//                        else {
//                            remaining = maxLen - (len - selectedLength);
//                        }
//                    }
//                    break;

//                default:
//                    remaining = maxLen - (len + 1);
//            }
//            if (lblID != undefined) {  // If label parameter was passed in, then show remaining
//                document.getElementById(lblID).innerHTML = "(" + remaining + " characters remaining)";
//            }
//        }
//    } catch (e) {
//    }
//}

//// Validates the comment format
//function textboxCommentValidation(sender, args) {
//    try {
//        var input = args.Value;
//        var stop = false;
//        var len = input.length;
//        var item = "";

//        // Blacklist
//        var badCombos = [
//        "''",
//        "//",
//        "--",
//        "' '"
//        ];

//        if (len > 0) {
//            for (var i = 0; i < badCombos.length; i++) {
//                item = badCombos[i];
//                if (input.indexOf(item) != -1) {
//                    args.IsValid = false;
//                    stop = true;
//                }
//            }

//            // Whitelist
//            if (!stop) {
//                if ((input.match(/^[a-zA-Z0-9\s+-.,!?#'\/():]*$/)) != null) {
//                    args.IsValid = true;
//                }
//                else {
//                    args.IsValid = false;
//                }
//            }
//        }
//    } catch (e) {
//    }
//}