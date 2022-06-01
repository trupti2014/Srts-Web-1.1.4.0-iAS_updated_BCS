/// <reference path="../Global/jquery-1.11.1.min.js"/>
/// <reference path="../Global/jquery-ui.min.js"/>
/// <reference path="OrderManagementVal.js" />
/// <reference path="../Global/PassFailConfirm.js" />
var hdfIsReprint;
var isReprint = false;
$(document).ready(function () {
    $('#btnPrevious').hide();

    $('#ddlShipTo').on('change', DoAddressValidation());

    $("#id_truebtn").click(function (e) {
        return true;
    });

    $('#id_falsebtn').click(function (e) {
        document.getElementById('id_confrmdiv').style.display = "none";
        e.preventDefault();
        return false;
    });

    $("#ProdLab *").attr("disabled", "disabled").off('click');//Aldela FS 382: Disable lab selection

});

$(document).on('change',
    '[id$=rblRejectRedirect]',
    function () {
        DoRejectRedirect($('#rblRejectRedirect').find('input:checked').val());
    }

);

//*********************************************************************
// Order Screen Wizard
//*********************************************************************
function ShowOrderDetail(s) {
    $('#hdfShowOrderStep').val("1");
    $('#ShowOrderDetail').show();
    $('#ShowOrderDistribution').hide();
    $('.btnPrevious').hide();
    $('.btnNext').show();
    $('#divAddNewOrderButtons').hide();
    if (s == "1")
    {
        HideBtnNextTop();
    }
    return false;
}

function ShowOrderDistribution() {
    $('#hdfShowOrderStep').val("2");
    $('#ShowOrderDetail').hide();
    $('#ShowOrderDistribution').show();
    $('.btnNext').hide();
    $('.btnPrevious').show();
    $('#divAddNewOrderButtons').show();
    return true;
}

function ShowBtnNextTop() {
    $('#divBtnNext').show();
    $('#hdfShowOrderStep').val("");
}
function HideBtnNextTop() {
    $('#divBtnNext').hide();
    $('#hdfShowOrderStep').val("");
}


//*********************************************************************
// Address Validation
//*********************************************************************

function DoAddressValidation() {
    //if (isReprint) { return };
    var shipTo = $('#ddlShipTo').find('option:selected').val();
    var lblDateVerified = $('#lblDateVerified');
    var lblExpireDays = $('#lblExpireDays');
    var hfCurrentStatus = $('#hfOrderStatus');
    var status = hfCurrentStatus.val();
    var date = lblDateVerified.text();
    var days = lblExpireDays.text();
   // var bVerifyAddress = $('#bVerifyAddress');
    var bSaveCreateNewOrder = $('#bSaveCreateNewOrder');
    var bSaveOrder = $('#bSaveOrder');
    var bSaveIncompleteOrder = $('#bSaveIncompleteOrder');
    var divEyeOrderInformation = $('#divEyeOrderInformation');
   
    var oNumber = document.getElementById('lblOrderNumber').innerHTML;
    if (oNumber == '') {
        if (shipTo == "CD") {
            // address validation not necessary
            $('[id*=divOrderOptions]').hide();
         //   $('[id*=bSaveNewOrder]').hide()
           // $('[id*=bReprint771]').hide()

            var divValidateAddress = $('#divValidateAddress');
            var msgValidateAddress = document.getElementById('msgValidateAddress');
            msgValidateAddress.innerHTML = "";
            divValidateAddress.hide();
            status = "";
            //  bVerifyAddress.hide();
            bSaveOrder.show();
            bSaveIncompleteOrder.show(); 
            bSaveCreateNewOrder.show(); 
            divEyeOrderInformation.show(); 
        }
        else if (shipTo == "C2P" || shipTo == "L2P") {
            ValidateAddress(date, days);
        }
        $('#' + DispenseMethod).focus();
    }
    else
    {
        if (status == "Clinic Order Created") {
             if (shipTo == "CD") {
                    // address validation not necessary
                    var divValidateAddress = $('#divValidateAddress');
                    var msgValidateAddress = document.getElementById('msgValidateAddress');
                    msgValidateAddress.innerHTML = "";
                    divValidateAddress.hide();
                    //status = "";
                    bSaveOrder.show();
                    bSaveIncompleteOrder.show();
                    bSaveCreateNewOrder.show();
                    divEyeOrderInformation.show(); 
             }
             else if (shipTo == "C2P" || shipTo == "L2P") {
                 ValidateAddress(date, days); 
             }       
        }
 $('#' + DispenseMethod).focus();
    }
   
}

function ValidateAddress(dateVerified, expireDays) {
    //If the standardized address is selected, this address is considered valid for 90 days. 
    //if a non-standardized address is selected, this address is considered valid for 30 days. 
    var shipTo = $('#ddlShipTo').find('option:selected').val();
    var status = "Adddress validation is current.";
    var verifyExpiration = "";
    var isExpired = false;
    var divValidateAddress = $('#divValidateAddress');
    var msgValidateAddress = document.getElementById('msgValidateAddress');
    var btnValidateAddress = $('#btnValidateAddress');
   // var bVerifyAddress = $('#bVerifyAddress');
    var bSaveCreateNewOrder = $('#bSaveCreateNewOrder');
    var bSaveOrder = $('#bSaveOrder');
    var bSaveIncompleteOrder = $('#bSaveIncompleteOrder');
    var bSaveNewOrder = $('#bSaveNewOrder');
    var bReprint771 = $('#bReprint771');
    var divEyeOrderInformation = $('#divEyeOrderInformation');
    var hfCurrentStatus = $('#hfOrderStatus');
    var currentStatus = hfCurrentStatus.val();

    var oNumber = document.getElementById('lblOrderNumber').innerHTML;


    if (dateVerified != "1/1/0001" && dateVerified != null && dateVerified != "" && expireDays != 0 && expireDays != null)
    {
        var expDate = new Date(dateVerified); // date address was last verified
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
        else {
            status = "<span style='font-weight:normal'>Address validation is current until " + expDate.toLocaleDateString(); ".</span>";
            btnValidateAddress.hide();

            //bVerifyAddress.hide();

            if (oNumber == '') {
                bSaveOrder.show();
                bSaveIncompleteOrder.show();
                bSaveCreateNewOrder.show();
                $('[id*=divOrderOptions]').hide();
                $('[id*=bSaveNewOrder]').hide()
                $('[id*=bReprint771]').hide()
            }
            else {
                if (currentStatus == "Clinic Order Created" || currentStatus == "Hold Order for Batching") {
                    ////if (shipTo == "CD") {
                    // address validation complete
                    var divValidateAddress = $('#divValidateAddress');
                    var msgValidateAddress = document.getElementById('msgValidateAddress');
                    msgValidateAddress.innerHTML = "";
                    divValidateAddress.hide();
                    status = "";
                    bSaveOrder.text = "Update";
                    bSaveOrder.show();
                    //bSaveIncompleteOrder.show();
                    bSaveCreateNewOrder.show();
                bReprint771.show();
                    divEyeOrderInformation.show();
                    //}
                    //else if (shipTo == "C2P" || shipTo == "L2P") {
                    //    ValidateAddress(date, days);
                }
            }






            //bSaveOrder.text = "Update";
            //bSaveOrder.show();
            //bSaveIncompleteOrder.hide();
            //bSaveNewOrder.show();
            //bReprint771.show();
            //bSaveCreateNewOrder.hide();
            //$('[id*=divOrderOptions]').show();
            //$('[id*=bSaveNewOrder]').show()
            //$('[id*=bReprint771]').show()
            }
            divEyeOrderInformation.show();
        }
    else {
        isExpired = true;
    }
        if (isExpired) {
            btnValidateAddress.show();
            status = "<span style='color:#990000'>In order to ship to patient, there must be a valid address on file.  This patient's mailing address has not been validated or validation has expired. Please validate this address or select 'Clinic Distribution' as the dispense method for this order.</span>";   // validation is not current
          //  bVerifyAddress.hide();
            bSaveOrder.hide(); 
            bSaveIncompleteOrder.hide();
            bSaveCreateNewOrder.hide();
            divEyeOrderInformation.hide(); 
            bReprint771.show();
                bSaveNewOrder.hide();

    divValidateAddress.show();
    msgValidateAddress.innerHTML = status;
}
    //else {
    //   // btnValidateAddress.show();
    //   // status = "<span style='color:#990000'>In order to ship to patient, there must be a validated address on file.  This patient's mailing address has not been validated or validation has expired. Please validate this address or select 'Clinic Distribution' as the dispense method for this order.</span>";   // validation is not current
    //   //// bVerifyAddress.hide();
    //        bSaveOrder.show();
    //        bReprint771.show();
    //   // bSaveIncompleteOrder.hide();
    //    bSaveCreateNewOrder.hide();
    //    divEyeOrderInformation.show(); 
   }

