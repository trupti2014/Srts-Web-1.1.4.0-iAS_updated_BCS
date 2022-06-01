/* Enable the continue order button once all the fields have
 * been filled out.
 */
var ValidateContinueOrderBtn = function () {
    var shippingAddress = document.getElementById("userShippingAddresses");
    var prescription = document.getElementById("userPrescriptions");
    var emailAddress = document.getElementById("userEmailAddresses");
    var hiddenFieldFrames = document.getElementById("MainContent_hfieldFramesSelected");
    var continueOrderBtn = document.getElementById("lbtnSubmitDetails");
    var glassesSelected = document.getElementById("userGlassesSelection");

    if (shippingAddress.options[shippingAddress.selectedIndex].value !== "0"
        && prescription.options[prescription.selectedIndex].value !== "0"
        && emailAddress.options[emailAddress.selectedIndex].value !== "0"
        && hiddenFieldFrames.value !== "") {
        continueOrderBtn.setAttribute("disabled", false);
        continueOrderBtn.disabled = false;
        if ((glassesSelected.value == "2" || glassesSelected.value == "4") && hiddenFieldFrames.value == "UPLC;REVISION UPLC INSERT") {
            continueOrderBtn.disabled = true;
            continueOrderBtn.setAttribute("disabled", true);
        }
    } else {
        continueOrderBtn.disabled = true;
        continueOrderBtn.setAttribute("disabled", true);
    }
}

ValidateAddFramesBtn = function () {
    var glassesSelected = document.getElementById("userGlassesSelection");
    var selectFramesBtn = document.getElementById("MainContent_btnDisplayAddGlasses");
    var hiddenFieldFrames = document.getElementById("MainContent_hfieldFramesSelected");
    var frameSelectionTxt = document.getElementById("MainContent_labelFieldFrameDescription");
    
    
    if (glassesSelected.selectedIndex == "1") {
        hiddenFieldFrames.value = glassesSelected.value;
        frameSelectionTxt.innerHTML = "";
    } else if (glassesSelected.selectedIndex == "0") {
        hiddenFieldFrames.value = "";
        frameSelectionTxt.innerHTML = "";
    }
    if (glassesSelected.selectedIndex == "2" || glassesSelected.selectedIndex == "4") {
        selectFramesBtn.Enabled = true
        selectFramesBtn.setAttribute("disabled", false);
        selectFramesBtn.disabled = false
        
    } else {
        selectFramesBtn.setAttribute("disabled", true);
        selectFramesBtn.disabled = true;
    }
    ValidateContinueOrderBtn();
}

ValidateAddFramesBtn();
ValidateContinueOrderBtn();
