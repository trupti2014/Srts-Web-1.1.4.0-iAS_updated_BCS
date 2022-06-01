/// <reference path="../Global/jquery-1.11.1.min.js" />
/// <reference path="PersonDetailVal.js" />
/// <reference path="../Global/jquery-ui.min.js" />
/// <reference path="../Global/SharedAddress.js" />

$(document).ready(function () {
    /**
 * Re-assigns a couple of the ASP.NET validation JS functions to
 * provide a more flexible approach
 */
    function UpgradeASPNETValidation() {
        // Hi-jack the ASP.NET error display only if required
        if (typeof (Page_ClientValidate) != "undefined") {
            ValidatorUpdateDisplay = NicerValidatorUpdateDisplay;
            AspPage_ClientValidate = Page_ClientValidate;
            Page_ClientValidate = NicerPage_ClientValidate;
        }
    }

    /**
     * Extends the classic ASP.NET validation to add a class to the parent span when invalid
     */
    function NicerValidatorUpdateDisplay(val) {
        if (val.isvalid) {
            // do custom removing
            $(val).fadeOut('slow');
        } else {
            // do custom show
            $(val).fadeIn('slow');
        }
    }

    /**
     * Extends classic ASP.NET validation to include parent element styling
     */
    function NicerPage_ClientValidate(validationGroup) {
        var valid = AspPage_ClientValidate(validationGroup);

        if (!valid) {
            // do custom styling etc
            // I added a background colour to the parent object
            $(this).parent().addClass('invalidField');
        }
    }




    $('.btnShow').removeClass('srtsButton btnShow').addClass('btnHide');


    $("#tbPrimaryAddress1").on("input", function () {
        $('.btnHide').removeClass('btnHide').addClass('srtsButton btnShow');

    });
    specChar = $('#bSpecialChars');
    specChar.hide();
});

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
    addressHeader.innerHTML = status;
}

function Cancel_Save(item) {
    switch (item) {
        case "address":
            $('.btnShow').removeClass('btnShow').addClass('btnHide');
            break;
    }
}

function pageLoad() {
    // hide US Address fields after adding FN address
    rbl = $('#rblAddressType').find('input:checked').val();
    uic = $('#divUIC');
    st = $('#divState');
    usAddr = $('#divUsAddress');
    specChar = $('#bSpecialChars');
    specChar.hide();
    if (rbl == 'FN') {
        st.hide();
        uic.hide();
        usAddr.hide();
        specChar.show();
    }
    $('#cbProvider').on('change', SetIndTypeButtonState);
    $('#cbTechnician').on('change', SetIndTypeButtonState);
    $('#cbAdministrator').on('change', SetIndTypeButtonState);

    $('#ddlPrimaryState').on('change', SetPrimaryAddressStCtry);
    $('#ddlSecondaryState').on('change', SetSecondaryAddressStCtry);

    ClearMsgs('divIdNumber', 'divIdNumMsg');
    ClearMsgs('divAddresses', 'divAddressMsg');
    ClearMsgs('divPhoneNumbers', 'divPhoneNumMsg');
    ClearMsgs('divEmailAddresses', 'divEmailMsg');
}




var previousAddress = {
    uic: "",
    address1: "",
    address2: "",
    city: "",
    state: "",
    zip: "",
    country: "",
    Clear: function () {
        this.uic = "";
        this.address1 = "";
        this.address2 = "";
        this.city = "";
        this.state = "";
        this.zip = "";
        this.country = "";
    },
    Set: function () {
        this.uic = address.uic;
        this.address1 = address.address1;
        this.address2 = address.address2;
        this.city = address.city;
        this.zip = address.zip;
        this.country = address.country;
        //switch (address.city) {
        //    case "APO":
        //        this.state = "AP"
        //        break;
        //    case "FPO":
        //        this.state = "AE"
        //        break
        //    case "DPO":
        //        this.state = "AA"
        //        break
        //    default:
        //        this.state = address.state;
        //        break;
        //}
        this.state = address.state;
        $('#tbPrimaryUIC').val(this.uic);
        $('#tbPrimaryAddress1').val(this.address1);
        $('#tbPrimaryAddress2').val(this.address2);
        $('#ddlPrimaryCountry').val(this.country);
        $('#ddlPrimaryState').val(this.state);
        $('#tbPrimaryCity').val(this.city);
        $('#tbPrimaryZipCode').val(this.zip);

    }
};

