/// <reference path="Scripts/jquery-1.11.1.min.js"/>

var sideChanged = null;

$(document).ready(function () {
    var OD = $('[id*=lboxODAdd] option:selected').val();
    var OS = $('[id*=lboxOSAdd] option:selected').val();

    if (!((OD == "0.00") && (OS == "0.00"))) {
        sideChanged = "done";
    }

    $(".listbox, .listboxBottom").each(function () {
        if ((this.id.search(/prism/i) != -1) || (this.id.search(/base/i) != -1)) {
            $(this).prop("enabled", false);
            $(this).prop("disabled", true);
        }
    });
    loadCalcValues();
    setVertPos();
});

//function pageLoad(e)
//{
//    setPDFields();
//}

function setVertPos() {
    var y = screen.height * 0.1
    window.scrollTo(0, y);
}

function loadCalcValues() {
    SetODCalcValues();
    SetOSCalcValues();
    displayCalcValues();
}

function ChkSphereSigns() {
    var sphOD = $('[id*=lboxODSphere] option:selected').val().substring(0, 1);
    var sphOS = $('[id*=lboxOSSphere] option:selected').val().substring(0, 1);

    if ((sphOD == "+" && sphOS == "-") || (sphOD == "-" && sphOS == "+")) {
        alert('Sphere values have opposite signs.');
    }
}

function ChkCylinderSigns() {
    var cylOD = $('[id*=lboxODCylinder] option:selected').val().substring(0, 1);
    var cylOS = $('[id*=lboxOSCylinder] option:selected').val().substring(0, 1);

    if ((cylOD == "+" && cylOS == "-") || (cylOD == "-" && cylOS == "+")) {
        alert('Cylinder values have opposite signs.');
    }
}

function displayCalcValues() {
    var cylOD = $('[id*=lboxODCylinder] option:selected').val().substring(0, 1);
    var cylOS = $('[id*=lboxOSCylinder] option:selected').val().substring(0, 1);

    if (cylOD == "+" || cylOD == "0" || cylOS == "+" || cylOS == "0") {
        $('#CalcValuesDiv').fadeIn(500);
    }
    else {
        $('#CalcValuesDiv').fadeOut(500);
    }
}

function ODSphere() {
    SetODCalcValues();
    ChkSphereSigns();
}

function ODCylinder(ddl) {
    SetODCalcValues();
    displayCalcValues();
    ChkCylinderSigns();
}

function ODAxis() {
    SetODCalcValues();
}

function SetODCalcValues() {
    var sph = parseFloat(removePlusSign($('[id*=lboxODSphere] option:selected').val()));
    var cyl = parseFloat(removePlusSign($('[id*=lboxODCylinder] option:selected').val()));
    var axis = 1 * $('[id*=lboxODAxis] option:selected').val();

    var sphCalc = "";
    var cylCalc = "";
    var axisCalc = "";

    if (cyl > 0) {
        cylCalc = (-1 * cyl).toFixed(2);

        if (axis > 90) {
            axisCalc = axis - 90;
        }
        else {
            axisCalc = axis + 90;
        }

        if ((sph + cyl) > 0) {
            sphCalc = "+" + (sph + cyl).toFixed(2);
        }
        else if ((sph + cyl) == 0) {
            sphCalc = "Plano";
        }
        else {
            sphCalc = (sph + cyl).toFixed(2);
        }
    }
    else if (cyl < 0) {
        if (sph == 0) {
            sphCalc = "Plano";
        }
        else if (sph > 0) {
            sphCalc = "+" + sph.toFixed(2);
        }
        else {
            sphCalc = sph.toFixed(2);
        }

        if (axis == 0) {
            axisCalc = "Unknown";
        }
        else {
            axisCalc = axis;
        }

        cylCalc = cyl.toFixed(2);
    }
    else if (cyl == 0) {
        if (sph == 0) {
            sphCalc = "Plano";
        }
        else if (sph > 0) {
            sphCalc = "+" + sph.toFixed(2);
        }
        else {
            sphCalc = sph.toFixed(2);
        }

        $('[id*=lboxODAxis]').val("000");
        cylCalc = "Sphere";
        axisCalc = "N/A";
    }

    if (sphCalc != "Plano") {
        sph = parseFloat(sphCalc);
    }
    else {
        sph = 0;
    }

    if (cylCalc != "Sphere") {
        cyl = parseFloat(cylCalc);
    }
    else {
        cyl = 0;
    }

    var axisCalcPadded = "";
    if (axisCalc == "N/A" || axisCalc == "Unknown") {
        axisCalcPadded = axisCalc;
    }
    else {
        axisCalcPadded = String("000" + axisCalc).slice(-3);
    }

    $('[id*=lblODSphere_calc]').text(sphCalc);
    $('[id*=hfODSphereCalc]').val(sph);
    $('[id*=lblODCylinder_calc]').text(cylCalc);
    $('[id*=hfODCylinderCalc]').val(cyl);
    $('[id*=lblODAxis_calc]').text(axisCalcPadded);
    $('[id*=hfODAxisCalc]').val(axisCalc);
}

