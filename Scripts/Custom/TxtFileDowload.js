
function RedirectClientDPUCCText() {
    var txtclientcode = $("#clientcode").val();
    var Exchangetype = $("#Exchange").val();
    if (txtclientcode == "") {
        $("#myModal").modal('show');
        $("#SpnMsg").html("Please Enter Client Code");
        
        return;
    }
    var dataValue = '{ "txtclientcode": "' + txtclientcode + '","Exchangetype" : "' + Exchangetype + '" }';
    $.ajax({
        type: "POST",
        url: "/Client/ExportToTextFile/",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: dataValue,
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

                alert(result.Message + "This ClientCode");
                
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


function DowloadCDSLFILE() {
    var txtclientcode = $("#clientcode").val();
    if (txtclientcode == "") {
        $("#myModal").modal('show');
        $("#SpnMsg").html("Please Enter Client Code");

        return;
    }

    var dataValue = '{ "clientCodeCDSL": "' + txtclientcode + '" }';
    $.ajax({
        type: "POST",
        url: "/Client/ExportTextFileCDSL/",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: dataValue,
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


function DowloadNSDLFILE() {
    var txtclientcode = $("#clientcode").val();
    if (txtclientcode == "") {
        $("#myModal").modal('show');
        $("#SpnMsg").html("Please Enter Client Code");

        return;
    }

    var dataValue = '{ "clientCodoNSDL": "' + txtclientcode + '" }';
    $.ajax({
        type: "POST",
        url: "/Client/ExporttextFileNSDL/",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: dataValue,
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


function clickBankfile() {
    var txtclientcode = $("#clientcode").val();
    if (txtclientcode == "") {
        $("#myModal").modal('show');
        $("#SpnMsg").html("Please Enter Client Code");

        return;
    }

    var dataValue = '{ "clientCode": "' + txtclientcode + '" }';
    $.ajax({
        type: "POST",
        url: "/Client/ExportBankFile/",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: dataValue,
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







