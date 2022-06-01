/// <reference path="ManageOrders.js" />
/// <reference path="../Global/jquery-1.11.1.min.js"/>

var OrderNumbersNotInGrid = [], currModule = "";
var rowSelectedCount = 0;
var notAddedCount = 0;
var ordersToPrint = [];
var editState = '';
var ddlState = $('#ddlPrimaryState');
var ddlCountry = $('#ddlPrimaryCountry');
var strValue = document.getElementById("hdfStates");

var previousAddress = {
    address1: "",
    address2: "",
    city: "",
    state: "",
    zip: "",
    country: "",
    Clear: function () {
        this.address1 = "";
        this.address2 = "";
        this.city = "";
        this.state = "";
        this.zip = "";
        this.country = "";
    },
    Set: function () {
        this.address1 = address.address1;
        this.address2 = address.address2;
        this.city = address.city;
        this.zip = address.zip;
        this.country = address.country;
        switch (address.city) {
            case "APO":
                this.state = "AP"
                break;
            case "FPO":
                this.state = "AE"
                break
            case "DPO":
                this.state = "AA"
                break
            default:
                this.state = address.state;
                break;
        }
    }
};

var address = {
    address1: "",
    address2: "",
    city: "",
    state: "",
    zip: "",
    country: "",
    Clear: function () {
        this.address1 = "";
        this.address2 = "";
        this.city = "";
        this.state = "";
        this.zip = "";
        this.country = "";
    },
    Set: function () {
        this.address1 = $('#tbPrimaryAddress1').val();
        this.address2 = $('#tbPrimaryAddress2').val();
        this.city = $('#tbPrimaryCity').val();
        this.state = $('#ddlPrimaryState').val();
        this.country = $('#ddlPrimaryCountry').val();
        this.zip = $('#tbPrimaryZipCode').val();
        previousAddress.Set();
    }
};
function SetAddress() {
    address.Set();
}
function pageLoad() {
    $('#divInstruction').text('Use the single order textbox to enter an order number or scan a 771. To enter multiple order numbers, use the bulk input.  Select the desired label format to use.');

    // hide US Address fields after adding FN address
    rbl = $('#rblAddressType').find('input:checked').val();
    st = $('#divState');
    usAddr = $('#divUsAddress');
    if (ddlState.val == 'NA') {
        alert("foreign");
    }
    if (rbl == 'FN') {
        st.hide();
        // uic.hide();
        usAddr.hide();
    }
    RemoveInValidStates();
    $('#chkUseMailingAddress').on('click', function () {
        ///;
    });

    $('#ddlPrimaryState').on('change', SetPrimaryAddressStCtry);
    $('[id*=tbSingleReadScan]').keydown(function (e) {
        if (e.which == 13) {
            var orderNum = $(this).val().trim().toString().toUpperCase();
            if (orderNum != "") { ordersToPrint.push(orderNum) };
            orderNum = '';
            $('#btnAddToGrid').trigger('click');
            $('[id*=tbSingleReadScan]').focus();
        }
    });
    $('[id*=tbSingleReadScan]').focus();
}


function GetAddressValues() {
    getLocalStateData();
    getForeignCountryData();
    RemoveInValidStates();
    DoRblAddressTypeChangeEdit();
}

