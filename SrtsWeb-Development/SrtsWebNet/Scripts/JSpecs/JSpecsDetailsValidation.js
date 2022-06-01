/* Enable the continue order button once all the fields have
 * been filled out.
 */
var ValidateContinueOrderBtn = function () {
    var shippingAddress = document.getElementById("userShippingAddresses");
    var prescription = document.getElementById("userPrescriptions");
    var emailAddress = document.getElementById("userEmailAddresses");
    var continueOrderBtn = document.getElementById("lbtnSubmitDetails");

    if (shippingAddress.options[shippingAddress.selectedIndex].value !== "0" && prescription.options[prescription.selectedIndex].value !== "0" && emailAddress.options[emailAddress.selectedIndex].value !== "0") {
        continueOrderBtn.setAttribute("disabled", false);
        continueOrderBtn.disabled = false;
    } else {
        continueOrderBtn.disabled = true;
        continueOrderBtn.setAttribute("disabled", true);
    }
}

ValidateContinueOrderBtn();
