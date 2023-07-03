function UploadSP() {
    debugger;
    if (window.FormData !== undefined) {
        var fileUpload = $("#fuSPM").get(0);
        var files = fileUpload.files;

        if (files.length == 0) {
            $("#SpnMsg").html('Please upload file');
            $('#myModal').modal();
            return;
        }

        // Create FormData object
        var fileData = new FormData();

        // Looping over all files and add it to FormData object
        for (var i = 0; i < files.length; i++) {
            fileData.append(files[i].name, files[i]);
        }

        $('#loading').show();

        $.ajax({
            url: '/Client/UploadSPFile',
            type: "POST",
            contentType: false, // Not to set any content header
            processData: false, // Not to process data
            data: fileData,
            success: function (result) {
                debugger;
                document.getElementById("fuSPM").value = null;

                if (result.Status == '-1') {
                    //alert("Your session has expired!");
                    $('#loading').hide();
                    location.href = result.Message;
                    return;
                }
                if (result.Status == '1') {
                    $('#loading').hide();
                    $("#SpnMsg").html('File Upload Successfully').css("color","green");
                    $('#myModal').modal();
                    return;
                }

                if (result.Status == '0') {
                    //debugger;
                    $('#loading').hide();
                    $("#SpnMsg").html(result.Message).css("color", "red");
                    $('#myModal').modal();
                    
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

function DownloadSampleFile() {
    $.ajax({
        type: "POST",
        url: "/Client/GenrateUsstextFile",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        
        beforeSend: function () {
            $('#loading').show();
        },
        success: function (result) {
            debugger;
            if (result.Status == -1) {
                window.location.href = result.url;
                return;
            }
            if (result.Status == "0") {

                alert(result.Message)

            }
            else if (result.Status == "1") {
                toastr.success(result.Message, "Success", { timeOut: 3000, "closeButton": true });
                window.location.href = "/Client/DownloadModificationText/?FileName=" + result.filename;
            }

        },
        complete: function () {
            $('#loading').hide();
        },
        error: function () {
            alert("ENTER VALIDE CLIENT CODE");
        }
    });
}