function RemoveInValidStates() {
    // removing 'Unknown' and 'Non Applicable' as options in the State dropdown List
    ddlState.find('option[value=UN]').remove();
    ddlState.find('option[value=NA]').remove();
    ddlState.find('option[value=AA]').remove();
    ddlState.find('option[value=AE]').remove();
    ddlState.find('option[value=AP]').remove();
}
function DoRblAddressTypeChangeEdit() {
    if (previousAddress.address1 == "") {
        previousAddress.Set();
    }
    rblAddressType = $('#rblAddressType').find('input:checked').val();
    if (rblAddressType == undefined) {
        //load saved address
        if (state == 'NA') {
            isForeign = 'true';
            $('#rblAddressType').find('input[value=' + fn + ']').prop('checked', true);
            DoAddressTypeChangeEdit(fn, false);
        }
        else if (state != 'X' && state != 'NA') {
            isForeign = 'false';
            $('#rblAddressType').find('input[value=' + us + ']').prop('checked', true);
            getLocalStateData();
            RemoveInValidStates();
            DoAddressTypeChangeEdit(us, false);
        }
        else {
            DoAddressTypeChangeEdit(us, true);
        }
    }
    else if (rblAddressType == 'FN') {
        // do foreign address    
        isForeign = 'true';
        if (previousAddress.state == 'NA') {
            //load current foreign address
            DoAddressTypeChangeEdit(fn, false);
        }
        else {
            //load new foreign address
            DoAddressTypeChangeEdit(fn, true);
        }
    }
    else {
        isForeign = 'false';// do US address
        if (previousAddress.state != 'NA') {
            //load current us address
            DoAddressTypeChangeEdit(us, false);
        }
        else {
            //load new us address
            DoAddressTypeChangeEdit(us, true);
        }
    }
}
function DoAddressTypeChangeEdit(t, isnew) {
    var usAddress = $('#divUsAddress');
    var uicAddress = $('#divUIC');
    var divState = $('#divState');
    var divAddress1 = $('#divAddress1');
    var divAddress2 = $('#divAddress2');
    ClearMsgs('divAddresses', 'divAddressMsg');
    // show selected address type form
    switch (t) {
        case (fn):
            isForeign = 'true';
            //set radio button to foreign
            $('#rblAddressType').find('input[value=' + fn + ']').prop('checked', true);
            getCountries();
            switch (isnew) {
                case (true):
                    //set country to 'select'
                    $('#ddlPrimaryCountry').val('X');
                    // clear address 1
                    $('#tbPrimaryAddress1').val('');
                    //clear address2
                    $('#tbPrimaryAddress2').val('');
                    //set state to 'na' state
                    $('#ddlPrimaryState').val('NA');
                    break;
                case (false):
                    $('#ddlPrimaryCountry').val(previousAddress.country);
                    $('#tbPrimaryAddress1').val(previousAddress.address1);
                    $('#tbPrimaryAddress2').val(previousAddress.address2);
                    break;
            }

            // clear and hide city, state and zip
            SetStatetoNA();

            // clear city, and zip values
            $('#tbPrimaryCity').val("");
            $('#tbPrimaryZipCode').val("");
            $('#rblCity').find('input').removeAttr('checked');

            // show Foreign address form
            usAddress.hide();
            divState.hide();

            break;
        case (us):
            isForeign = 'false';
            //set radio button to US
            $('#rblAddressType').find('input[value=' + us + ']').prop('checked', true);
            getStatesLocal();
            getCountriesUS();
            switch (isnew) {
                case (true):
                    //set country to 'select'
                    $('#ddlPrimaryCountry').val('X');
                    // clear address 1
                    $('#tbPrimaryAddress1').val('');
                    //clear address2
                    $('#tbPrimaryAddress2').val('');
                    //set city value 
                    $('#tbPrimaryCity').val('');
                    SetAPOCheckBox();
                    //set zip code value
                    $('#tbPrimaryZipCode').val('');
                    //set state value
                    ddlState.prop("disabled", false);
                    $('#ddlPrimaryState').val('X');
                    //set country value
                    $('#ddlPrimaryCountry').val(us);
                    break;
                case (false):
                    // set to saved values
                    //set address1 value
                    $('#tbPrimaryAddress1').val(previousAddress.address1);
                    //set address2 value
                    $('#tbPrimaryAddress2').val(previousAddress.address2);
                    //set city value 
                    $('#tbPrimaryCity').val(previousAddress.city);
                    SetAPOCheckBox();
                    //set zip code value
                    $('#tbPrimaryZipCode').val(previousAddress.zip);
                    //set state value
                    ddlState.prop("disabled", false);
                    if (previousAddress.state == "AP" || previousAddress.state == "AE" || previousAddress.state == "AA") {
                        getForeignStates();
                    }
                    $('#ddlPrimaryState').val(previousAddress.state);
                    //set country value
                    $('#ddlPrimaryCountry').val(previousAddress.country);
                    break;
            }

            // show US address form
            usAddress.show();
            divState.show();
            break;
    }
}
function SetAPOCheckBox() {
    switch (true) {
        case (previousAddress.city == "APO"):
            getForeignStates();
            $('#ddlPrimaryState').val(previousAddress.state);
            $('#rblCity').find('input[value="APO"]').prop('checked', true);
            break;
        case (previousAddress.city == "FPO"):
            getForeignStates();
            $('#ddlPrimaryState').val(previousAddress.state);
            $('#rblCity').find('input[value="FPO"]').prop('checked', true);
            break;
        case (previousAddress.city == "DPO"):
            getForeignStates();
            $('#ddlPrimaryState').val(previousAddress.state);
            $('#rblCity').find('input[value="DPO"]').prop('checked', true);
            break;
        default:
            getStatesLocal();
            $('#ddlPrimaryState').val(previousAddress.state);
            break;
    }
}
function getForeignStates() {
    var statesForeign = {
        "X": "-Select-",
        "AE": "AREA EUROPE (AE)",
        "AP": "AREA PACIFIC (AP)",
        "AA": "AREA ATLANTIC (AA)"
    };
    bindStatesdll(statesForeign);
};
function bindStatesdll(states) {
    ddlState.empty();
    $.each(states, function (key, value) {
        ddlState.
            append($("<option></option>")
            .attr("value", key)
            .text(value));
    });
};
function DisplayEditDialogMessage(title, message, type) {
    var EditDialogMessage = document.getElementById('EditDialogMessage');
    editState = document.getElementById("ddlPrimaryState");
    address.Set();
    var messageType = type;
    if (message != "") {
        getCountryData();
        getStateData();
        if (editState.value == 'NA') {
            RemoveInValidStates();
            DoAddressTypeChangeEdit("FN", false)
        }
        $("#EditDialogMessage").fadeIn(10);
    }
}

