function dowloadExcelFile() {
    debugger;
    var type = $("#btn").val();
    var dataValue = '{ "type": "' + type + '"}';

    $.ajax({
        type: "POST",
        url: "/ExcelFileDownload/ExcelFileGenrate",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function () {
            $('#loading').show();
        },
        success: function (result) {
            debugger;
            if (result.Status == "-1") {
                window.location.href = result.url;
                return;
            }
            if (result.Status == "0") {
                alert(result.Message);
            }
            else if (result.Status == "1") {
                toastr.success(result.Message, "Success", { timeOut: 3000, "closeButton": true });
                window.location.href = "/ExcelFileDownload/DownloadModificationText?FileName=" + result.filename;
            }
        },
        complete: function () {
            $('#loading').hide();
        },
        error: function (result) {
            debugger;
            alert("ENTER VALID CLIENT CODE");
        }
    });

}


//function downloadFile(filename) {
//    debugger;
//    var downloadUrl = "/ExcelFileDownload/DownloadModificationText/?FileName=" + filename;
//    var link = document.createElement("a");
//    link.href = downloadUrl;
//    link.target = "_blank";
//    document.body.appendChild(link);
//    link.click();
//    document.body.removeChild(link);
//}