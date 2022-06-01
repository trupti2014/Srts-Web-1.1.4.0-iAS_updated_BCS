/// <reference path="../Global/jquery-1.11.1.min.js" />

$(function () {
    var AP = $('#hfpanel').val();
    resetPanels();
    if (AP == '' || AP == '0') {
        showFrameImagesPanel();
    }
    else if (AP == '1') {

        showFrameImagesEditPanel();
    }


    $('#lnkFrameImages').click(function () {
        hidePanels();
        clearActive();
        showFrameImagesPanel()
        resetFormFields();
        __doPostBack('ddlFrameFamily', '');
        return false;
    });

    $('#lnkFrameImagesEdit').click(function () {
        hidePanels();
        clearActive();
        showFrameImagesEditPanel()
        resetFormFieldsEdit();
        __doPostBack('ddlFrameFamilyEdit', '');
        return false;
    });
});
function onMyFrameLoad() {
    alert('myframe is loaded');
};



function resetPanels() {
    hidePanels();
    clearActive();
    hideLinks();
}


function hidePanels() {
    resetFormFields();
    $('#pnlFrameImages').hide();
    $('#pnlFrameImagesEdit').hide();

};


function hideLinks() {
    $('#lnkFrameImages').hide();
    $('#lnkFrameImagesEdit').hide();

};


function clearActive() {
    $("#lstFrameImages").removeClass("active");
    $("#lstFrameImagesEdit").removeClass("active");
};
function showFrameImagesPanel() {
    $('#pnlFrameImages').show();
    $('#lnkFrameImages').show();
    $('#lnkFrameImagesEdit').show();
    $("#lstFrameImages").addClass("active");
}
function showFrameImagesEditPanel() {
    $('#pnlFrameImagesEdit').show();
    $('#lnkFrameImages').show();
    $('#lnkFrameImagesEdit').show();
    $("#lstFrameImagesEdit").addClass("active");
}

function resetFormFields() {
    var frameFamily = $('#hdfFrameFamily').val();

    if (!frameFamily == 0) {
        $('#ddlFrameFamily').prop('selectedIndex', frameFamily);  // on ucManageFrames
        $('#hdfFrameFamily').val('0');
    }
    else {
        $('#ddlFrameFamily').prop('selectedIndex', 0);  // on ucManageFrames
    }
}

function resetFormFieldsEdit() {
    $('#ddlFrameFamilyEdit').prop('selectedIndex', 0);  // on ucManageFramesEdit
}

function DisplayFrameImageDialog() {
    var FrameImageDialog = document.getElementById('FrameImageDialog');
    FrameImageDialog.style.display = 'block';
    $("#FrameImageDialog").fadeIn(10);
}



// This is used to capture the partial postback and determine if the success dialog should be shown.
Sys.Application.add_load(function () {
    var ImageSuccess = $('#hfSuccessFrames').val();
    var msg = $('#hfMsgFrames').val();
    if ((ImageSuccess == '' || ImageSuccess == '0') && msg != '') {
        displaySrtsMessage('Error!', msg, 'error');
    }
    else if (ImageSuccess == '1') {
        // Call function to show message.
        displaySrtsMessage('Success!', msg, 'success');
    }

    // Reset the success hidden field to 0 so not to re-show the success dialog on the next partial postback.
    $('#hfSuccessFrames').val('0');
    $('#hfMsgFrames').val('');
});