function RblAddressTypeChange() {
    country = $('#ddlPrimaryCountry').val();
    rblAddressType = $('#rblAddressType').find('input:checked').val();
    if (rblAddressType == undefined) {
        //load saved address
        if (editState.value == 'NA') {
            isForeign = 'true';
            $('#rblAddressType').find('input[value=' + fn + ']').prop('checked', true);
            AddressTypeChange(fn, false);
        }
        else if (editState.value != 'NA') {
            isForeign = 'false';
            $('#rblAddressType').find('input[value=' + us + ']').prop('checked', true);
            AddressTypeChange(us, false);
        }
        else {
            AddressTypeChange(us, true);
        }
    }
    else {
        // do foreign address
        if (rblAddressType == 'FN') {
            isForeign = 'true';
            if (editState.value == 'NA') {
                //load current foreign address
                AddressTypeChange(fn, false);
            }
            else {
                //load new foreign address
                AddressTypeChange(fn, true);
            }
        }


        // do US address
        if (rblAddressType == 'US') {
            isForeign = 'false';
            if (editState.value != 'NA') {
                //load current us address
                AddressTypeChange(us, false);
            }
            else {
                //load new us address
                AddressTypeChange(us, true);
            }
        }
    }
}

function AddressUpdated() {
    previousAddress.Clear();
    address.Clear();
}
function getStateData() {
    var ddl = $('#ddlPrimaryState');
    var len = (ddl.children('option').length);
    $.each(ddl.children('option').map(function (i, e) {
        localStateList[e.value] = e.innerText;
    }));
};
function getStates() {
    bindStateData(localStateList);
};
function bindStateData(states) {
    ddlState = $('#ddlPrimaryState');
    ddlState.empty();
    $.each(states, function (key, value) {
        ddlState.
            append($("<option></option>")
            .attr("value", key)
            .text(value));
    });
    // removing 'Unknown' and 'Non Applicable' as options in the State dropdown List
    ddlState.find('option[value=UN]').remove();
    ddlState.find('option[value=NA]').remove();
    ddlState.find('option[value=AA]').remove();
    ddlState.find('option[value=AE]').remove();
    ddlState.find('option[value=AP]').remove();
};

function getCountryData() {
    var ddlCountry = $('#ddlPrimaryCountry');
    var len = (ddlCountry.children('option').length);
    $.each(ddlCountry.children('option').map(function (i, e) {
        countryList[e.value] = e.innerText;
    }));
}
function getCountries() {
    bindCountries(countryList);
    $('#ddlPrimaryCountry').find('option[value=US]').remove();
    $('#ddlPrimaryCountry').find('option[value=UM]').remove();
}
function bindCountryData() {
    ddlCountry = $('#ddlPrimaryCountry');
    ddlCountry.empty();
    $.each(countries, function (key, value) {
        ddlCountry.
            append($("<option></option>")
            .attr("value", key)
            .text(value));
    });
}