function DoRejectRedirect(action) {
    if (action == 'none') {
        $('#divLabRedirectReject').hide();
        return;
    }

    if ($('#divLabRedirectReject').is('visible') == false)
        $('#divLabRedirectReject').show();

    switch (action) {
        case 'redirect':
            $('#divHoldForStock').hide();
            $('#divReject').hide();
            $('#divRedirect').show();
            $('#tbLabJust').val($('#hfRedirectJust').val());
            break;
        case 'reject':
            $('#divHoldForStock').hide();
            $('#divReject').show();
            $('#divRedirect').hide();
            $('#tbLabJust').val($('#hfRejectJust').val());
            break;
        case 'hfs':
            $('#divHoldForStock').show();
            $('#divReject').hide();
            $('#divRedirect').hide();
            $('#tbLabJust').val('');
            break;
    }
}

//*********************************************************************
// GLOBAL
//*********************************************************************

function ComboBoxVal(clientId, ctlCntrId, errorCtlId, ddlLabelText) {
    var good = false;
    if ($('#' + clientId).val() != 'X')
        good = Pass(clientId, ctlCntrId, errorCtlId);
    else
        good = Fail(clientId, ctlCntrId, errorCtlId, ddlLabelText + ' is a required field. ');

    if (ctlCntrId.toLowerCase() != 'divorder') return good;
    EnableSaveIncompleteReprint771();
    return good;
}

function DdlHelper(ctlId, ctlCntrId, errorCtlId, ddlLabelText) {
    var good = false;
    if (DdlRequriedFieldVal(ctlId))
        good = Pass(ctlId, ctlCntrId, errorCtlId);
    else
        good = Fail(ctlId, ctlCntrId, errorCtlId, ddlLabelText + ' is a required field. ');

    if (ctlCntrId.toLowerCase() != 'divorder') return good;
    EnableSaveIncompleteReprint771();
    return good;
}

function DoToggle(div, img) {
    $('#' + div).toggle();

    if (img == null) return;
    var i = $('#' + img);
    var current = i.attr("src");
    var swap = i.attr("data-swap");

    //if (div == "divPrescriptionHist") {
    //    if (swap == "../../Styles/images/BlueArrowOpen.gif") { $('#lblPrescriptions').text('Expand Prescriptions') };
    //    if (swap == "../../Styles/images/BlueArrowClose.gif") { $('#lblPrescriptions').text('Collapse Prescriptions') };
    //}
    //if (div == "divExam") {
    //    if (swap == "../../Styles/images/BlueArrowOpen.gif") { $('#lblExams').text('Expand Exams') };
    //    if (swap == "../../Styles/images/BlueArrowClose.gif") { $('#lblExams').text('Collapse Exams') };
    //}
    //if (div == "divOrderNotification") {
    //    if (swap == "../../Styles/images/BlueArrowOpen.gif") { $('#lblOrderNotifications').text('Expand Order Notifications') };
    //    if (swap == "../../Styles/images/BlueArrowClose.gif") { $('#lblOrderNotifications').text('Collapse Order Notifications') };
    //}
    i.attr('src', swap).attr('data-swap', current);
}

function DoFocus(ctlId) {
    var ctlNextTabIndex = (parseInt($('[id*=' + ctlId + ']').next().attr("tabindex"))) + 1;
    var ctlArray = $('.tabable');

    for (i = 0; len = ctlArray.length, i <= len; i++) {
        if ((parseInt($(ctlArray[i]).attr("tabindex"))) == ctlNextTabIndex) {
            setTimeout(ctlArray.eq(i).focus(), 20);
        }
    }
}

//*********************************************************************
// PRESCRIPTIONS
//*********************************************************************

function validateFileSize() {
  
}

function DoScriptDialog() {
    var dialogOpts = {
        autoOpen: false,
        modal: true,
        width: 500,
        height: 780,
        title: 'Prescription Information',
        dialogClass: 'generic',
        open: function () {
            $('[id*=ddlPrescriptionName]').focus();
        },
        close: function () {
            $(this).dialog('destroy');
            $(this).hide();

            ClearMsgs('divPrescription', 'errorMessage');
        }
    };
    var d2 = $('#divPrescription').dialog(dialogOpts);
    d2.parent().appendTo($('form:first'));
    d2.dialog('open');
}

function PrescriptionSaveUpdateButtonClick() {
    var good = true;

    // If the delete checkbox is checked the return true...
    if ($('[id$=cbDeletePrescription]').is(':checked')) {
        ClearMsgs('divPrescription', 'errorMessage');
        TogglePrescriptionSaveAsNew();
        return true;
    }

    TogglePrescriptionSaveAsNew();

    if (!DoPrescriptionDateVal('tbPrescriptionDate'))
        good = false;

    if (!DdlHelper('ddlPrescriptionProvider', 'divPrescription', 'errorMessage', 'Provider name'))
        good = false;

    if (!DdlHelper('ddlPrescriptionName', 'divPrescription', 'errorMessage', 'Prescription type'))
        good = false;

    if ($('#tbOdSphere').val() != '')
        if (!SphInput('tbOdSphere', 'true'))
            good = false;

    if ($('#tbOsSphere').val() != '')
        if (!SphInput('tbOsSphere', 'true'))
            good = false;

    if ($('#tbOdCylinder').val() != '')
        if (!CylInput('tbOdCylinder', 'tbOdAxis', 'true'))
            good = false;

    if ($('#tbOsCylinder').val() != '')
        if (!CylInput('tbOsCylinder', 'tbOsAxis', 'true'))
            good = false;

    if ($('#tbOdAxis').val() != '')
        if (!AxisInput('tbOdAxis', 'tbOdCylinder'))
            good = false;

    if ($('#tbOsAxis').val() != '')
        if (!AxisInput('tbOsAxis', 'tbOsCylinder'))
            good = false;

    if ($('#tbOdAdd').val() != '')
        if (!AddInput('tbOdAdd'))
            good = false;

    if ($('#tbOsAdd').val() != '')
        if (!AddInput('tbOsAdd'))
            good = false;

    if ($('#cbMonoOrComboPd').is(':checked') == false) {
        if (!DoComboPdVal('tbOdPdNearCombo'))
            good = false;

        if (!DoComboPdVal('tbOdPdDistCombo'))
            good = false;
    }
    else {
        if (!DoMonoPdVal('tbOdPdDistMono'))
            good = false;

        if (!DoMonoPdVal('tbOsPdDistMono'))
            good = false;

        if (!DoMonoPdVal('tbOdPdNearMono'))
            good = false;

        if (!DoMonoPdVal('tbOsPdNearMono'))
            good = false;
    }
    var pbGood = DoAllPrismsAndBasesVal();

    return good && pbGood;
}

function SphInput(ctlId, isSave) {
    var _text = $('#' + ctlId).val();

    if (!DoCylSphVal(ctlId)) {
        Fail(ctlId, 'divPrescription', 'errorMessage', "Value must be between -25 and +25, in increments of 0.25");
        return false;
    }
    else {
        Pass(ctlId, 'divPrescription', 'errorMessage');
        if (_text == "") {
            DoRxCalc(ctlId.substring(2, 4));
            return;
        }
    }

    var sign = _text.charAt(0);
    var num = parseFloat(_text);

    var mod = Math.abs(num % 1);
    mod = mod.toPrecision(2);
    var modAbs = Math.abs(num);

    if (mod == "0.20" || mod == "0.70") modAbs = modAbs + .05;

    AddSign(sign, modAbs, ctlId);

    DoRxCalc(ctlId.substring(2, 4));

    if (!isSave) ChkSphereSigns();

    return true;
}

function CylInput(ctlId, axisId, isSave) {
    var _text = $('#' + ctlId).val();

    if (!DoCylSphVal(ctlId)) {
        Fail(ctlId, 'divPrescription', 'errorMessage', "Value must be between -25 and +25, in increments of 0.25");
        if ($('#' + axisId).val() == '')
            Fail(axisId, 'divPrescription', 'errorMessage', "Axis is required when cylinder is present");
        return false;
    }
    else {
        Pass(ctlId, 'divPrescription', 'errorMessage');
        Pass(axisId, 'divPrescription', 'errorMessage');
        if (_text == "") {
            DoRxCalc(ctlId.substring(2, 4));
            return true;
        }
    }

    var sign = _text.charAt(0);
    var num = parseFloat(_text);

    var mod = Math.abs(num % 1);
    mod = mod.toPrecision(2);
    var modAbs = Math.abs(num);

    if (mod == "0.20" || mod == "0.70") modAbs = modAbs + .05;

    AddSign(sign, modAbs, ctlId);

    DoRxCalc(ctlId.substring(2, 4));

    if (!DoAxisVal(axisId, ctlId))
        Fail(axisId, 'divPrescription', 'errorMessage', "Axis required, value must be between 1 and 180");

    if (!isSave) ChkCylinderSigns();

    return true;
}

