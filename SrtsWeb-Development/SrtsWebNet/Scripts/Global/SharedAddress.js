
///////////////////////////////////
// VALIDATION
///////////////////////////////////
function ValidateAddress1(sender, args) {
    if (isForeign != 'true') {
        var c = sender.controltovalidate.toString();
        var s = sender.getAttribute('id');

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

//function ShouldPrimaryAddressValidate() {
//    if ($('#tbPrimaryAddress1').val() == '' &&
//        $('#tbPrimaryCity').val() == '' &&
//        $('#ddlPrimaryState').val() == 'X' &&
//        $('#ddlPrimaryCountry').val() == 'X' &&
//        $('#tbPrimaryZipCode').val() == '') {
//        ClearMsgs('divAddresses', 'divAddressMsg');
//        return false;
//    }
//    else {
//        if (isForeign != 'true') {
//            if ($('#tbPrimaryAddress1').val() == '') { Fail('tbPrimaryAddress1', 'divAddresses', 'divAddressMsg', 'Address 1 is a requred field'); }
//            if ($('#tbPrimaryCity').val() == '') { Fail('tbPrimaryCity', 'divAddresses', 'divAddressMsg', 'City is a required field'); }
//            if ($('#ddlPrimaryState').val() == 'X') { Fail('ddlPrimaryState', 'divAddresses', 'divAddressMsg', 'State is a required field'); }
//            if ($('#ddlPrimaryCountry').val() == 'X') { Fail('ddlPrimaryCountry', 'divAddresses', 'divAddressMsg', 'Country is a required field'); }
//            if ($('#tbPrimaryZipCode').val() == '') { Fail('tbPrimaryZipCode', 'divAddresses', 'divAddressMsg', 'Zip code is a required field'); }
//        }
//    }
//    return true;
//}

function ShouldPrimaryAddressValidate() {
    if (isForeign != 'true') {
        if ($('#tbPrimaryAddress1').val() == '' &&
            $('#tbPrimaryCity').val() == '' &&
            $('#ddlPrimaryState').val() == 'X' &&
            $('#ddlPrimaryCountry').val() == 'X' &&
            $('#tbPrimaryZipCode').val() == '') {
            ClearMsgs('divAddresses', 'divAddressMsg');
            return false;
        }
        else {
            if ($('#tbPrimaryAddress1').val() == '') { Fail('tbPrimaryAddress1', 'divAddresses', 'divAddressMsg', 'Address 1 is a requred field'); }
            if ($('#tbPrimaryCity').val() == '') { Fail('tbPrimaryCity', 'divAddresses', 'divAddressMsg', 'City is a required field'); }
            if ($('#ddlPrimaryState').val() == 'X') { Fail('ddlPrimaryState', 'divAddresses', 'divAddressMsg', 'State is a required field'); }
            if ($('#ddlPrimaryCountry').val() == 'X') { Fail('ddlPrimaryCountry', 'divAddresses', 'divAddressMsg', 'Country is a required field'); }
            if ($('#tbPrimaryZipCode').val() == '') { Fail('tbPrimaryZipCode', 'divAddresses', 'divAddressMsg', 'Zip code is a required field'); }
            return true;
        }
    }
}

function ShouldSecondaryAddressValidate() {
    if ($('#tbSecondaryAddress1').val() == '' &&
        $('#tbSecondaryCity').val() == '' &&
        $('#ddlSecondaryState').val() == 'X' &&
        $('#ddlSecondaryCountry').val() == 'X' &&
        $('#tbSecondaryZipCode').val() == '') {
        ClearMsgs('divAddresses', 'divAddressMsg');
        return false;
    }
    else {
        if ($('#tbSecondaryAddress1').val() == '') { Fail('tbSecondaryAddress1', 'divAddresses', 'divAddressMsg', 'Address 1 is a requred field'); }
        if ($('#tbSecondaryCity').val() == '') { Fail('tbSecondaryCity', 'divAddresses', 'divAddressMsg', 'City is a required field'); }
        if ($('#ddlSecondaryState').val() == 'X') { Fail('ddlSecondaryState', 'divAddresses', 'divAddressMsg', 'State is a required field'); }
        if ($('#ddlSecondaryCountry').val() == 'X') { Fail('ddlSecondaryCountry', 'divAddresses', 'divAddressMsg', 'Country is a required field'); }
        if ($('#tbSecondaryZipCode').val() == '') { Fail('tbSecondaryZipCode', 'divAddresses', 'divAddressMsg', 'Zip code is a required field'); }
    }
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
    }

    if (isValid) {
        $('#hdfIsValid').val("true");
        return true;
    }
    else {
        $('#hdfIsValid').val("false");
        return false;
    }
}

///////////////////////////////////
// EVENTS
///////////////////////////////////
var country = "";
var localStateList = {};
var countryList = {};
var fn = 'FN';
var us = 'US';
var isForeign = '';
var addr1 = '';
var addr2 = '';
var c = '';
var s = '';
var city = '';
var tState = '';
var state = '';
var zip = '';

//var previousAddress = {
//    address1: "",
//    address2: "",
//    city: "",
//    state: "",
//    zip: "",
//    country: "",
//    Clear: function () {
//        this.address1 = "";
//        this.address2 = "";
//        this.city = "";
//        this.state = "";
//        this.zip = "";
//        this.country = "";
//    },
//    Set: function () {
//        this.address1 = address.address1;
//        this.address2 = address.address2;
//        this.city = address.city;
//        this.zip = address.zip;
//        this.country = address.country;
//        switch (address.city) {
//            case "APO":
//                this.state = "AP"
//                break;
//            case "FPO":
//                this.state = "AE"
//                break
//            case "DPO":
//                this.state = "AA"
//                break
//            default:
//                this.state = address.state;
//                break;
//        }
//        $('#tbPrimaryAddress1').val(this.address1);
//        $('#tbPrimaryAddress2').val(this.address2);
//        $('#ddlPrimaryCountry').val(this.country);
//        $('#ddlPrimaryState').val(this.state);
//        $('#tbPrimaryCity').val(this.city);
//        $('#tbPrimaryZipCode').val(this.zip);

//    }
//};

//var address = {
//    address1: "",
//    address2: "",
//    city: "",
//    state: "",
//    zip: "",
//    country: "",
//    Clear: function () {
//        this.address1 = "";
//        this.address2 = "";
//        this.city = "";
//        this.state = "";
//        this.zip = "";
//        this.country = "";
//    },
//    Set: function () {
//        this.address1 = $('#tbPrimaryAddress1').val();
//        this.address2 = $('#tbPrimaryAddress2').val();
//        this.city = $('#tbPrimaryCity').val();
//        this.state = $('#ddlPrimaryState').val();
//        this.country = $('#ddlPrimaryCountry').val();
//        this.zip = $('#tbPrimaryZipCode').val();
//        previousAddress.Set();
//    },
//    Get: function () {
//        $('#tbPrimaryAddress1').val(this.address1);
//        $('#tbPrimaryAddress2').val(this.address2);
//        $('#ddlPrimaryCountry').val(this.country);
//        $('#ddlPrimaryState').val(this.state);
//        $('#tbPrimaryCity').val(this.city);
//        if (this.state == "NA" || this.country != "US" && this.country != "UM") {
//            SetAddressType("FN");
//        }
//        else {
//            SetAddressType("US");
//        }
//    }
//};

//var savedAddress = {
//    address1: "",
//    address2: "",
//    city: "",
//    state: "",
//    zip: "",
//    country: "",
//    Clear: function () {
//        this.address1 = "";
//        this.address2 = "";
//        this.city = "";
//        this.state = "";
//        this.zip = "";
//        this.country = "";
//    },
//    Set: function (result) {
//        this.address1 = (result.Address1);
//        this.address2 = (result.Address2);
//        this.city = (result.City);
//        this.state = (result.State);
//        this.country = (result.Country);
//        this.zip = (result.ZipCode);

//        $('#tbPrimaryAddress1').val(this.address1);
//        $('#tbPrimaryAddress2').val(this.address2);
//        $('#ddlPrimaryCountry').val(this.country);
//        $('#ddlPrimaryState').val(this.state);
//        $('#tbPrimaryCity').val(this.city);
//        $('#tbPrimaryZipCode').val(this.zip);
//        address.Set(result);

//    }
//};


$(document).ready(function () {
    addr1 = $('#tbPrimaryAddress1').val();
    addr2 = $('#tbPrimaryAddress2').val();
    city = $('#tbPrimaryCity').val();
    tState = $('#hdfState').val();
    state = tState == undefined || tState == '' ? 'X' : tState;
    c = $('#ddlPrimaryCountry').val();
    s = $('#ddlPrimaryState').val();
    zip = $('#tbPrimaryZipCode').val();
    ddlCountry = $('#ddlPrimaryCountry');
    // removing 'Unknown' and 'Non Applicable' as options in the State dropdown List
    ddlState = $('#ddlPrimaryState');
    ddlState.find('option[value=UN]').remove();
    ddlState.find('option[value=NA]').remove();
    ddlState.find('option[value=AA]').remove();
    ddlState.find('option[value=AE]').remove();
    ddlState.find('option[value=AP]').remove();
    getLocalStateData();
    getForeignCountryData();

    //get saved country
    if (c == 'X' || c == 'US' || c == 'UM') {
        if (c == 'UM') {
            $('#ddlPrimaryCountry').val('UM');
        }
        if (c == 'X' || c == 'US') {
            $('#ddlPrimaryCountry').val('US');
        }

        //get saved city
        $('#tbPrimaryCity').val(city);

        //set apo, fpo and dpo checkbox value
        SetPOCheckBox();

        //get saved zip
        $('#tbPrimaryZipCode').val(zip);

        //get saved state
        ddlState.prop("disabled", false);
        if (s != 'X') {
            $('#ddlPrimaryState').val(s);
        }
        else {
            $('#ddlPrimaryState').val('X');
        }
    }
    else {
        $('#ddlPrimaryCountry').val(c);
        ddlState.prop("disabled", true);
    }
    if (typeof (address) !== "undefined") {
        address.Set();
    }
    DoRblAddressTypeChange();


    $('#tbPrimaryCity').on("focus", function () {
        DoTbCityClick();
    });

    $('#divFnAddress').hide();


});

function SetPOCheckBox() {
    switch (true) {
        case (city == "APO"):
            getStatesForeign();
            $('#ddlPrimaryState').val(s);
            $('#rblCity').find('input[value="APO"]').prop('checked', true);
            break;
        case (city == "FPO"):
            getStatesForeign();
            $('#ddlPrimaryState').val(s);
            $('#rblCity').find('input[value="FPO"]').prop('checked', true);
            break;
        case (city == "DPO"):
            getStatesForeign();
            $('#ddlPrimaryState').val(s);
            $('#rblCity').find('input[value="DPO"]').prop('checked', true);
            break;
        default:
            getStatesLocal();
            $('#ddlPrimaryState').val(s);
            break;
    }
}

function SetStatetoNA() {
    // set state to not applicable
    ddlState.append($("<option></option>").attr("value", "NA").text("Not Applicable"));
    ddlState.val("NA");
    ddlState.prop("disabled", true);
}

function DoZipLookup() {
    var zip = $("#tbPrimaryZipCode").val();
    var isValidZip = /^\b\d{5}(-\d{4})?\b$/.test(zip);
    if (isValidZip) {
        GetUSPSAddressbyZip(zip);
    }
}

function GetUSPSAddressbyZip(zip) {
    PageMethods.GetUSPSAddressbyZip(zip, GetUSPSAddressbyZip_Success);
}

function GetUSPSAddressbyZip_Success(res) {
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
        //ddlState.val('X'); //Aldela: commented this out

        if (addrCity == "APO" || addrCity == "DPO" || addrCity == "FPO") {
            // set the appropriate city radio button value based on city returned
            $('#rblCity').find('input[value=' + addrCity + ']').prop('checked', true);

            // get state selections for AA, AE, AP
            getStatesForeign();
            //ddlState.val('X');//Aldela: commented this out
            Pass('ddlPrimaryState', 'divAddresses', 'divAddressMsg');
        }
        else {
            // get state selections for local States
            getStatesLocal();
        }

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

function DoDdlCountryChange() {
    country = $('#ddlPrimaryCountry').val();
    $('#ddlPrimaryCountry').val(country);
};

function DoTbCityBlur() {

    //var c = $('#rblCity').find('input:checked').val();
    //if (c == undefined || c == "") return;
    // clear radiobutton city selections

    $('#rblCity').find('input:checked').removeAttr('checked');
    var selectedState = ddlState.val();
    if ($('#tbPrimaryCity').val() == "APO" || $('#tbPrimaryCity').val() == "DPO" || $('#tbPrimaryCity').val() == "FPO")
    {
        // set the appropriate city radio button value based on city returned
        $('#rblCity').find('input[value=' + $('#tbPrimaryCity').val() + ']').prop('checked', true);

        // get state selections for AA, AE, AP
        getStatesForeign();
//        ddlState.val('X');
        //Pass('ddlPrimaryState', 'divAddresses', 'divAddressMsg');
    }
    else {
        // get state selections for local States
        getStatesLocal();
    }
    ddlState.val(selectedState);
    var stateValue = $('#ddlPrimaryState').find('option:selected').val();
    if (stateValue == undefined)
        ddlState.val('X');
    // show local states
    ddlState.show();

    // set state to -select-
   // ddlState.val('X'); //Aldela: 12/27/2017 Commented this out

    // set country to 'US'
    country = 'US';
    $('#ddlPrimaryCountry').val(country);


    // make sure state is enabled
    ddlState.prop("disabled", false);


};

function DoTbCityClick() {

    //var c = $('#rblCity').find('input:checked').val();
    //if (c == undefined || c == "") return;
    //// clear city text box value
    //$('#tbPrimaryCity').val("");

    //// clear radiobutton city selections
    //$('#rblCity').find('input').removeAttr('checked');

    //// set country to 'US'
    //country = 'US';
    //$('#ddlPrimaryCountry').val(country);

    //// get states for US
    //getStatesLocal();

    //var addresstype = $('#rblAddressType').find('input:checked').val();
    //if (addresstype == 'US')
    //// make sure state is enabled
    //ddlState.prop("disabled", false);
};

function DoRblCityChange() {
    // get value of radiobutton city
    var rblCity = $('#rblCity').find('input:checked').val();

    if ($('#tbPrimaryCity').val() == rblCity) return;
    //set city textbox to radiobutton selected option value
    $('#tbPrimaryCity').val(rblCity);

    // Remove the city error
    Pass('tbPrimaryCity', 'divAddresses', 'divAddressMsg');

    // make sure state is enabled
    ddlState.prop("disabled", false);

    // get state selections for aa, ap, ae
    getStatesForeign();
    ddlState.val('AE');
    Pass('ddlPrimaryState', 'divAddresses', 'divAddressMsg');

    // set country to 'US'
    country = 'US';
    $('#ddlPrimaryCountry').val(country);
};

function DoRblAddressTypeChange() {
    country = $('#ddlPrimaryCountry').val();
    rblAddressType = $('#rblAddressType').find('input:checked').val();
    if (rblAddressType == undefined) {
        //load saved address
        if (tState == 'NA') {
            isForeign = 'true';
            $('#rblAddressType').find('input[value=' + fn + ']').prop('checked', true);
            DoAddressTypeChange(fn, false);
        }
        else if (tState != 'NA') {
            isForeign = 'false';
            $('#rblAddressType').find('input[value=' + us + ']').prop('checked', true);
            DoAddressTypeChange(us, false);
        }
        else {
            DoAddressTypeChange(us, true);
        }
    }
    else {
        // do foreign address
        if (rblAddressType == 'FN') {
            isForeign = 'true';
            if (address.state == "NA" || address.country != "US" && address.country != "UM") {
                //if (previousAddress.state == 'NA') {
                //load current foreign address
                DoAddressTypeChange(fn, false);
            }
            else {
                //load new foreign address
                DoAddressTypeChange(fn, true);
            }
        }


        // do US address
        if (rblAddressType == 'US') {
            isForeign = 'false';
            if (address.state != "NA" && address.country == "US" || address.country == "UM") {
                //if (address.state != 'NA') {
                //load current us address
                DoAddressTypeChange(us, false);
            }
            else {
                //load new us address
                DoAddressTypeChange(us, true);
            }
        }
    }
}

function DoAddressTypeChange(t, isnew) {
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
            $('#bSpecialChars').show();
            getCountriesForeign();
            switch (isnew) {
                case (true):
                    //set country to 'select'
                    $('#ddlPrimaryCountry').val('X');
                    // clear address 1
                    $('#tbPrimaryAddress1').val('');
                    //clear address2
                    $('#tbPrimaryAddress2').val('');
                    $('#tbPrimaryAddress1').focus();
                    break;
                case (false):
                    $('#ddlPrimaryCountry').val(previousAddress.country);
                    $('#tbPrimaryAddress1').val(previousAddress.address1);
                    $('#tbPrimaryAddress2').val(previousAddress.address2);
                    break;
            }

            // clear and hide city, state and zip
            ddlState.show();
            SetStatetoNA();
            // clear city, and zip values
            $('#tbPrimaryCity').val("");
            $('#tbPrimaryZipCode').val("");
            $('#rblCity').find('input').removeAttr('checked');

            // show Foreign address form
            usAddress.hide();
            uicAddress.hide();
            divState.hide();

            break;
        case (us):
            isForeign = 'false';
            //set radio button to US
            $('#rblAddressType').find('input[value=' + us + ']').prop('checked', true);
            $('#bSpecialChars').hide();
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
                    SetPOCheckBox();
                    //set zip code value
                    $('#tbPrimaryZipCode').val('');
                    //set state value
                    ddlState.prop("disabled", false);
                    $('#ddlPrimaryState').val('X');
                    //set country value
                    $('#ddlPrimaryCountry').val(us);
                    $('#tbPrimaryAddress1').focus();
                    break;
                case (false):
                    if (typeof (previousAddress !== "undefined")) {
                        // set to saved values
                        //set address1 value
                        $('#tbPrimaryAddress1').val(previousAddress.address1);
                        //set address2 value
                        $('#tbPrimaryAddress2').val(previousAddress.address2);
                        //set city value 
                        $('#tbPrimaryCity').val(previousAddress.city);
                        SetPOCheckBox();
                        //set zip code value
                        $('#tbPrimaryZipCode').val(previousAddress.zip);
                        //set state value
                        ddlState.prop("disabled", false);
                        if (s != 'X') {
                            SetPOCheckBox();
                            $('#ddlPrimaryState').val(previousAddress.state);
                        }
                        else {
                            $('#ddlPrimaryState').val('X');
                        }
                        //set country value
                        $('#ddlPrimaryCountry').val(previousAddress.country);
                        break;
                    }
            }

            // show US address form
            usAddress.show();
            uicAddress.show();
            divState.show();

            break;
    }
}