function GetUSPSAddressbyZip_Success(res) {
    // alert("the value of s is: " + s);
    var error = res.startsWith("error:");
    if (error) {
        GetUSPSAddressbyZip_Failure(res);
    }
    else {
        var addr = JSON.parse(res);
        var addrCity = (addr.City);
        var addrstate = (addr.State);
        $('#ddlPrimaryCountry').val('US');

        // clear radiobutton city selections
        $('#rblCity').find('input:checked').removeAttr('checked');

        // clear city 
        $("#tbPrimaryCity").val("");

        // make sure state is enabled and set to default
        ddlState.prop("disabled", false);
        // get state selections for local States

        if (addrCity == "APO" || addrCity == "DPO" || addrCity == "FPO") {
            // set the appropriate city radio button value based on city returned
            $('#rblCity').find('input[value=' + addrCity + ']').prop('checked', true);

            // get state selections for AA, AE, AP
            getStatesForeign();
            ddlState.val('X');
            Pass('ddlPrimaryState', 'divAddresses', 'divAddressMsg');
        }
        else {
            // get state selections for local States
            getStates();
        }
        ddlState.val('X');
        // set city and state field values
        $("#tbPrimaryCity").val(addrCity);
        $("#ddlPrimaryState").val(addrstate);
        SetPrimaryAddressStCtry();
        if (ShouldPrimaryAddressValidate()) {
            ClearMsgs('divAddresses', 'divAddressMsg');
        };
        if ($('#tbPrimaryAddress1').val() == '') { Fail('tbPrimaryAddress1', 'divAddresses', 'divAddressMsg', 'Address 1 is a requred field'); }
        if ($('#tbPrimaryCity').val() == '') { Fail('tbPrimaryCity', 'divAddresses', 'divAddressMsg', 'City is a required field'); }
        if ($('#ddlPrimaryState').val() == 'X') { Fail('ddlPrimaryState', 'divAddresses', 'divAddressMsg', 'State is a required field'); }
        if ($('#ddlPrimaryCountry').val() == 'X') { Fail('ddlPrimaryCountry', 'divAddresses', 'divAddressMsg', 'Country is a required field'); }
        if ($('#tbPrimaryZipCode').val() == '') { Fail('tbPrimaryZipCode', 'divAddresses', 'divAddressMsg', 'Zip code is a required field'); }
    }
}

function GetUSPSAddressbyZip_Failure(error) {
    $('#tbPrimaryZipCode').val("");
    $('#tbPrimaryCity').val("");
    $('#ddlPrimaryState').val("");
    alert(error.slice(6));
}

function SetPrimaryAddressStCtry() {
    var s = $('#ddlPrimaryState').find('option:selected').val();
    if (s == 'UN' || s == '0') return;
    if (s == 'MH' || s == 'GU' || s == 'MP' || s == 'AS' || s == 'VI' || s == 'FM' || s == 'PW') {
        $('#ddlPrimaryCountry').val('UM');
    }
    else {
        $('#ddlPrimaryCountry').val('US');
    }
    Pass('ddlPrimaryCountry', 'divAddresses', 'divAddressMsg');
}

function SetSecondaryAddressStCtry() {
    var s = $('#ddlSecondaryState').find('option:selected').val();
    if (s == 'UN' || s == '0' || s == 'AA' || s == 'AE' || s == 'AP') return;
    $('#ddlSecondaryCountry').val('US');
}

function DisplayBulkInputDialog() {
    var BulkInput = document.getElementById('OrderNumberBulkInput');
    notAddedCount = 0;

    $("#OrderNumberBulkInput").fadeIn(10);

    $('[id*=lblOrderScanCount]').empty();
    var d = $("#OrderNumberBulkInput");
    $('[id*=tbOrderNumbers]').val("");
    $('[id*=tbOrderNumbers]').focus();
    d.click(function () { $('[id*=tbOrderNumbers]').focus() });
    return false;
}
function CloseBulkInputDialog() {
    document.getElementById('OrderNumberBulkInput').style.display = 'none';
}
function BtnBulkDone() {
    var orderNums = $('[id*=tbOrderNumbers]').val().trim().split('\n');
    if (orderNums.length > 0 && orderNums[0] != "") {
        CloseBulkInputDialog();
        document.getElementById("hdfBulkOrders").value = orderNums;
        //for (i = 0; i < orderNums.length; i++) {
        //}
        $('[id*=tbOrderNumbers]').val('');
        $('[id*=tbSingleReadScan]').focus();
        return true;
    }
    else {
        alert("Please enter order numbers.")
        return false;
    }
}


function CloseEditDialogMessage(isGood) {
    document.getElementById('EditDialogMessage').style.display = 'none';
    if (isGood) { AddressUpdated(); }
}

function PerformFunction(funcName, para1, para2) {
    window[funcName](para1, para2);
}