function AxisInput(ctlId, cylId) {
    var _text = $('#' + ctlId).val();

    if (!DoAxisVal(ctlId, cylId)) {
        Fail(ctlId, 'divPrescription', 'errorMessage', "Axis required, value must be between 1 and 180");
        return false;
    }
    else {
        Pass(ctlId, 'divPrescription', 'errorMessage');
        if (_text == "") {
            DoRxCalc(ctlId.substring(2, 4));
            return;
        }
    }
    var num = parseInt(_text, 10);
    var paddedNum = "";

    paddedNum = String("000" + num).slice(-3);
    $('#' + ctlId).val(paddedNum);

    DoRxCalc(ctlId.substring(2, 4));

    return true;
}

function AddInput(ctlId) {
    var _text = $('#' + ctlId).val();

    if (!DoAddVal(ctlId)) {
        Fail(ctlId, 'divPrescription', 'errorMessage', "Value must be between 0.25 and 15, in increments of 0.25");
        return false;
    }
    else {
        Pass(ctlId, 'divPrescription', 'errorMessage');
        if (_text == "") return true;
    }

    var sign = _text.charAt(0);
    var num = parseFloat(_text);

    var mod = Math.abs(num % 1);
    mod = mod.toPrecision(2);
    var modAbs = Math.abs(num);

    if (mod == "0.20" || mod == "0.70") modAbs = modAbs + .05;

    $('#' + ctlId).val(modAbs.toFixed(2));

    return true;
}

function PrismInput(ctlId, baseId) {
    var _text = $('#' + ctlId).val();

    if (!DoPrismVal(ctlId)) {
        Fail(ctlId, 'divPrescription', 'errorMessage', "Value must be between 0.25 and 15, in increments of 0.25");
        return false;
    }
    else {
        Pass(ctlId, 'divPrescription', 'errorMessage');
        if (_text == "") {
            $('#' + baseId).val('');
            return;
        }
    }

    var sign = _text.charAt(0);
    var num = parseFloat(_text);

    var mod = Math.abs(num % 1);
    mod = mod.toPrecision(2);
    var modAbs = Math.abs(num);

    if (mod == "0.20" || mod == "0.70") modAbs = modAbs + .05;

    $('#' + ctlId).val('+' + modAbs.toFixed(2));

    return true;
}

function AddSign(sign, num, ctlId) {
    switch (sign) {
        case "+":
            $('#' + ctlId).val('+' + num.toFixed(2));
            break;
        case "-":
            $('#' + ctlId).val('-' + num.toFixed(2));
            break;
        default:
            $('#' + ctlId).val((num * -1).toFixed(2));
            break;
    }
}

function PrismBase(ctlId, depCtlId) {
    var val = $('#' + ctlId).val();
    var depVal = $('#' + depCtlId).val();

    if (!DoPrismBaseVal(ctlId, depCtlId)) return;

    if (depVal == '') {
        $('#' + ctlId).val('');
        return;
    }

    $('#' + ctlId).val(val.toUpperCase());
}

function DoRxCalc(Side) {
    var Sph = $('#tb' + Side + 'Sphere').val();
    Sph = Sph == '' ? 0 : parseFloat(Sph);
    var Cyl = $('#tb' + Side + 'Cylinder').val();
    Cyl = Cyl == '' ? 0 : parseFloat(Cyl);
    var Axis = $('#tb' + Side + 'Axis').val();
    Axis = Axis == '' ? 0 : parseInt(Axis, 10);

    var tSph = "";
    var tCyl = "";
    var tAxis = "";

    if (Sph == 0.00 || !isFinite(Sph)) {
        tSph = "Plano"
    }

    if (Axis == 0.00 || !isFinite(Axis)) {
        tAxis = "N/A";
    }

    if (Cyl == 0 || !isFinite(Cyl)) {
        tCyl = "Sphere";
        if (Sph == 0) {
            tSph = "Plano";
        }
        else {
            tSph = Sph.toFixed(2).toString();
        }
        tAxis = "N/A";
    }
    else {
        if (isFinite(Axis)) {
            if (Cyl < 0) {
                tSph = Sph > 0 ? "+" + (Sph.toFixed(2)).toString() : (Sph.toFixed(2)).toString();
                tCyl = Cyl > 0 ? "+" + (Cyl.toFixed(2)).toString() : (Cyl.toFixed(2)).toString();
                tAxis = Axis < 100 ? "0" + Axis.toString() : Axis.toString();
                tAxis = Axis < 10 ? "0" + tAxis : tAxis;
            }
            else if (Cyl > 0) {
                tCyl = (Cyl * -1) > 0 ? "+" + ((Cyl * -1).toFixed(2)).toString() : ((Cyl * -1).toFixed(2)).toString()
                tSph = (Sph + Cyl) > 0 ? "+" + ((Sph + Cyl).toFixed(2)).toString() : ((Sph + Cyl).toFixed(2)).toString();
                if (Axis > 90) {
                    Axis -= 90;
                }
                else {
                    Axis += 90
                }
                tAxis = Axis < 100 ? "0" + Axis.toString() : Axis.toString();
                tAxis = Axis < 10 ? "0" + tAxis : tAxis;
            }
        }
    }

    Side = Side.toUpperCase();
    $('#lbl' + Side + 'Sphere_calc').text(tSph);
    $('#lbl' + Side + 'Cylinder_calc').text(tCyl);
    $('#lbl' + Side + 'Axis_calc').text(tAxis);

    $('#hf' + Side + 'SphereCalc').val(tSph);
    $('#hf' + Side + 'CylinderCalc').val(tCyl);
    $('#hf' + Side + 'AxisCalc').val(tAxis);

    if (!$('#divCalcValues').is(':visible') && Cyl > 0) {
        DoToggle('divCalcValues', 'imgCalcValues');
    }

    Side == "OD" && $('#lblOSSphere_calc').text() == "" && $('#lblOSCylinder_calc').text() == "" && $('#lblOSAxis_calc').text() == "" ? DoRxCalc("Os") : false;
    Side == "OS" && $('#lblODSphere_calc').text() == "" && $('#lblODCylinder_calc').text() == "" && $('#lblODAxis_calc').text() == "" ? DoRxCalc("Od") : false;

    return false;
}

function AddHelper(source, dest) {
    if (!AddInput(source)) {
        return false;
    }
    if ($('#' + dest).val() != '') {
        return true;
    }
    $('#' + dest).val($('#' + source).val());
}

function CalcSetNearComboPdField(calcSource, calcDest) {
    if (!DoComboPdVal(calcSource))
        if (!InErrorState()) {
            EnableSaveIncompleteReprint771();
            return false;
        }

    var s = parseInt($('#' + calcSource).val(), 10);

    if (s == '') {
        EnableSaveIncompleteReprint771();
        return true;
    }
    var d = 0;
    if (s >= 55)
        d = s - 3;
    else
        d = 52;

    $('#' + calcDest).val(d);
    Pass(calcDest, 'divPrescription', 'errorMessage');
    EnableSaveIncompleteReprint771();
    return true;
}

function CalcSetNearMonoPdField(calcSource, calcDest) {
    if (!DoMonoPdVal(calcSource))
        if (!InErrorState()) {
            EnableSaveIncompleteReprint771();
            return false;
        }

    var s = parseInt($('#' + calcSource).val(), 10);

    if (s == '') {
        EnableSaveIncompleteReprint771();
        return true;
    }
    var d = 0;

    if (s >= 27.5)
        d = s - 1.5;
    else
        d = 26;

    $('#' + calcDest).val(d);
    Pass(calcDest, 'divPrescription', 'errorMessage');
    EnableSaveIncompleteReprint771();
    return true;
}

function DoSelectedPrescription() {
    DoScriptDialog();
    $('[id$=bSaveRxOpenOrder]').hide();
    var isMono = $('[id$=cbMonoOrComboPd]').is(':checked');
    if (isMono == true) {
        $('#divMonoPd').show();
        $('#divComboPd').hide();
    }
    else {
        $('#divMonoPd').hide();
        $('#divComboPd').show();
    }

    DoRxCalc('Od');
    DoRxCalc('Os');
}

