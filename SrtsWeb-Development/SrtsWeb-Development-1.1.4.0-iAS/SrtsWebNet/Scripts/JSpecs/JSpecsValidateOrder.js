/*
 * Require user to click validate information to enable the user to submit an order.
 * 
*/
document.addEventListener("DOMContentLoaded", function () {
    var orderAddress = document.getElementById("orderAddress");
    var orderPrescription = document.getElementById("orderPrescription");
    var orderEmailAddress = document.getElementById("orderEmailAddress");
    var inputUserAgreementCheckbox = document.getElementById("inputUserAgreementCheckbox");
    var lbtnSubmitOrder = document.getElementById("lbtnSubmitOrder");
    var informationValidated = false;

    document.getElementById("lbtnVerifyInformation").addEventListener("click", validateInformation);
    inputUserAgreementCheckbox.addEventListener("click", EnableSubmitOrder);

    function validateInformation() {
        orderAddress.setAttribute("valid", "");
        orderPrescription.setAttribute("valid", "");
        orderEmailAddress.setAttribute("valid", "");

        informationValidated = true;

        EnableSubmitOrder();
    }

    function EnableSubmitOrder(){
        if (informationValidated && inputUserAgreementCheckbox.checked) {
            console.log("enabled");
            lbtnSubmitOrder.setAttribute("disabled", false);
            lbtnSubmitOrder.disabled = false;
            //lbtnSubmitOrder.disabled = "false";
        } else {
            console.log("disabled");
            lbtnSubmitOrder.disabled = true;
            lbtnSubmitOrder.setAttribute("disabled", true);
            //lbtnSubmitOrder.disabled = "true";
        }
    }
});