function getLocalStateData() {
    var len = (ddlState.children('option').length);
    $.each(ddlState.children('option').map(function (i, e) {
        localStateList[e.value] = e.innerText;
    }));
};

function getForeignCountryData() {
    var len = (ddlCountry.children('option').length);
    $.each(ddlCountry.children('option').map(function (i, e) {
        countryList[e.value] = e.innerText;
    }));
};

function getStatesLocal() {
    bindStates(localStateList);
};

function getStatesForeign() {
    var statesForeign = {
        "X": "-Select-",
        "AE": "AREA EUROPE (AE)",
        "AP": "AREA PACIFIC (AP)",
        "AA": "AREA ATLANTIC (AA)"
    };
    bindStates(statesForeign);
};

function getCountriesUS() {
    var countryUS = {
        "US": "UNITED STATES",
        "UM": "UNITED STATES MINOR OUTLYING ISLANDS"
    };
    bindCountries(countryUS);
}

function getCountriesForeign() {
    bindCountries(countryList);
    $('#ddlPrimaryCountry').find('option[value=US]').remove();
}

function bindStates(states) {
    ddlState = $('#ddlPrimaryState');
    ddlState.empty();
    $.each(states, function (key, value) {
        ddlState.
            append($("<option></option>")
            .attr("value", key)
            .text(value));
    });
};

function bindCountries(countries) {
    ddlCountry = $('#ddlPrimaryCountry');
    ddlCountry.empty();
    $.each(countries, function (key, value) {
        ddlCountry.
            append($("<option></option>")
            .attr("value", key)
            .text(value));
    });
};