function DoAddPrescription() {
    //Clear out all textboxes and reset the DDL
    ClearPrescriptionFields();
    $('[id*=ddlPrescriptionName]').focus();
    //$('[id*=ddlPrescriptionName]').val("FTW");
    $('[id*=ddlPrescriptionName]').val($('[id*=hfRxName]').val());

    $('[id*=bSaveUpdatePrescription]').show();
    $('[id*=bSaveUpdatePrescription]').val('Save');

    $('[id$=bSaveRxOpenOrder]').show();

    $('[id$=hfButtonText]').val('Save');
    $('[id$=bSaveNewPrescription]').hide();

    $('#cbDeletePrescription').hide();
    $('#cbDeletePrescription').next('label').hide();

    DoScriptDialog();

    if ($('#divOrder').is(':visible')) {
        DoToggle('divOrder', '');
    }

    if ($('#divPrismBase').is(':visible')) {
        DoToggle('divPrismBase', 'imgPrismBase');
    }

    if ($('#divCalcValues').is(':hidden')) return;

    DoToggle('divCalcValues', 'imgCalcValues');
}

function ClearPrescriptionFields() {
    $('#ddlPrescriptionName').prop('selectedIndex', 0);
    $('#tbPrescriptionDate').val(CurrentDate);
    $('#tbOdAdd').val('');
    $('#tbOdAxis').val('');
    $('#tbOdCylinder').val('');
    $('#tbOdHBase').val('');
    $('#tbOdHPrism').val('');

    //$('#tbOdPdDistCombo').val('63.00');
    $('#tbOdPdDistCombo').val($('[id*=hfOdPdDistCombo]').val());

    //$('#tbOdPdNearCombo').val('60.00');
    $('#tbOdPdNearCombo').val($('[id*=hfOdPdNearCombo]').val());

    $('#tbOdPdDistMono').val('31.50');
    $('#tbOdPdNearMono').val('30.00');
    $('#tbOsPdDistMono').val('31.50');
    $('#tbOsPdNearMono').val('30.00');

    $('#tbOdSphere').val('');
    $('#tbOdVBase').val('');
    $('#tbOdVPrism').val('');
    $('#tbOsAdd').val('');
    $('#tbOsAxis').val('');
    $('#tbOsCylinder').val('');
    $('#tbOsHBase').val('');
    $('#tbOsHPrism').val('');
    $('#tbOsSphere').val('');
    $('#tbOsVBase').val('');
    $('#tbOsVPrism').val('');

    //$('#ddlPrescriptionProvider').prop('selectedIndex', 0);
    $('#ddlPrescriptionProvider').val($('[id*=hfProviderId]').val());

    $('#cbDeletePrescription').removeProp('checked');
    $('#cbMonoOrComboPd').removeProp('checked');

    $('#hfODSphereCalc').val('');
    $('#hfODCylinderCalc').val('');
    $('#hfODAxisCalc').val('');
    $('#hfOSSphereCalc').val('');
    $('#hfOSCylinderCalc').val('');
    $('#hfOSAxisCalc').val('');

    $('#lblODSphere_calc').text('');
    $('#lblODCylinder_calc').text('');
    $('#lblODAxis_calc').text('');
    $('#lblOSSphere_calc').text('');
    $('#lblOSCylinder_calc').text('');
    $('#lblOSAxis_calc').text('');
    $('#divExtraRxs').css('display','block');
    ToggleMonoComboPd('cbMonoOrComboPd');
}

function ToggleMonoComboPd(cbId) {
    var a = $('#' + cbId).is(':checked');
    if (a == true) {
        $('#divComboPd').hide();
        Pass('tbOdPdDistCombo', 'divPrescription', 'errorMessage');
        Pass('tbOdPdNearCombo', 'divPrescription', 'errorMessage');
        $('#divMonoPd').show();
        DoMonoPdVal('tbOdPdDistMono');
        DoMonoPdVal('tbOdPdNearMono');
        DoMonoPdVal('tbOsPdDistMono');
        DoMonoPdVal('tbOsPdNearMono');
    }
    else {
        $('#divComboPd').show();
        DoComboPdVal('tbOdPdDistCombo');
        DoComboPdVal('tbOdPdNearCombo');
        $('#divMonoPd').hide();
        Pass('tbOdPdDistMono', 'divPrescription', 'errorMessage');
        Pass('tbOdPdNearMono', 'divPrescription', 'errorMessage');
        Pass('tbOsPdDistMono', 'divPrescription', 'errorMessage');
        Pass('tbOsPdNearMono', 'divPrescription', 'errorMessage');
    }
}

function TogglePrescriptionSaveAsNew() {
    if ($('#cbDeletePrescription').is(':checked'))
        $('[id$=bSaveNewPrescription]').attr('disabled', true);
    else
        $('[id$=bSaveNewPrescription]').removeAttr('disabled');
}

function ChkSphereSigns() {
    var sphOD = $('#tbOdSphere').val().substring(0, 1);
    var sphOS = $('#tbOsSphere').val().substring(0, 1);

    if ((sphOD == "+" && sphOS == "-") || (sphOD == "-" && sphOS == "+")) {
        alert('Sphere values have opposite signs.');
    }
}

function ChkCylinderSigns() {
    var cylOD = $('#tbOdCylinder').val().substring(0, 1);
    var cylOS = $('#tbOsCylinder').val().substring(0, 1);

    if ((cylOD == "+" && cylOS == "-") || (cylOD == "-" && cylOS == "+")) {
        alert('Cylinder values have opposite signs.');
    }
}

function IsHighPowerLensRx() {
    //If either Abs(sph) or Abs(sph + cyl) is >= 6, then select hi-index.
    //For example, a +7.00 -2.00 would be high index, as would a -5.00-2.00.

    var odSph = parseFloat($('#hfODSphereCalc').val());
    var odCyl = parseFloat($('#hfODCylinderCalc').val());
    var osSph = parseFloat($('#hfOSSphereCalc').val());
    var osCyl = parseFloat($('#hfOSCylinderCalc').val());

    return Math.abs(odSph) >= 6 || Math.abs(odSph + odCyl) >= 6 || Math.abs(osSph) >= 6 || Math.abs(osSph + osCyl) >= 6;
}
//*********************************************************************
// ORDERS
//*********************************************************************

function DoShowOrder(orderNumber) {
    __doPostBack('ShowOrder', orderNumber);
}

function DoShowDoc() {
    return true;
}


function DoDialogAjax() {
    $.ajax({
        type: 'POST',
        url: 'OrderManagement.aspx/AjaxOrderHistory',
        data: '{PatientId: "' + $('#hfPatientId').val() + '",UserId: "' + $('#hfUserId').val() + '"}',
        contentType: 'application/json; charset-utf-8',
        dataType: 'json',
        success: function (data) {
            var t = $('#tblOrderHist');
            t.empty();

            // Set the header row.
            t.append('<tr><th></th><th>Frame</th><th>Date Last Modified</th><th>Current Status</th></tr>');

            if (data.d.length == 0) return;

            for (var i = 0; i < data.d.length; i++) {
                var dt = new Date(parseInt(data.d[i].DateLastModified.substr(6), 10));
                if (i % 2 == 0)
                    t.append('<tr><td><a href="#" id="lbOrderHist' + i.toString() + '" runat="server" onclick="DoShowOrder(\'' +
                        data.d[i].OrderNumber + '\')">Select</a></td><td>' +
                        data.d[i].Frame + '</td><td>' +
                        $.datepicker.formatDate('m/d/yy', dt) + ' ' + dt.toLocaleTimeString() + '</td><td>' +
                        data.d[i].CurrentStatus + '</td></tr>');
                else
                    t.append('<tr class="OrderHistAlt"><td><a href="#" id="lbOrderHist' + i.toString() + '" runat="server" onclick="DoShowOrder(\'' +
                        data.d[i].OrderNumber + '\')">Select</a></td><td>' +
                        data.d[i].Frame + '</td><td>' +
                        $.datepicker.formatDate('m/d/yy', dt) + ' ' + dt.toLocaleTimeString() + '</td><td>' +
                        data.d[i].CurrentStatus + '</td></tr>');
            }
        },
        failure: function () { }
    });

    var dialogOpts = {
        autoOpen: false,
        modal: true,
        width: 950,
        height: 450,
        title: 'Order History',
        dialogClass: 'generic',
        closeOnEscape: true
    };
    var doh = $('#divOrderHistory').dialog(dialogOpts);
    doh.parent().appendTo($('form:first'));

    doh.dialog('open');
}

function DoDialog() {
    var dialogOpts = {
        autoOpen: false,
        modal: true,
        width: 950,
        height: 450,
        title: 'Order History',
        dialogClass: 'generic',
        closeOnEscape: true
    };
    var doh = $('#divOrderHistory').dialog(dialogOpts);
    doh.parent().appendTo($('form:first'));

    doh.dialog('open');
}

