function uploadFileABUGN() {
    debugger;
    if (window.FormData !== undefined) {
        fileupload = $("#clientcode").get(0)
        var files = fileupload.files;

        if (files.length == 0) {
            $("#SpnMsg").html('Please upload file');
            $('#myModal').modal();
            return;
        }
        var fileData = new FormData()

        for (var i = 0; i < files.length; i++) {
            fileData.append(files[i].name, files[i]);
        }

       
        $.ajax({
            url: '/Client/UploadSPFile',
            type: "POST",
            contentType: false, // Not to set any content header
            processData: false, // Not to process data
            data: fileData,
            success: function (result) {
                debugger;
                /*document.getElementById("clientcode").value = null;*/

                if (result.Status == '-1') {
                    //alert("Your session has expired!");
                    $('#loading').hide();
                    location.href = result.Message;
                    return;
                }
              
            },
            error: function (err) {
                $('#loading').hide();
                $("#spnMsg").html('Something went wrong');
                $('#divPopupSPM').modal();
            }
        });
    }
    else {
        alert("FormData is not supported.");
    }
    
}