var address = {
    uic: "",
    address1: "",
    address2: "",
    city: "",
    state: "",
    zip: "",
    country: "",
    Clear: function () {
        this.uic = "";
        this.address1 = "";
        this.address2 = "";
        this.city = "";
        this.state = "";
        this.zip = "";
        this.country = "";
    },
    Set: function () {
        this.uic = $('#tbPrimaryUIC').val();
        this.address1 = $('#tbPrimaryAddress1').val();
        this.address2 = $('#tbPrimaryAddress2').val();
        this.city = $('#tbPrimaryCity').val();
        this.state = $('#ddlPrimaryState').val();
        this.country = $('#ddlPrimaryCountry').val();
        this.zip = $('#tbPrimaryZipCode').val();
        // if (previousAddress.address1 != "") {
        previousAddress.Set();
        // }
    },
    Get: function () {
        $('#tbPrimaryUIC').val(this.uic);
        $('#tbPrimaryAddress1').val(this.address1);
        $('#tbPrimaryAddress2').val(this.address2);
        $('#ddlPrimaryCountry').val(this.country);
        $('#ddlPrimaryState').val(this.state);
        $('#tbPrimaryCity').val(this.city);
        if (this.state == "NA" || this.country != "US" && this.country != "UM") {
            SetAddressType("FN");
        }
        else {
            SetAddressType("US");
        }
    }
};


function SetAddressType(addressType) {
    switch (addressType) {
        case "US":
            $('#rblAddressType').find('input[value="US"]').prop('checked', true);
            break;
        case "FN":
            $('#rblAddressType').find('input[value="FN"]').prop('checked', true);
            $("#<%=btnAddressVerify.ClientID%>").val('Save');
            break;
    }
    DoRblAddressTypeChange();
}

var savedAddress = {
    uic: "",
    address1: "",
    address2: "",
    city: "",
    state: "",
    zip: "",
    country: "",
    Clear: function () {
        this.uic = "";
        this.address1 = "";
        this.address2 = "";
        this.city = "";
        this.state = "";
        this.zip = "";
        this.country = "";
    },
    Set: function (result, expireDays) {
        this.uic = (result.uic);
        this.address1 = (result.Address1);
        this.address2 = (result.Address2);
        this.city = (result.City);
        this.state = (result.State);
        this.country = (result.Country);
        this.zip = (result.ZipCode);

        $('#tbPrimaryUIC').val(this.uic);
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

function ClearPreviousAddress() {
    //   previousAddress.Clear();
}
function SetAddress() {
    address.Set();
}

function SetSavedAddress(result, expireDays) {
    //hide save and cancel buttons
    $('.btnShow').removeClass('srtsButton btnShow').addClass('btnHide');
    savedAddress.Set(result, expireDays);

}

function CloseAddressVerificationDialog() {
    document.getElementById('AddressVerificationDialog').style.display = 'none';
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

function DoRefreshDialog() {
    var dialogOpts = {
        autoOpen: false,
        modal: true,
        width: 725,
        height: 660,
        title: 'DMDC/DEERs Data Refresh Comparison',
        dialogClass: 'generic',
        close: function () {
            $(this).dialog('destroy');
            $(this).hide();
        }
    };
    var d2 = $('#divRefreshDialog').dialog(dialogOpts);
    d2.parent().appendTo($('form:first'));
    d2.dialog('open');
}

function SetIndTypeButtonState() {
    var good = ValidateIndividualType();

    if (good) {
        $('[id$=btnUpdateIndTypes]').removeAttr('disabled');
        $('#IndTypeErrorMsg').html('');
    }
    else {
        $('[id$=btnUpdateIndTypes]').attr('disabled', true);
        $('#IndTypeErrorMsg').html('Select at least one individual type');
    }
}




function showCalendar() {
    $("#calImage1").trigger("click");
}