function DoIncompleteDialog() {
    $.ajax({
        type: 'POST',
        url: 'OrderManagement.aspx/AjaxIncompleteOrderHistory',
        data: '{PatientId: "' + $('#hfPatientId').val() + '",UserId: "' + $('#hfUserId').val() + '"}',
        contentType: 'application/json; charset-utf-8',
        dataType: 'json',
        success: function (data) {
            var t = $('#tblIncOrderHist');
            t.empty();

            // Set the header row.
            t.append('<tr><th></th><th>Order Number</th><th>Date Last Modified</th><th>Current Status</th></tr>');

            if (data.d.length == 0) return;
            for (var i = 0; i < data.d.length; i++) {
                var dt = new Date(parseInt(data.d[i].DateLastModified.substr(6), 10));
                if (i % 2 == 0)
                    t.append('<tr><td><a href="#" id="lbOrderHist' + i.toString() + '" runat="server" onclick="DoShowOrder(\'' + data.d[i].OrderNumber + '\')">Select</a></td><td>' + data.d[i].OrderNumber + '</td><td>' + $.datepicker.formatDate('m/d/yy', dt) + ' ' + dt.toLocaleTimeString() + '</td><td>' + data.d[i].CurrentStatus + '</td></tr>');
                else
                    t.append('<tr class="OrderHistAlt"><td><a href="#" id="lbOrderHist' + i.toString() + '" runat="server" onclick="DoShowOrder(\'' + data.d[i].OrderNumber + '\')">Select</a></td><td>' + data.d[i].OrderNumber + '</td><td>' + $.datepicker.formatDate('m/d/yy', dt) + ' ' + dt.toLocaleTimeString() + '</td><td>' + data.d[i].CurrentStatus + '</td></tr>');
            }
        },
        failure: function () { }
    });

    var dialogOpts = {
        autoOpen: false,
        modal: true,
        width: 750,
        height: 400,
        title: 'Incomplete Order History',
        dialogClass: 'generic'
    };
    var dio = $('#divIncompleteOrderHistory').dialog(dialogOpts);
    dio.parent().appendTo($('form:first'));

    dio.dialog('open');
}



function DoAddOrder() {
    $('[id*=tbPair]').val(1);
    //$('[id*=ddlShipTo]').val('C');
    //$('[id*=rblShipTo] input[value=C]').attr('checked', true);
    $('[id*=lblOrderingTech]').text('<%=HttpContext.Current.User.Identity.Name%>');
    $('[id*=lblCurrentLabHead]').hide();
    $('[id*=lblCurrentLab]').hide();
    $('[id*=lblOrderingTech]').hide();
    $('[id*=lblTechnician]').hide();
    $('[id$=hfIsPrevOrderFOC]').val('');
    $('[id*=tbFocJust]').val('');
    $('[id*=tbMaterialJust]').val('');
    $('[id*=tbCoatingJust]').val('');
    $('[id*=tbRejectLabReason]').val('');
    $('[id*=tbRejectClinicReply]').val('');
    $('[id*=tbComment1]').val('');
    $('[id*=tbComment2]').val('');
    $('[id*=hfIsFocFrame]').val('');
    $('[id*=hfIsFocEligible]').val('');
    $('[id*=hfMaxPair]').val('');
    $('[id*=hfIsFocEligible]').val('N');

    if ($('[id*=lblOrderNumber]').val() == '') {
        $('[id*=divOrderOptions]').hide();
        $('[id*=bSaveNewOrder]').hide()
        $('[id*=bReprint771]').hide()
    }

    // If the hfOppositeSign field is not bland then add the message to comment2.
    if ($('#hfOppositeSign').val() != '')
        $('[id*=tbComment2]').val('Opposite signs verified.');

    DoOrderDialog();

    $('[id*=hrJustBlock]').hide();

    ClearMsgs('divOrder', 'divOrderError');

    $('input:text[id^=' + OrderPriority + ']').focus();
}

function CloseOrderDialog() {
    $('#divOrder').dialog('close');
}

function DoOrderDialog() {
    //var ht = IsLab ? 850 : 750;
    var dialogOpts = {
        autoOpen: false,
        modal: true,
        width: 800,
        height: 870,//850
        //position: { my: "center", at: "top", of: window },
        title: 'Eyewear Order Information',
        dialogClass: 'generic',
        create: function () {
            $(this).closest('div.ui-dialog')
                .find('.ui-dialog-titlebar-close')
                .click(function (e) {
                    if ($('#' + OrderIsPrefill).val() == 'False') return;
                    SharedRedirect(window.location.href);
                    //var response = window.location.href;
                    //window.location.href = response;
                });
        },
        open: function () {
            $('#' + NextCombo).val(OrderPriority);
            //$('#' + NextCombo).val(DispenseMethod);
            HideBtnNextTop();
        },
        close: function () {
            $(this).dialog('destroy');
            $(this).hide();
            ClearMsgs('divOrder', 'divOrderError');
        }
    };
    var d3 = $('#divOrder').dialog(dialogOpts);
    d3.parent().appendTo($('form:first'));
    d3.dialog('open');
}

function DoReOrderDialog() {
    var dialogOpts = {
        autoOpen: false,
        modal: true,
        closeOnEscape: false,
        width: 350,
        height: 245,
        title: 'Eyewear Order Information',
        dialogClass: 'generic',
        zindex: 1100,
        open: function (event, ui) {
            $('[id$=ddlReorderReason]').focus();
        },
        close: function () {
            $('[id$=ddlReorderReason]').val('X');
            $('[id$=tbOther]').val('');
            $(this).dialog('destroy');
            $(this).hide();
        }
    };
    var d3 = $('#divReorder').dialog(dialogOpts);
    d3.parent().appendTo($('form:first'));
    d3.dialog('open');
}

function DoUpdateEmailAddressDialog() {
        var dialogOpts = {
            autoOpen: false,
            modal: true,
            closeOnEscape: false,
            width: 450,
            height: 150,
            title: 'Update Email Address',
            dialogClass: 'generic',
            zindex: 1100,
            open: function (event, ui) {
                $('[id$=tbEmailAddress]').val($('#hdfPatientEmailAddress').val());
                $('[id$=tbEmailAddress]').focus();
            },
            close: function () {
                $('#divUpdateEmailAddress').dialog('destroy');
                $('#divUpdateEmailAddress').hide();
                ClearMsgs('divUpdateEmailAddress', 'divUpdateEmailAddressMsg');
            }
        };
        var d3 = $('#divUpdateEmailAddress').dialog(dialogOpts);
        d3.parent().appendTo($('form:first'));
        d3.dialog('open');
    
}


function DoRedirect(response) {
    SharedRedirect(response.d);
}

function DoSelectedOrder(isLab) {
    DoOrderDialog();
    $('#cbDelete').removeProp('checked');
    $('#cbReturnToStock').removeProp('checked');
    $('#cbDispense').removeProp('checked');
    ToggleSegHt();
    if (isLab === false) {
        OrderRequiresFocJust();
        OrderRequiresMatJust();
        OrderRequiresCoatingJust();
        return;
    }

        $('[id*=divOrderOptions]').show();
        $('[id*=bSaveNewOrder]').show()
        $('[id*=bReprint771]').show()
    EnableRejectRedirect();
}

function SetLab(forceSingle) {
    var c = $('#' + Lab).find('option');
    var length = c.length;
    var lens = $('#' + Lens).val();

    if (length <= 2) {
        $('#' + Lab).prop('selectedIndex', length - 1);
        return true;
    }

    var t = '';
    if (lens.substring(0, 2).toLowerCase() == 'sv') {
        if (!forceSingle) return;
        // Determine if there is a lab preference and use it.
        var l = $('#hfLabPreference').val();
        t = l == '' ? 'SV' : l;
    }
    else
        t = 'MV';

    var o = $('#' + Lab + ' option:contains(' + t + ')');
    o.prop('selected', true);
    $('#' + Lab).next().val(o.text());

    SetLabComboboxState();
}

function SegHtHelper(source, dest) {
    if (!OrderRequiresSegHt())
        if (!InErrorState()) {
            if (!IsDelete()) return false;
            DoDeleteCheckAction();
            return false;
        }
    CopySegHt(source, dest);
    Pass(dest, 'divOrder', 'divOrderError');
    if (!IsDelete()) return true;
    DoDeleteCheckAction();
    return true;
}

function CopySegHt(source, dest) {
    if ($('[id$=' + dest + ']').val() != '') {
        EnableSaveIncompleteReprint771();
        return;
    }
    $('[id$=' + dest + ']').val($('[id$=' + source + ']').val());
    EnableSaveIncompleteReprint771();
}

