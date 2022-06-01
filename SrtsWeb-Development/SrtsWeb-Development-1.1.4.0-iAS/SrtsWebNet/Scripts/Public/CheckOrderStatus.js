/// <reference path="../Global/jquery-1.11.1.min.js" />

function GetCurrentOrderStatusToolTip() {
    $('#divCurrentOrderStatusToolTip').css('visibility', 'visible');
    var sb = new StringBuilder();

    // define hover descriptions
    var strClinicCreateOrder =
        "<div class='descriptionItem' style='text-align:left'><span class='colorBlue'>Clinic Created Order:</span></br> The order has been sent to the lab. If this status is more than 2 business days old, please contact the clinic.</div>";
    var strLabCheckedInOrder =
        "<div class='descriptionItem'><span class='colorBlue'>Lab Checked In Order:</span></br> The lab has received the order. If this status is more than 7 days old, please contact the clinic.</div>";
    var strLabDispensedOrder =
        "<div class='descriptionItem'><span class='colorBlue'>Lab Dispensed Order**:</span></br> The lab has completed the glasses, and has sent them out. If this status remains for more than 5* business days and you have not gotten your glasses, please contact the clinic.</div>";
    var strClinicReceivedOrder =
       "<div class='descriptionItem'><span class='colorBlue'>Clinic Received Order**:</span></br> The clinic has received the glasses. If they are to be mailed to you, and you haven’t received your glasses within 5* days of this status, contact the clinic. If you are supposed to pick them up, they are ready for you. Remember: the clinic will not hold glasses for an extended period. Pick them up soon! </div>";
    var strClinicDispensedOrder =
        "<div class='descriptionItem'><span class='colorBlue'>Clinic Dispensed Order**:</span></br> The clinic has dispensed the glasses. If you did not get your glasses within 5* days of this status, please contact the clinic.</div>";
    var strClinicCancelled =
        "<div class='descriptionItem'><span class='colorBlue'>Clinic Cancelled**:</span></br> The clinic cancelled the order for some reason, usually entered again to correct errors. </div>";
    var strNote = "<div class='descriptionItem'><span class='colorBlue'>If you have questions about any other status, please contact the clinic.</span></div>";
    var strNote1 = "<div class='descriptionItem'><span class='colorBlue'>* Note:</span></br> If you’re in a deployed/remote environment, please factor in mailing delays in getting to you or to the clinic.</div>";
    var strNote2 = "<div class='descriptionItem'>** Any of these may be the final status, depending on the order.</div>";

    sb.append("<div class='w3-row'>");
    sb.append("<div><div style='padding:5px'>");
    sb.append(strClinicCreateOrder);
    sb.append(strLabCheckedInOrder);
    sb.append(strLabDispensedOrder);
    sb.append(strClinicReceivedOrder);
    sb.append(strClinicDispensedOrder);
    sb.append(strClinicCancelled);
    sb.append(strNote);
    sb.append(strNote1);
    sb.append(strNote2);
    sb.append("</div></div>");

    var s = sb.toString();
    $('#divCurrentOrderStatusContent').html(s);

}
function GetTintToolTip() {
    $('#divTintToolTip').css('visibility', 'visible');
    var sb = new StringBuilder();

    // define hover descriptions
    var strCL =
        "<div class='descriptionItem' style='text-align:left'><span class='colorBlue'>Clear:</span>&nbsp;&nbsp; Clear Lenses, No Tint or Coatings</div>";
    var strOther =
        "<div class='descriptionItem'>Anything else is some type of tint or coating. If you need more details, contact the clinic where you ordered.</div>";

    sb.append(strCL);
    sb.append(strOther);
    var s = sb.toString();
    $('#divTintContent').html(s);
}
function GetLabToolTip() {
    $('#divLabToolTip').css('visibility', 'visible');
    var sb = new StringBuilder();

    // define hover descriptions
    var strItem1 =
        "<div class='descriptionItem'>This is the lab that is making your glasses. </div>";
    var strItem2 =
        "<div class='descriptionItem'>The clinic may ask for this information if you need to contact them. </div>";
    var strItem3 =
        "<div class='descriptionItem'>Please do not contact the lab if you have a problem with your order, please contact where you ordered them.</div>";

    sb.append(strItem1);
    sb.append(strItem2);
    sb.append(strItem3);
    var s = sb.toString();
    $('#divLabContent').html(s);
}
function GetOrderDetailToolTip() {
    $('#divOrderDetailToolTip').css('visibility', 'visible');
    var sb = new StringBuilder();

    // define hover descriptions
    var strItem1 =
        "<div class='descriptionItem'>If you call the clinic, they may ask for the <span class='colorBlue'>ordering clinic, order number, or frame</span> information.</div>";
    sb.append(strItem1);
    var s = sb.toString();
    $('#divOrderDetailContent').html(s);
}
function GetOrdersListToolTip() {
    $('#divOrdersListToolTip').css('visibility', 'visible');//.style.visibility = 'visible';
    var sb = new StringBuilder();

    // define hover descriptions
    var strItem1 =
        "<div class='descriptionItem'>Hovering over the bold column headings below will display column specific information.<br /><br />Clicking anywhere in an order row will display that order's detail information.</div>";
    sb.append(strItem1);
    var s = sb.toString();
    $('#divOrdersListContent').html(s);
}
function Hide(d) {
    $('#' + d).css('visibility', 'hidden');//style.visibility = 'hidden';
}
function StringBuilder(value) {
    this.strings = new Array("");
    this.append(value);
}
StringBuilder.prototype.append = function (value) {
    if (value) {
        this.strings.push(value);
    }
}
StringBuilder.prototype.clear = function () {
    this.strings.length = 1;
}
StringBuilder.prototype.toString = function () {
    return this.strings.join("");
}

function IdNumGood() {
    var idN = $('#tbIdNum');
    if (idN.val() == null || idN.val() == '') { alert('ID Number is a required field.'); return false; }
    if (idN.val().length == 0 || idN.val().length < 9 || idN.val().length > 10) { alert('ID Number is either 9 (SSN) or 10 (DODID) digits long.'); return false; }
    return true;
}