function IsFunctionValid(name, isValid) {
    switch (name) {
        case 'print':
            if (isValid != "true") {
                alert("No items to print!");
                return false;
            }
            break;
        case 'add':
            var add = $('[id*=tbSingleReadScan]').val()
            if (add == "") {
                alert("Please provide an order number.");
                return false;
            }
            break;
        case 'history':
            if (isValid != "true") {
                alert("No history available!");
                return false;
            }
            break;
        case 'clear':
            if (isValid != "true") {
                alert("The grid is empty!");
                return false;
            }
            break;
        default:
            return true;
    }
}
function SetPageSubmenu(currModule) {
    $("#divSubMenu ul li a").each(function () {
        if ((($(this).text()).replace(/\s+/g, '')).toLowerCase() == currModule) {
            $(this).addClass("active");
        }
    });
    $('[id*=hfPageModule]').val(currModule.toString());
    SetPageContent(currModule);
}
function SetPageDataParams() {
    // Clear Selected Order Array
    ordersToPrint = [];
}
function ClearMsgs(errorCtlCntrId, msgCtlId) {
    sessionStorage.clear();
    $('[id*=' + msgCtlId + ']').html('');
    $('[id*=' + msgCtlId + ']').hide();
    $('[id*=' + errorCtlCntrId + ']' + ' :input[type=submit]').removeAttr('disabled');
    $('.redBorder').removeClass();
}