function ToggleSegHt() {
    var c = $('#' + Lens);

    if (c.children('option').length == 0) return true;

    var lens = c.val();

    if (lens == null || lens == 'X') return;

    if (lens.substring(0, 2).toLowerCase() == 'sv') {
        $('[id$=tbOdSegHt]').prop('disabled', true);
        $('[id$=tbOsSegHt]').prop('disabled', true);
        $('[id$=tbOdSegHt]').val('');
        $('[id$=tbOsSegHt]').val('');
    }
    else {
        $('[id$=tbOdSegHt]').removeProp('disabled');
        $('[id$=tbOsSegHt]').removeProp('disabled');
    }
}

function OrderRequiresSegHt() {
    var c = $('#' + Lens);

    if (c.children('option').length == 0) return true;

    var val = c.val();

    var g1 = false;
    var g2 = false;

    if (val.substring(0, 2).toLowerCase() == 'sv') {
        if ($('[id$=tbOsSegHt]').val() == '') {
            Pass('tbOsSegHt', 'divOrder', 'divOrderError');
            g1 = true;
        }
        if ($('[id$=tbOdSegHt]').val() == '') {
            Pass('tbOdSegHt', 'divOrder', 'divOrderError');
            g2 = true;
        }
        EnableSaveIncompleteReprint771();
        return g1 && g2;
    }

    var g = true;
    if (!DoSegHtVal('tbOsSegHt'))
        g = false;
    if (!DoSegHtVal('tbOdSegHt'))
        g = false;

    EnableSaveIncompleteReprint771();
    return g;
}

function ToggleOrderUpdateButton(cbId) {
    if ($('[id$=' + cbId + ']').is(':checked')) {
        $('[id$=bSaveOrder]').show();
        $('[id$=bSaveOrder]').removeProp('disabled');
        $('[id$=bSaveNewOrder]').hide();
        $('[id$=bReprint771]').hide();
    }
    else {
        if (!IsIncomplete)
            $('[id$=bSaveOrder]').hide();
        $('[id$=bSaveOrder]').prop('disabled', true);
        $('[id$=bSaveNewOrder]').show();
        $('[id$=bReprint771]').show();
    }
}

function OrderRequiresMatJust() {
    if (IsDelete()) return;

    var c = $('#' + Material);
    var val = c.val();
    if (val == null) return;
    var reqJust = false;

    //reqJust = (val.toUpperCase() != "PLAS" && val.toUpperCase() != '') && c.children('option').length > 2 ? true : false;
    reqJust = (val.toUpperCase() != "PLAS" && val.toUpperCase() != '') && c.children('option').length > 2 && !IsHighPowerLensRx() ? true : false;

    if (reqJust) {
        if (!$('[id$=divJustBlock]').is(':visible')) {
            DoToggle('divJustBlock');
            $('[id$=hrJustBlock]').toggle();
        }
        if (!$('[id$=divMatJust]').is(':visible')) {
            $('[id$=divMatJust]').show();
        }
        DoMatVal('tbMaterialJust')
    }
    else {
        $('[id$=divMatJust]').hide();
        Pass('tbMaterialJust', 'divOrder', 'divOrderError');
        if (!$('[id$=divFocJust]').is(':visible')) {
            $('[id$=divJustBlock]').hide();
            $('[id$=hrJustBlock]').hide();
        }
    }

    //EnableRejectRedirect()
}

function OrderRequiresCoatingJust() {
    if (IsDelete()) return;

    var reqJust = false;

    $("[id*=ddlCoating] input:checked").each(function () {
        reqJust = true;
        $('[id$=divCoatingJust]').show();
    });

    if (reqJust) {
       // if (!$('[id$=divJustBlock]').is(':visible')) {
        //    DoToggle('divJustBlock');
          //  $('[id$=hrJustBlock]').toggle();
        //}
        //if (!$('[id$=divCoatingJust').is(':visible')) {
        //    $('[id$=divCoatingJust]').show();
        //}
        DoCoatingVal('tbCoatingJust');
    }
    else {
        $('[id$=divCoatingJust]').hide();
        Pass('tbCoatingJust', 'divOrder', 'divOrderError');
        //if (!$('#divMatJust').is(':visible')) {
        //    $('[id$=divJustBlock]').hide();
        //    $('[id$=hrJustBlock]').hide();
        }
 }

function ClearComments() {
    $('[id*=tbFocJust]').val('');
    $('[id*=tbMaterialJust]').val('');
    $('[id*=tbCoatingJust]').val('');
    var comm1 = $('[id*=tbComment1]');
    var comm2 = $('[id*=tbComment2]');

    if (comm1.val().indexOf('Opposite signs verified.') >= 0)
        $('[id*=tbComment1]').val('Opposite signs verified.');
    else
        $('[id*=tbComment1]').val('');

    if (comm2.val().indexOf('Opposite signs verified.') >= 0)
        $('[id*=tbComment2]').val('Opposite signs verified.');
    else
        $('[id*=tbComment2]').val('');

    if ($('[id$=ddlOrderPriority]').val() == "F")
        $('[id$=hfIsPrevOrderFOC]').val('Y');
}

function OrderRequiresFocJust() {
    if (IsDelete()) return;
    var elig = $('[id$=hfIsFocEligible]').val();
    var isPrevOrderFOC = $('[id$=hfIsPrevOrderFOC]').val();
    var val = $('[id$=hfIsFocFrame]').val();

    val = val == 'true' ? 'F' : 'N';

    var nextFocDate = Date.parse($('[id$=lblNextEligFoc]').text());
    var today = $.now()
    var reqJust = false;

    reqJust = val == "F" && today < nextFocDate ? true : false;
    if (elig == '' || elig == 'Y')
        reqJust = false;

    if (isPrevOrderFOC == 'Y')
        reqJust = true;

    if (reqJust) {
        if (!$('[id$=divJustBlock]').is(':visible')) {
            DoToggle('divJustBlock');
            $('[id$=hrJustBlock]').toggle();
        }
        if (!$('[id$=divFocJust]').is(':visible')) {
            $('[id$=divFocJust]').show();
        }
        DoGenericCommentVal('tbFocJust', 'FOC');
    }
    else {
        $('[id$=divFocJust]').hide();
        Pass('tbFocJust', 'divOrder', 'divOrderError');
        if (!$('#divMatJust').is(':visible')) {
            $('[id$=divJustBlock]').hide();
            $('[id$=hrJustBlock]').hide();
        }
    }

    EnableRejectRedirect()
}

function DeleteCheckToggle() {
    if (IsDelete()) {
        DoDeleteCheckAction();
    }
    else {
        DoDeleteUnCheckAction();
    }
}

function DoDeleteCheckAction() {
    ClearMsgs('divOrder', 'divOrderError');
    ToggleOrderUpdateButton('cbDelete');
}

function DoDeleteUnCheckAction() {
    ClearMsgs('divOrder', 'divOrderError');
    ToggleOrderUpdateButton('cbDelete');
    OrderSaveUpdateButtonClick(false);
}

function IsDelete() {
    return $('[id$=cbDelete]').is(':checked');
}

function DoStatusHistoryDialog() {
    var dialogOpts = {
        autoOpen: false,
        modal: true,
        width: '850',
        height: 'auto',
        title: 'Order Status Detail',
        dialogClass: 'generic',
        position: {
            my: 'center',
            at: 'center'
        }
    };
    var d = $("#divOrderStatusHistoryDialog").dialog(dialogOpts);
    d.dialog('open');
}

function DoStatusHistoryDialogClose() {
    var d = $("#divOrderStatusHistoryDialog").dialog('close');
}

