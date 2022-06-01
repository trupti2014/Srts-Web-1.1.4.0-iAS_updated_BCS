function PDRequiredSet() {
    // One PD or Two PD only, can't be both
    // at least ONE Or TWO PD is required
    const pdOne = document.getElementById('MainContent_onePDDD');
    const pdTwoRight = document.getElementById('MainContent_twoPDRightDD');
    const pdTwoLeft = document.getElementById('MainContent_twoPDLeftDD');
    //
    // PD one is set
    if (pdOne.options[pdOne.selectedIndex].value !== "0") {
        return true;
    } else if (pdTwoRight.options[pdTwoRight.selectedIndex].value !== "0" && pdTwoLeft.options[pdTwoLeft.selectedIndex].value !== "0") {
        // else PD Two are set
        return true;
    }
    return false;
}

function prescriptionFieldsSet(rightCylinder, leftCylinder) {
    const rightSphere = document.getElementById('MainContent_rightSphereDD');
    const leftSphere = document.getElementById('MainContent_leftSphereDD');
    if (rightSphere.options[rightSphere.selectedIndex].value !== "null" && leftSphere.options[leftSphere.selectedIndex].value !== "null" &&
        rightCylinder.options[rightCylinder.selectedIndex].value !== "null" && leftCylinder.options[leftCylinder.selectedIndex].value !== "null") {
        return true;
    }
    return false;
}

function axisIsSet(isValid,cylinder,axis) {
    if (cylinder.options[cylinder.selectedIndex].value === "0") {
        if (axis.selectedIndex !== 0) {
            isValid = false;
        }
    } else {
        // Axis is required
        if (axis.selectedIndex !== 0) {
            isValid = true;
        }
    }
    return isValid;
}

function validateForm() {
    
    const rxDate = document.getElementById('MainContent_tbPrescriptionDate');
    const rightCylinder = document.getElementById('MainContent_rightCylinderDD');
    const leftCylinder = document.getElementById('MainContent_leftCylinderDD');
    const rightAxis = document.getElementById('MainContent_rightAxisDD');
    const leftAxis = document.getElementById('MainContent_leftAxisDD');
    const rxPicture = document.getElementById('choosePrescription');
    const btnAddPrescription = document.getElementById('MainContent_btnAddPrescription');
    var isValidated = false;
    // required fields
    if (rxDate.value !== "" && prescriptionFieldsSet(rightCylinder, leftCylinder) && PDRequiredSet() && rxPicture.value !== "") {
        isValidated = axisIsSet(true, rightCylinder, rightAxis);
        if (isValidated) {
            isValidated = axisIsSet(isValidated, leftCylinder, leftAxis); 
        }
        
    }
    if (isValidated) {
        btnAddPrescription.removeAttribute('disabled');
        return true;
    } else {
        event.preventDefault();
        return false;
    }


}

function autoPopulateOSFromOD(event) {
    // OS Add should autopopulate with OD Add value
    const target = event.target;
    const rightAdd = document.getElementById('MainContent_rightAddDD');
    const leftAdd = document.getElementById('MainContent_leftAddDD');
    if (target.id === 'MainContent_rightAddDD') {
        leftAdd.selectedIndex = target.selectedIndex;
    } else {
        rightAdd.selectedIndex = target.selectedIndex;
    }
    validateForm();
}

function resetOnePD() {
    const onePD = document.getElementById('MainContent_onePDDD');
    onePD.selectedIndex = 0;
    validateForm();
}

function resetTwoPDs() {
    const rightPD = document.getElementById('MainContent_twoPDRightDD');
    const leftPD = document.getElementById('MainContent_twoPDLeftDD');
    rightPD.selectedIndex = 0;
    leftPD.selectedIndex = 0;
    validateForm();
}

function resetAxisIfCylIsZero(event) {
    const rightCyl = document.getElementById('MainContent_rightCylinderDD');
    const leftCyl = document.getElementById('MainContent_leftCylinderDD');
    if (event.target.id === 'MainContent_rightAxisDD') {
        if (rightCyl.options[rightCyl.selectedIndex].value === "0") {
            event.target.selectedIndex = 0;
            event.target.value = "0";
        }
    } else {
        if (leftCyl.options[leftCyl.selectedIndex].value === "0") {
            event.target.selectedIndex = 0;
            event.target.value = "0";
        }
    }
    validateForm();
}

function noAxisIfCylIsZero(event) {
    const rightAxis = document.getElementById('MainContent_rightAxisDD');
    const leftAxis = document.getElementById('MainContent_leftAxisDD');
    if (event.target.value == "0") {
        if (event.target.id === 'MainContent_rightCylinderDD') {
            rightAxis.selectedIndex = 0;
            rightAxis.disabled = true;
        } else {
            leftAxis.selectedIndex = 0;
            leftAxis.disabled = true;
        }
    } else {
        if (event.target.id === 'MainContent_rightCylinderDD') {
            rightAxis.disabled = false;
        } else {
            leftAxis.disabled = false;
        }
    }
    validateForm();
}

function convertToBase64(file) {
    const reader = new FileReader();
    reader.onloadend = function (event) {
        const img = new Image();
        img.src = event.target.result;

        //console.log("IMAGE ", img.src)
        img.onload = function (ev) {
            const natWidth = (ev.hasOwnProperty('path')) ? ev.path[0].naturalWidth : ev.target.naturalWidth;
            const natHeight = (ev.hasOwnProperty('path')) ? ev.path[0].naturalHeight : ev.target.naturalHeight;
            const isLandscape = (natWidth > natHeight);
            const multiplier = (isLandscape) ? natHeight / natWidth : natWidth / natHeight;
            const canvas = document.createElement('canvas');
            const ctx = canvas.getContext('2d');
            const previewImg = document.getElementById('previewImageRX');
            const hiddenField = document.getElementById('MainContent_hfieldPrescriptionImageSelected');

            img.width = (isLandscape) ? 600 : 600 * multiplier;
            img.height = (isLandscape) ? 600 * multiplier : 600;

            ctx.clearRect(0, 0, canvas.width, canvas.height);

            canvas.width = img.width;
            canvas.height = img.height;

            ctx.drawImage(img, 0, 0, img.width, img.height);
            const dataURLStr = canvas.toDataURL('img/png');
            hiddenField.value = dataURLStr;
            previewImg.src = dataURLStr;
            validateForm();
        }
    }
    reader.readAsDataURL(file);
}

function addPrescriptionPhoto(event) {

    
    convertToBase64(event.target.files[0]);
    console.log("EVENT TARGET FILES ", event.target.files)
    const hiddenName = document.getElementById('MainContent_hfieldPrescriptionImageName');
    hiddenName.value = event.target.files[0].name;

}
