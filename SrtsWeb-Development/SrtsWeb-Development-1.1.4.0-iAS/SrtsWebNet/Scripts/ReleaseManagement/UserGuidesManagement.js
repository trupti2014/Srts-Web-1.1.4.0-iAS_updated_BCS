/// <reference path="../Global/jquery-1.11.1.min.js" />
/// <reference path="../Global/PassFailConfirm.js" />

function CheckFileExists() {
    //Need to check if file name is empty or fileupload does not have the file
    var filename = $('input:file').val().split('\\').pop();
    var result = true;

    $.ajax({
        type: "POST",
        url: "ReleaseManagement.aspx/DoesFileExist",
        data: {},
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (data) {
            for (i in data.d) {
                if (data.d[i] == filename)
                {
                    if (confirm(filename + ' already exists.  Do you want to replace it?')) {
                        result = true;
                        return;
                    }
                    else {
                        result = false;
                        return;
                    }
                }
            }
           
        },
        failure: function (data) {
            alert("An error occurred.");
        }
    });
    return result;
}