function OrderSaveUpdateButtonClick(isSubmitButtonClick) {
    if ($('#cbReturnToStock').is(':checked')) { ClearMsgs('divOrder', 'divOrderError'); return true; }
    if ($('#cbDispense').is(':checked')) { ClearMsgs('divOrder', 'divOrderError'); return true; }
    if ($('#cbDelete').is(':checked')) { ClearMsgs('divOrder', 'divOrderError'); ToggleOrderUpdateButton('cbDelete'); return true; }

    var good = true;
    if (!ComboBoxVal(OrderPriority, 'divOrder', 'divOrderError', 'Order priority'))
        good = false;
    if (!ComboBoxVal(Lab, 'divOrder', 'divOrderError', 'Production lab'))
        good = false;
    if (!ComboBoxVal(Frame, 'divOrder', 'divOrderError', 'Frame'))
        good = false;
    if (!ComboBoxVal(Color, 'divOrder', 'divOrderError', 'Color'))
        good = false;
    if (!ComboBoxVal(Eye, 'divOrder', 'divOrderError', 'Eye'))
        good = false;
    if (!ComboBoxVal(Bridge, 'divOrder', 'divOrderError', 'Bridge'))
        good = false;
    if (!ComboBoxVal(Temple, 'divOrder', 'divOrderError', 'Temple'))
        good = false;
    if (!ComboBoxVal(Lens, 'divOrder', 'divOrderError', 'Lens'))
        good = false;
    if (!ComboBoxVal(Tint, 'divOrder', 'divOrderError', 'Tint'))
        good = false;
    if (!ComboBoxVal(Material, 'divOrder', 'divOrderError', 'Material'))
        good = false;
    if (!DoMaxPairVal('tbPair'))
        good = false;
    if (!OrderRequiresSegHt())
        good = false;
    if (OrderRequiresFocJust())
        good = false;
    if (OrderRequiresMatJust())
        good = false;
    if (OrderRequiresCoatingJust())
        good = false;

    // If requires Clinic justification...
    if ($('[id$=divRejectBlock]').is(':visible')) {
        good = DoGenericCommentVal('tbRejectClinicReply', 'Clinic');
    }

    if (!DoEmailAddressVal('tbOrderEmailAddress', 'Clinic')) {
        good = false;
    }
    // Used for submit and create new
    //if ($('[id$=ddlOrderPriority]').val() == "F") {
    //    $('[id$=hfIsPrevOrderFOC]').val('Y');
    //}
    //else {
    //    $('[id$=hfIsPrevOrderFOC]').val('N');
    //}

    EnableSaveIncompleteReprint771();

    if (!good && isSubmitButtonClick)
    {
        var message = $("#divOrderError").text();
        alert(message);
    }

    return good;
}

function DoPriorityFocus() {
    $('#' + OrderPriority).focus();
}

function DoFrameFocus() {
    $('#' + Frame).focus();
}

function EnableSaveIncompleteReprint771() {
    $('#' + IncompleteButton).removeAttr('disabled');
    if ($('[id$=bReprint771]').is(':visible'))
        $('[id$=bReprint771]').removeAttr('disabled');
}

//*********************************************************************
// LAB
//*********************************************************************

function EnableRejectRedirect() {
    $('[id$=bLabStatusChange]').removeAttr('disabled');
}

function DoStockReason() {
    // X, F, L, O
    var sVal = $('#ddlStockReason').find('option:selected').val();
    var msg = '';
    var pass = true;

    switch (sVal.toLowerCase()) {
        case 'o':
            msg = '';
            break;
        case 'x':
            pass = false;
            msg = '';
            break;
        case 'f':
            msg = 'Frame: ' + $('#' + Frame).val() + ' is unavailable.';
            break;
        case 'l':
            msg = 'Lens: ' + $('#' + Lens).val() + ' is unavailable.';
            break;
    }

    $('#tbLabJust').val(msg);

    if (!pass) return;
    Pass('ddlStockReason', 'divOrder', 'divOrderError');
}

function DoCheckinCheck() {
    if ($('#cbCheckInHfsOrder').is(':checked') == false) return;
    $('#bLabStatusChange').removeAttr('disabled');
    Pass('tbHfsDate', 'divOrder', 'divOrderError');
    Pass('tbLabJust', 'divOrder', 'divOrderError');
    Pass('ddlStockReason', 'divOrder', 'divOrderError');
}

//*********************************************************************
// END LAB
//*********************************************************************

function DoPriorityChanged() {
    $('#hdfShowOrderStep').val("1");
    ClearMsgs('divOrder', 'divOrderError');
    var good = ComboBoxVal(OrderPriority, 'divOrder', 'divOrderError', 'Order priority');
    OrderRequiresFocJust('tbFocJust');
    if (!IsDelete()) return good;
    DoDeleteCheckAction();
    return good;
}

function DoFrameChanged() {
    var good = ComboBoxVal(Frame, 'divOrder', 'divOrderError', 'Frame');
    OrderRequiresFocJust();
    OrderRequiresCoatingJust();
    EnableSaveIncompleteReprint771();
    if (!IsDelete()) return good;
    DoDeleteCheckAction();
    return good;
}

function ComboChangedGeneric(controlId, name) {
    var good = ComboBoxVal(controlId, 'divOrder', 'divOrderError', name);
    if (!IsDelete()) return good;
    DoDeleteCheckAction();
    return good;
}

function DoLensChanged() {
    SetLab(true);
    ComboBoxVal(Lab, 'divOrder', 'divOrderError', 'Lab');
    ToggleSegHt();
    ComboBoxVal(Lens, 'divOrder', 'divOrderError', 'Lens');
    DoFocus(Lens);
    if (!IsDelete()) return;
    DoDeleteCheckAction();
}

function DoLabChanged() {
    SetLab(false);
    ComboBoxVal(Lab, 'divOrder', 'divOrderError', 'Lab');
    DoFocus(Lab);
    if (!IsDelete()) return;
    DoDeleteCheckAction();
}

function SetLabComboboxState() {
    $("#ProdLab *").attr("disabled", "disabled").off('click'); //Aldela FS 382: Disable lab selection
    //if ($('#' + Lens).val() == null) return;
    
    //if ($('#' + Lens).val().substring(0, 2) == 'SV')
    //$('#' + Lab).next(':input').removeProp('disabled').next('button').removeProp('disabled');
    //else {
   //     $('#' + Lab).next(':input').prop('disabled', true).next('button').prop('disabled', true); 
    //}
}

function DoSaveReorder() {
    if ($('[id$=ddlReorderReason]').val() == 'X') {
        alert('A re-order reason is required.');
        return false;
    }

    var selReason = $('[id$=ddlReorderReason]').children(':selected').text();
    var other = $('[id$=tbOther]');
    var comm1 = $('[id$=tbComment1]');
    var comm2 = $('[id$=tbComment2]');

    if (selReason == 'OTHER' && (other.val().trim() == '' || other.val().trim().length < 5)) {
        alert('A comment is required with a minimum of 5 characters.');
        return false;
    }

    // Create the combined reason/reason text
    var comb = selReason + ': ' + other.val().trim();

    if (comb.length > 45) {
        alert('Combined reason cannot be more than 45 characters long including a system generated ": "');
        return false;
    }

    var tmp = '';
    // put the comment into comment 1 or 2
    if (comm1.val().length + comb.length <= 45) {
        comm1.val(comb + comm1.val());
    }
    else if (comm2.val().length + comb.length <= 45) {
        comm2.val(comb + comm2.val());
    }
    else if (comm1.val().indexOf('Opposite signs verified.') >= 0) {
        // Remove the opposite signs text.
        tmp = comm1.val().split('Opposite signs verified.').join('');
        if (tmp.length + comb.length <= 45)
            comm1.val(tmp + comb);
        else
            comm1.val(comb);
    }
    else if (comm2.val().indexOf('Opposite signs verified.') >= 0) {
        // Remove the opposite signs text.
        tmp = comm2.val().split('Opposite signs verified.').join('');
        if (tmp.length + comb.length <= 45)
            comm2.val(tmp + comb);
        else
            comm2.val(comb);
    }
    else {
        // overwrite comment1 with reorder reason
        comm1.val(comb);
    }

    var t =
    $.ajax({
        type: 'POST',
        url: 'OrderManagement.aspx/AjaxInsertOrder',
        data: '{comment1: "' + comm1.val() + '",comment2: "' + comm2.val() + '"}',
        contentType: 'application/json; charset-utf-8',
        dataType: 'json',
        success: function (response) {
            DoRedirect(response);
        },
        failure: function (response) {
            DoRedirect(response);
        }
    });

    $('#divReorder').dialog('close');
}


//*********************************************************************
// EXAMS
//*********************************************************************

function ExamSaveUpdateButtonClick() {
    var good = true;

    if (!DoIsNotFutureDate('tbExamDate'))
        good = false;

    if (!DdlHelper('ddlExamProviders', 'divExamData', 'divExamError', 'Provider name'))
        good = false;

    return good;
}

function DoAddExam() {
    $('[id$=ddlExamProviders]').val('X');
    $('[id$=ddlOdUncorrected]').val('');
    $('[id$=ddlOdCorrected]').val('20/20');
    $('[id$=ddlOsUnCorrected]').val('');
    $('[id$=ddlOsCorrected]').val('20/20');
    $('[id$=ddlOdOsUnCorrected]').val('');
    $('[id$=ddlOdOsCorrected]').val('20/20');
    $('[id$=tbComments]').val('');
    $('[id$=tbExamDate]').val(CurrentDate);
    $('[id$=bAddUpdateExam]').val('Save');
    $('[id$=hfButtonText]').val('Save');
    DoExamDialog();
}