function OSSphere() {
    SetOSCalcValues();
    ChkSphereSigns();
}

function OSCylinder(ddl) {
    SetOSCalcValues();
    displayCalcValues();
    ChkCylinderSigns();
}

function OSAxis() {
    SetOSCalcValues();
}

function SetOSCalcValues() {
    var sph = parseFloat(removePlusSign($('[id*=lboxOSSphere] option:selected').val()));
    var cyl = parseFloat(removePlusSign($('[id*=lboxOSCylinder] option:selected').val()));
    var axis = 1 * $('[id*=lboxOSAxis] option:selected').val();

    var sphCalc = "";
    var cylCalc = "";
    var axisCalc = "";

    if (cyl > 0) {
        cylCalc = (-1 * cyl).toFixed(2);

        if (axis > 90) {
            axisCalc = axis - 90;
        }
        else {
            axisCalc = axis + 90;
        }

        if ((sph + cyl) > 0) {
            sphCalc = "+" + (sph + cyl).toFixed(2);
        }
        else if ((sph + cyl) == 0) {
            sphCalc = "Plano";
        }
        else {
            sphCalc = (sph + cyl).toFixed(2);
        }
    }
    else if (cyl < 0) {
        if (sph == 0) {
            sphCalc = "Plano";
        }
        else if (sph > 0) {
            sphCalc = "+" + sph.toFixed(2);
        }
        else {
            sphCalc = sph.toFixed(2);
        }

        if (axis == 0) {
            axisCalc = "Unknown";
        }
        else {
            axisCalc = axis;
        }

        cylCalc = cyl.toFixed(2);
    }
    else if (cyl == 0) {
        if (sph == 0) {
            sphCalc = "Plano";
        }
        else if (sph > 0) {
            sphCalc = "+" + sph.toFixed(2);
        }
        else {
            sphCalc = sph.toFixed(2);
        }
        $('[id*=lboxOSAxis]').val("000");
        cylCalc = "Sphere";
        axisCalc = "N/A";
    }

    if (sphCalc != "Plano") {
        sph = parseFloat(sphCalc);
    }
    else {
        sph = 0;
    }

    if (cylCalc != "Sphere") {
        cyl = parseFloat(cylCalc)
    }
    else {
        cyl = 0;
    }

    var axisCalcPadded = "";
    if (axisCalc == "N/A" || axisCalc == "Unknown") {
        axisCalcPadded = axisCalc;
    }
    else {
        axisCalcPadded = String("000" + axisCalc).slice(-3);
    }

    $('[id*=lblOSSphere_calc]').text(sphCalc);
    $('[id*=hfOSSphereCalc]').val(sph);
    $('[id*=lblOSCylinder_calc]').text(cylCalc);
    $('[id*=hfOSCylinderCalc]').val(cyl);
    $('[id*=lblOSAxis_calc]').text(axisCalcPadded);
    $('[id*=hfOSAxisCalc]').val(axisCalc);
}

function removePlusSign(val) {
    var num = val.replace("+", "");
    return num;
}

function setAddPower(side) {
    var AddOD = $('[id*=lboxODAdd] option:selected').val();
    var AddOS = $('[id*=lboxOSAdd] option:selected').val();

    if (sideChanged == null) {
        if (side == "OD") {
            $('[id*=lboxOSAdd]').val(AddOD);
        }
        else if (side == "OS") {
            $('[id*=lboxODAdd]').val(AddOS);
        }
        sideChanged = side;
    }
    else if (sideChanged == side) {
        if (side == "OD") {
            $('[id*=lboxOSAdd]').val(AddOD);
        }
        else if (side == "OS") {
            $('[id*=lboxODAdd]').val(AddOS);
        }
    }
    else {
        sideChanged = "done";
    }
}

function setPDFieldsToDisplay() {
    var rblPDMode = $("[id*=rblPDMode] input:checked");
    var rblPDModevalue = rblPDMode.val();

    if (rblPDModevalue == "T") {
        $.each($('.PDTotal'), function () {
            $(this).show();
        });
        $.each($('.PDMono'), function () {
            if ($(this).is(":visible"))
                $(this).hide();
        });
    }
    else {
        $.each($('.PDTotal'), function () {
            if ($(this).is(":visible"))
                $(this).hide();
        });
        $.each($('.PDMono'), function () {
            $(this).show();
        });
    }
}

