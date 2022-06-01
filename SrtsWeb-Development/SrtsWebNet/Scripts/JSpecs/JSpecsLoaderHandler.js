// Get the instance of PageRequestManager.
var prm = Sys.WebForms.PageRequestManager.getInstance();
// Add initializeRequest and endRequest
prm.add_initializeRequest(prm_InitializeRequest);
prm.add_endRequest(prm_EndRequest);

// Called when async postback begins
function prm_InitializeRequest(sender, args) {
    // get the divImage and set it to visible
    var panelProg = $get('loader');
    panelProg.style.display = '';

    // Disable button that caused a postback
    $get(args._postBackElement.id).disabled = true;
}

// Called when async postback ends
function prm_EndRequest(sender, args) {
    // get the divImage and hide it again
    var panelProg = $get('loader');
    panelProg.style.display = 'none';

    // Enable button that caused a postback
    //sender._postBackSettings.sourceElement.id.disabled
    var sourceElement = $get(sender._postBackSettings.sourceElement.id);
    if (sourceElement != null) {
        sourceElement.disabled = false;
    }
}