///////////////////////////////////
// VALIDATION
///////////////////////////////////
function Fail(errorCtlId, errorCtlCntrId, msgCtlId, errorMsg) {
    var tmp = sessionStorage.getItem('error');
    var errs = {};
    sessionStorage.clear();
    if (tmp != undefined && tmp != '')
        errs = JSON.parse(tmp);

    if (errs.hasOwnProperty(errorCtlId)) {
        delete errs[errorCtlId];
    }
    errs[errorCtlId] = errorMsg;
    sessionStorage['error'] = JSON.stringify(errs);

    // Create combined message
    var msg = '';
    for (var k in errs)
        msg += errs[k] + '<br />';

    try {
        $('[id*=' + errorCtlId + ']').addClass('redBorder');
        // New for combo boxes
        //var comboBoxCntrId = GetAltCntrID(errorCtlId);

        //if (comboBoxCntrId != '') {
        //    $('[id*=' + comboBoxCntrId + ']').addClass('redBorder');
        //}
    } catch (err) { /*NOTHING*/ }
    $('[id*=' + msgCtlId + ']').html(msg);
    $('[id*=' + msgCtlId + ']').show();
    if (errorCtlCntrId == '') return false;
    $('[id*=' + errorCtlCntrId + ']' + ' :input[type=submit]').attr('disabled', true);
    return false;
}
function Pass(errorCtlId, errorCtlCntrId, msgCtlId) {
    var tmp = sessionStorage.getItem('error');
    var errs;
    sessionStorage.clear();

    if (tmp == undefined || tmp == '') return true;

    errs = JSON.parse(tmp);

    delete errs[errorCtlId];

    sessionStorage['error'] = JSON.stringify(errs);

    // Create combined message
    var msg = '';
    for (var k in errs)
        msg += errs[k] + '<br />';

    // New for combo boxes
    //var comboBoxCntrId = GetAltCntrID(errorCtlId);

    //if (comboBoxCntrId != '') {
    //    $('[id*=' + comboBoxCntrId + ']').removeClass('redBorder');
    //}

    if (errorCtlId != '')
        $('[id*=' + errorCtlId + ']').removeClass('redBorder');
    $('[id*=' + msgCtlId + ']').html(msg);
    if (msg === '') {
        $('[id*=' + msgCtlId + ']').hide();
        if (errorCtlCntrId == '') return true;
        $('[id*=' + errorCtlCntrId + ']' + ' :input[type=submit]').removeAttr('disabled');
        return true;
    }
    else {
        $('[id*=' + msgCtlId + ']').show();
        if (errorCtlCntrId == '') return false;
        $('[id*=' + errorCtlCntrId + ']' + ' :input[type=submit]').attr('disabled', true);
        return false;
    }
}
function ValidateAddress1(sender, args) {
    if (isForeign != 'true') {
        var c = sender.controltovalidate.toString();
        if (c.indexOf('Primary') == 0) {
            if (!ShouldSecondaryAddressValidate()) return;
        }
        else {
            if (!ShouldPrimaryAddressValidate()) return;
        }
        var regex = /^[a-zA-Z0-9'.\s-\/]{1,40}$/;
        args.IsValid = regex.test(args.Value);
        if (!args.IsValid) {
            $('#hdfIsValid').val("false");
            Fail(c, 'divAddresses', 'divAddressMsg', 'Address 1 contains invalid characters');
        }
        else {
            $('#hdfIsValid').val("true");
            Pass(c, 'divAddresses', 'divAddressMsg');
        }
    }
}
function ValidateAddress2(sender, args) {
    var c = sender.controltovalidate.toString();
    if (c.indexOf('Primary') == 0) {
        if (!ShouldSecondaryAddressValidate()) return;
    }
    else {
        if (!ShouldPrimaryAddressValidate()) return;
    }
    if (args.Value == '') {
        Pass(c, 'divAddresses', 'divAddressMsg');
        args.IsValid = true;
        return;
    }
    var regex = /^[a-zA-Z0-9'.\s-\/]{1,40}$/;
    args.IsValid = regex.test(args.Value);
    if (!args.IsValid)
        Fail(c, 'divAddresses', 'divAddressMsg', 'Address 2 contains invalid characters');
    else
        Pass(c, 'divAddresses', 'divAddressMsg');
}
function ValidateCity(sender, args) {
    if (isForeign != 'true') {
        var c = sender.controltovalidate.toString();
        if (c.indexOf('Primary') == 0) {
            if (!ShouldSecondaryAddressValidate()) return;
        }
        else {
            if (!ShouldPrimaryAddressValidate()) return;
        }
        var regex = /^[a-zA-Z0-9'.\s-\/]{1,40}$/;
        args.IsValid = regex.test(args.Value);
        if (!args.IsValid) {
            $('#hdfIsValid').val("false");
            Fail(c, 'divAddresses', 'divAddressMsg', 'City contains invalid characters');
        }
        else {
            $('#hdfIsValid').val("true");
            Pass(c, 'divAddresses', 'divAddressMsg');
        }
    }
}
function ValidateState(sender, args) {
    if (isForeign != 'true') {
        var c = sender.controltovalidate.toString();
        if (c.indexOf('Primary') == 0) {
            if (!ShouldSecondaryAddressValidate()) return;
        }
        else {
            if (!ShouldPrimaryAddressValidate()) return;
        }
        args.IsValid = args.Value != 'X';
        if (!args.IsValid) {
            $('#hdfIsValid').val("false");
            Fail(c, 'divAddresses', 'divAddressMsg', 'State is a required field');
        }
        else {
            $('#hdfIsValid').val("true");
            Pass(c, 'divAddresses', 'divAddressMsg');
        }
    }
}
function ValidateCountry(sender, args) {
    if (isForeign != 'true') {
        var c = sender.controltovalidate.toString();
        if (c.indexOf('Primary') == 0) {
            if (!ShouldSecondaryAddressValidate()) return;
        }
        else {
            if (!ShouldPrimaryAddressValidate()) return;
        }
        args.IsValid = args.Value != 'X';
        if (!args.IsValid) {
            $('#hdfIsValid').val("false");
            Fail(c, 'divAddresses', 'divAddressMsg', 'Country is a required field');
        }
        else {
            $('#hdfIsValid').val("true");
            Pass(c, 'divAddresses', 'divAddressMsg');
        }
    }
}
function ValidateZip(sender, args) {
    if (isForeign != 'true') {
        var c = sender.controltovalidate.toString();
        if (c.indexOf('Primary') == 0) {
            if (!ShouldSecondaryAddressValidate()) return;
        }
        else {
            if (!ShouldPrimaryAddressValidate()) return;
        }
        var regex = /^\d{5}(\-\d{4})?$/;
        args.IsValid = regex.test(args.Value);
        if (!args.IsValid) {
            $('#hdfIsValid').val("false");
            Fail(c, 'divAddresses', 'divAddressMsg', 'Zip code contains invalid characters');
        }
        else {
            $('#hdfIsValid').val("true");
            Pass(c, 'divAddresses', 'divAddressMsg');
        }
    }
}
function ShouldPrimaryAddressValidate() {
    if (isForeign != 'true') {
        //if ($('#tbPrimaryAddress1').val() == '' &&
        //    $('#tbPrimaryCity').val() == '' &&
        //    $('#ddlPrimaryState').val() == 'X' &&
        //    $('#ddlPrimaryCountry').val() == 'X' &&
        //    $('#tbPrimaryZipCode').val() == '') {
        //    ClearMsgs('divAddresses', 'divAddressMsg');
        //    return false;
        //}
        //else {
        //    if ($('#tbPrimaryAddress1').val() == '') { Fail('tbPrimaryAddress1', 'divAddresses', 'divAddressMsg', 'Address 1 is a requred field'); }
        //    if ($('#tbPrimaryCity').val() == '') { Fail('tbPrimaryCity', 'divAddresses', 'divAddressMsg', 'City is a required field'); }
        //    if ($('#ddlPrimaryState').val() == 'X') { Fail('ddlPrimaryState', 'divAddresses', 'divAddressMsg', 'State is a required field'); }
        //    if ($('#ddlPrimaryCountry').val() == 'X') { Fail('ddlPrimaryCountry', 'divAddresses', 'divAddressMsg', 'Country is a required field'); }
        //    if ($('#tbPrimaryZipCode').val() == '') { Fail('tbPrimaryZipCode', 'divAddresses', 'divAddressMsg', 'Zip code is a required field'); }
        //    return true;
        //}
        return true;
    }
}
function ShouldSecondaryAddressValidate() {
    //if ($('#tbSecondaryAddress1').val() == '' &&
    //    $('#tbSecondaryCity').val() == '' &&
    //    $('#ddlSecondaryState').val() == 'X' &&
    //    $('#ddlSecondaryCountry').val() == 'X' &&
    //    $('#tbSecondaryZipCode').val() == '') {
    //    ClearMsgs('divAddresses', 'divAddressMsg');
    //    return false;
    //}
    //else {
    //    if ($('#tbSecondaryAddress1').val() == '') { Fail('tbSecondaryAddress1', 'divAddresses', 'divAddressMsg', 'Address 1 is a requred field'); }
    //    if ($('#tbSecondaryCity').val() == '') { Fail('tbSecondaryCity', 'divAddresses', 'divAddressMsg', 'City is a required field'); }
    //    if ($('#ddlSecondaryState').val() == 'X') { Fail('ddlSecondaryState', 'divAddresses', 'divAddressMsg', 'State is a required field'); }
    //    if ($('#ddlSecondaryCountry').val() == 'X') { Fail('ddlSecondaryCountry', 'divAddresses', 'divAddressMsg', 'Country is a required field'); }
    //    if ($('#tbSecondaryZipCode').val() == '') { Fail('tbSecondaryZipCode', 'divAddresses', 'divAddressMsg', 'Zip code is a required field'); }
    //}
    return true;
}

function IsValidAddress() {
    var isValid = true;

    if ($('#tbPrimaryAddress1').val() == '') { Fail('tbPrimaryAddress1', 'divAddresses', 'divAddressMsg', 'Address 1 is a requred field'); isValid = false; }
    if ($('#ddlPrimaryCountry').val() == 'X') { Fail('ddlPrimaryCountry', 'divAddresses', 'divAddressMsg', 'Country is a required field'); isValid = false; }

    if (isForeign != 'true') {
        if ($('#tbPrimaryCity').val() == '') { Fail('tbPrimaryCity', 'divAddresses', 'divAddressMsg', 'City is a required field'); isValid = false; }
        if ($('#ddlPrimaryState').val() == 'X') { Fail('ddlPrimaryState', 'divAddresses', 'divAddressMsg', 'State is a required field'); isValid = false; }
        if ($('#tbPrimaryZipCode').val() == '') { Fail('tbPrimaryZipCode', 'divAddresses', 'divAddressMsg', 'Zip code is a required field'); isValid = false; }

        if (isValid) {
            $('#hdfIsValid').val("true");
            return true;
        }
        else {
            $('#hdfIsValid').val("false");
            return false;
        }
    }
}

function Hide(d) {
    $('#' + d).css('visibility', 'hidden');//style.visibility = 'hidden';
    $('#divAddressToolTip').html("");
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
var savedAddress = {
    address1: "",
    address2: "",
    city: "",
    state: "",
    zip: "",
    country: "",
    Clear: function () {
        this.address1 = "";
        this.address2 = "";
        this.city = "";
        this.state = "";
        this.zip = "";
        this.country = "";
    },
    Set: function (result, expireDays) {
        this.address1 = (result.Address1);
        this.address2 = (result.Address2);
        this.city = (result.City);
        this.state = (result.State);
        this.country = (result.Country);
        this.zip = (result.ZipCode);

        $('#tbPrimaryAddress1').val(this.address1);
        $('#tbPrimaryAddress2').val(this.address2);
        $('#ddlPrimaryCountry').val(this.country);
        $('#ddlPrimaryState').val(this.state);
        $('#tbPrimaryCity').val(this.city);
        $('#tbPrimaryZipCode').val(this.zip);
        address.Set(result);
        ValidateAddress(result.DateVerified, expireDays);
    }
};
function ValidateAddress(dateVerified, expireDays) {
    var addressHeader = document.getElementById('addressHeader');

    //If the standardized address is selected, this address is considered valid for 90 days. 
    //if a non-standardized address is selected, this address is considered valid for 30 days. 
    var status = "Not Verified";
    var verifyExpiration = "";
    var isExpired = false;

    var re = /-?\d+/;
    var m = re.exec(dateVerified);
    var verifiedDate = new Date(parseInt(m[0]));

    if (verifiedDate != "1/1/0001" && verifiedDate != null && verifiedDate != "" && expireDays != "0") {
        var expDate = new Date(verifiedDate); // date address was last verified

        if (expireDays == "30") {
            expDate.setDate(expDate.getDate() + 30);  // set to expire in 30 days
        }
        else if (expireDays == "90") {
            expDate.setDate(expDate.getDate() + 90);  // set to expire in 90 days
        }

        verifyExpiration = expDate.toLocaleDateString();
        var currDate = new Date();

        if (expDate < currDate) {
            isExpired = true;
        }
        if (isExpired) {
            status = "<span style='color:#FF4500'>Address validation expired on " + expDate.toLocaleDateString() + ". <br /> Please validate this address.</span>";   // verification has expxired
        }
        else if (!isExpired) {
            status = "<span style='color:#228B22'>Address validation is current until " + expDate.toLocaleDateString() + ".</span>";   // verification has not expired;
        }
    }
    else {
        status = "<span style='color:#DC143C'>Address has not been validated. Please validate this address.</span>";   // verification has never been done.
    }
    //addressHeader.innerHTML = status;
}

function USPSVerifyAddressResult(result) {
    if (result != "error") {

        //load Verified Address
        var addrAddress1 = (result.Address1);
        var addrAddress2 = (result.Address2);
        var addrCity = (result.City);
        var addrState = (result.State);
        var addrZipCode = (result.ZipCode);
        $('#txtAddress1Verified').val(addrAddress1);
        $('#txtAddress2Verified').val(addrAddress2);
        $('#txtCityVerified').val(addrCity);
        $('#txtStateVerified').val(addrState);
        $('#txtZipCodeVerified').val(addrZipCode);
        $('#txtCountryVerified').val("US");
        document.getElementById('divAddressEntered').style.display = 'none';
        document.getElementById('divAddressSubmit').style.display = 'none';
    }
    else {
        document.getElementById('divAddressVerified').style.display = 'none';
        document.getElementById('divAddressEntered').style.display = 'none';
        document.getElementById('divAddressMessage').innerHTML = "<p style='color:red'>The United States Postal Service does not recognize this address.</p>";
    }
    //load Entered Mailing Address
    $('#txtAddress1').val(address.address1);
    $('#txtAddress2').val(address.address2);
    $('#txtCity').val(address.city);
    $('#txtState').val(address.state);
    $('#txtZipCode').val(address.zip);
    $('#txtCountry').val(address.country);

    DisplayAddressVerificationDialog();
}
function DisplayAddressVerificationDialog() {
    var AddressVerificationDialog = document.getElementById('AddressVerificationDialog');
    $("#AddressVerificationDialog").fadeIn(10);
}
function SetSavedAddress(result, expireDays) {
    //hide save and cancel buttons
    $('.btnShow').removeClass('srtsButton btnShow').addClass('btnHide');
    savedAddress.Set(result, expireDays);

}

function CloseAddressEditDialogMessage() {
    document.getElementById('EditDialogMessage').style.display = 'none';
}
function CloseAddressVerificationDialog() {
    document.getElementById('AddressVerificationDialog').style.display = 'none';
}
function GetToolTip(t, o) {
    //find the gridview
    var gv = document.getElementById('gvOrderAddresses');
    var i, CellValue, Row;
    i = parseInt(t) + 1;
    var table = document.getElementById('gvOrderAddresses');
    Row = table.rows[i].cells[3];

    var lblID = "lblIsValid" + o;
    var lbl = document.getElementById(Row.children[0].children[lblID].id);
    $(lbl).css('visibility', 'visible');
}