function setPDNear(mode) {
    if (mode == 'T') {
        var PD = $('[id*=lboxPDTotal] option:selected').val();
        if (PD == 'X')
            $('[id*=lboxPDTotalNear]').val('X');
        else
            if (PD < 55) {
                $('[id*=lboxPDTotalNear]').val(52);
            }
            else {
                $('[id*=lboxPDTotalNear]').val(PD - 3);
            }
    }
    else {
        var PDOD = $('[id*=lboxPDOD] option:selected').val();
        var PDOS = $('[id*=lboxPDOS] option:selected').val();

        if (PDOD == 'X')
            $('[id*=lboxPDODNear]').val('X');
        else
            if (PDOD < 27.5) {
                $('[id*=lboxPDODNear]').val(26);
            }
            else {
                $('[id*=lboxPDODNear]').val(PDOD - 1.5);
            }
        if (PDOS == 'X')
            $('[id*=lboxPDOSNear]').val('X');
        else
            if (PDOS < 27.5) {
                $('[id*=lboxPDOSNear]').val(26);
            }
            else {
                $('[id*=lboxPDOSNear]').val(PDOS - 1.5);
            }
    }
}

function setODHPrism() {
    var ODHPrism = $('[id*=lboxODHPrism] option:selected').val();
    $('[id*=hfODHPrism]').val(ODHPrism);
}

function setODHBase() {
    var ODHBase = $('[id*=ddlODHBase] option:selected').val();
    $('[id*=hfODHBase]').val(ODHBase);
}

function setODVPrism() {
    var ODVPrism = $('[id*=this.lboxODVPrism] option:selected').val();
    $('[id*=hfODVPrism]').val(ODVPrism);
}

function setODVBase() {
    var ODVBase = $('[id*=this.ddlODVBase] option:selected').val();
    $('[id*=hfODVBase]').val(ODVBase);
}

function setOSHPrism() {
    var OSHPrism = $('[id*=lboxOSHPrism] option:selected').val();
    $('[id*=hfOSHPrism]').val(OSHPrism);
}

function setOSHBase() {
    var OSHBase = $('[id*=ddlOSHBase] option:selected').val();
    $('[id*=hfOSHBase]').val(OSHBase);
}

function setOSVPrism() {
    var OSVPrism = $('[id*=lboxOSVPrism] option:selected').val();
    $('[id*=hfOSVPrism]').val(OSVPrism);
}

function setOSVBase() {
    var OSVBase = $('[id*=ddlOSVBase] option:selected').val();
    $('[id*=hfOSVBase]').val(OSVBase);
}

function getPrismVals() {
    var ODHPrism = $('[id*=lboxODHPrism] option:selected').val();
    var ODHBase = $('[id*=ddlODHBase] option:selected').val();
    var ODVPrism = $('[id*=lboxODVPrism] option:selected').val();
    var ODVBase = $('[id*=ddlODVBase] option:selected').val();
    var OSHPrism = $('[id*=lboxOSHPrism] option:selected').val();
    var OSHBase = $('[id*=ddlOSHBase] option:selected').val();
    var OSVPrism = $('[id*=lboxOSVPrism] option:selected').val();
    var OSVBase = $('[id*=ddlOSVBase] option:selected').val();

    $("[id*=hfODHPrism]").val(ODHPrism);
    $("[id*=hfODHBase]").val(ODHBase);
    $("[id*=hfODVPrism]").val(ODVPrism);
    $("[id*=hfODVBase]").val(ODVBase);
    $("[id*=hfOSHPrism]").val(OSHPrism);
    $("[id*=hfOSHBase]").val(OSHBase);
    $("[id*=hfOSVPrism]").val(OSVPrism);
    $("[id*=hfOSVBase]").val(OSVBase);
}

function enablePrism() {
    var enabled = $("[id*=cbEnablePrism]").is(':checked')
    $(".listbox, .listboxBottom").each(function () {
        if ((this.id.search(/prism/i) != -1) || (this.id.search(/base/i) != -1)) {
            $(this).prop("enabled", enabled);
            $(this).prop("disabled", !enabled);
        }
    });

    if (!enabled) {
        $("[id*=lboxODHPrism]").val("0.00");
        $("[id*=ddlODHBase]").val("");
        $("[id*=lboxODVPrism]").val("0.00");
        $("[id*=ddlODVBase]").val("");
        $("[id*=lboxOSHPrism]").val("0.00");
        $("[id*=ddlOSHBase]").val("");
        $("[id*=lboxOSVPrism]").val("0.00");
        $("[id*=ddlOSVBase]").val("");
    }
}