function DoExamDialog() {
    var dOpts = {
        autoOpen: false,
        modal: true,
        width: 600,
        height: 500,
        title: 'Exam Information',
        dialogClass: 'generic',
        close: function () {
            $('#divExamData').dialog('destroy');
            $('#divExamData').hide();

            // Clear the error message queue
            ClearMsgs('divExamData', 'divExamError');
        }
    };

    var dExam = $('#divExamData').dialog(dOpts);
    dExam.parent().appendTo($('form:first'));
    dExam.dialog('open');
}

//*********************************************************************
// ADDRESS
//*********************************************************************

function CloseAddressDialog() {
    // This is also done in code behind on the save address button.
    $('#ddlShipTo').append('<option value="C2P">Clinic Ship to Patient</option>');
    $('#ddlShipTo').append('<option value="L2P">Lab Ship to Patient</option>');

    $('#divAddressDialog').dialog('close');
}

function DoAddressDialog() {
    var addressOpts = {
        autoOpen: false,
        modal: true,
        width: 625,
        height: 360,
        title: 'Verify Address',
        dialogClass: 'generic',
        close: function () {
            // Clear the error message queue
            ClearMsgs('divAddresses', 'divAddressMsg');

            $(this).dialog('destroy');
            $(this).hide();
        }
    };
    var addressDialog = $('#divAddressDialog').dialog(addressOpts);
    addressDialog.parent().appendTo($('form:first'));
    addressDialog.dialog('open');
}



//********************
//*************************************************
// PAGE EVENTS
//*********************************************************************

function pageLoad() {
    //var lab = $('#txtProdLab').text();
    //if (lab != "") {
    //    ShowBtnNextTop();
    //}


   // if (!isReprint) {
        DoAddressValidation();
   // }

    //$('#' + GroupDropDown).combobox();

    $('#' + OrderPriority).combobox(
    ).change(function () {
        DoPriorityChanged()
    }).removeAttr('tabindex').next().attr('tabindex', 4);

    $('#' + Frame).combobox(
    ).change(function () {
        DoFrameChanged();
    }).removeAttr('tabindex').next().attr('tabindex', 5);

    $('#' + Color).combobox(
    ).change(function () {
        ComboChangedGeneric(Color, 'Color');
    }).removeAttr('tabindex').next().attr('tabindex', 6);

    $('#' + Eye).combobox(
    ).change(function () {
        ComboChangedGeneric(Eye, 'Eye');
    }).removeAttr('tabindex').next().attr('tabindex', 7);

    $('#' + Bridge).combobox(
        ).change(function () {
            ComboChangedGeneric(Bridge, 'Bridge');
        }).removeAttr('tabindex').next().attr('tabindex', 8);

    $('#' + Temple).combobox(
        ).change(function () {
            ComboChangedGeneric(Temple, 'Temple');
        }).removeAttr('tabindex').next().attr('tabindex', 9);

    $('#' + Lens).combobox(
        ).change(function () {
            DoLensChanged();
        }).removeAttr('tabindex').next().attr('tabindex', 10);

    $('#' + Tint).combobox(
    ).change(function () {
        ComboChangedGeneric(Tint, 'Tint');
    }).removeAttr('tabindex').next().attr('tabindex', 11);


    //$('#' + Coating).input(
    //).change(function () {
    //    //ComboChangedGeneric(Coating, 'Coating');
    //}).removeAttr('tabindex').next().attr('tabindex', 11);





    $('#' + Material).combobox(
    ).change(function () {
        ComboChangedGeneric(Material, 'Material');
    }).removeAttr('tabindex').next().attr('tabindex', 12);

    //  $('#ddlShipTo').combobox().removeAttr('tabindex').next().attr('tabindex', 13);;
    $('#ddlShipTo').combobox().removeAttr('tabindex').next().attr('tabindex', 1);

    $('tbDispComment').focusout().removeAttr('tabindex').next().attr('tabindex', 2);

    $('#' + Lab).combobox(
        ).change(function () {
            DoLabChanged();
        }).removeAttr('tabindex').next().attr('tabindex', 14);

    ToggleSegHt();
    SetLabComboboxState();

    var c = $('#' + NextCombo).val();
    if (c == '') {
        c = $('#hfTmpNext').val();
        if (c == '') return;
    }
    $('#' + c).next(':input').select();
    $('#' + NextCombo).val('');
};

function SetIsReprint() {
    isReprint = true;
    return true;
}



//function pageLoad() {
//    $('#' + OrderPriority).combobox(
//    ).change(function () {
//        DoPriorityChanged()
//    }).removeAttr('tabindex').next().attr('tabindex', 1);

//    $('#' + Frame).combobox(
//    ).change(function () {
//        DoFrameChanged();
//    }).removeAttr('tabindex').next().attr('tabindex', 2);

//    $('#' + Color).combobox(
//    ).change(function () {
//        ComboChangedGeneric(Color, 'Color');
//    }).removeAttr('tabindex').next().attr('tabindex', 3);

//    $('#' + Eye).combobox(
//    ).change(function () {
//        ComboChangedGeneric(Eye, 'Eye');
//    }).removeAttr('tabindex').next().attr('tabindex', 4);

//    $('#' + Bridge).combobox(
//        ).change(function () {
//            ComboChangedGeneric(Bridge, 'Bridge');
//        }).removeAttr('tabindex').next().attr('tabindex', 5);

//    $('#' + Temple).combobox(
//        ).change(function () {
//            ComboChangedGeneric(Temple, 'Temple');
//        }).removeAttr('tabindex').next().attr('tabindex', 6);

//    $('#' + Lens).combobox(
//        ).change(function () {
//            DoLensChanged();
//        }).removeAttr('tabindex').next().attr('tabindex', 7);

//    $('#' + Tint).combobox(
//    ).change(function () {
//        ComboChangedGeneric(Tint, 'Tint');
//    }).removeAttr('tabindex').next().attr('tabindex', 8);

//    //$('#' + Coating).combobox(
//    //).change(function () {
//    //    ComboChangedGeneric(Coating, 'Coating');
//    //}).removeAttr('tabindex').next().attr('tabindex', 9);

//    $('#' + Material).combobox(
//    ).change(function () {
//        ComboChangedGeneric(Material, 'Material');
//    }).removeAttr('tabindex').next().attr('tabindex', 10);

//    $('#ddlShipTo').combobox().removeAttr('tabindex').next().attr('tabindex', 13);;

//    $('#' + Lab).combobox(
//        ).change(function () {
//            DoLabChanged();
//        }).removeAttr('tabindex').next().attr('tabindex', 14);

//    ToggleSegHt();
//    SetLabComboboxState();

//    var c = $('#' + NextCombo).val();
//    if (c == '') {
//        c = $('#hfTmpNext').val();
//        if (c == '') return;
//    }
//    $('#' + c).next(':input').select();
//    $('#' + NextCombo).val('');
//};

$(function () {
    $('#tbOdAdd').on('blur', function () {
        showHideExtraRxTypes();
    });
    $('#tbOsAdd').on('blur', function () {
        showHideExtraRxTypes();
    });

    function showHideExtraRxTypes() {
        if ($('#tbOdAdd').val() == '0.00' && $('#tbOsAdd').val() == '0.00')
            $('#divExtraRxs').css('display', 'none');
        else
            $('#divExtraRxs').css('display', 'block');

        if (!$('#cbMonoOrComboPd').is(':checked')) {
            DoComboPdVal('tbOdPdNearCombo');
            DoComboPdVal('tbOdPdDistCombo');
        }
        else {
            DoMonoPdVal('tbOdPdDistMono');
            DoMonoPdVal('tbOdPdNearMono');
            DoMonoPdVal('tbOsPdDistMono');
            DoMonoPdVal('tbOsPdNearMono');
        }
    }
});

///////////////////////////////////
// VALIDATION
///////////////////////////////////
var isForeign = '';
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
function DisplayPrescriptionDocumentDialog() {
    var PrescriptionDocumentDialog = document.getElementById('PrescriptionDocumentDialog');
    $("#PrescriptionDocumentDialog").fadeIn(10);
}

function IsValidEmailAddress() {
    var emailAddress = $('[id*=tbEmailAddress]').val();
    var regex = /^([a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]){1,70}$/;
    if ( regex.test(emailAddress) == false || emailAddress == '') {
        Fail('tbEmailAddress', 'divUpdateEmailAddress', 'divUpdateEmailAddressMsg', 'Email address is required or invalid.');
        return false;
    }
    else {
        Pass('tbEmailAddress', 'divUpdateEmailAddress', 'divUpdateEmailAddressMsg');
        return true;
    }

}

function UpdateEmailAddressClick() {
    var good = true;

    if (!IsValidEmailAddress())
        good = false;

    return good;
}


function IsDeletePrescritionScan() {
    document.getElementById('id_confrmdiv').style.display = "block";
    return false;

}