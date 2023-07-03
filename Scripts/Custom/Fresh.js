function SearchIPO() {
    debugger;
    var clientCode = $("#search").val();
    var type = $('#dropdown').val()

    if (clientCode == "" && type == "") {

        $("#myModal").modal('show');
        $("#SpnMsg").html("Please Enter Client Code");
        return;
    }
    if (clientCode == "") {
        $("#myModal").modal('show');
        $("#SpnMsg").html("Please Enter Client Code");
        return;
    }
    if (type == "") {
        $("#myModal").modal('show');
        $("#SpnMsg").html("Please Select  Dropdwonlist");
        return;
    }
    $("#row1").css("display", "block");
    $("#spnMsg").html('');
    var dataValue = '{ "ClientCode": "' + clientCode + '" ,"type" : "' + type + '"}';
    $.ajax({
        type: "POST",
        url: "/Client/FreshTables/",
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
            $("#tableDiv").empty();
            $("#tableDiv").append(result.htmlcontent);

        },
        complete: function () {
            $('#loading').hide();
        }
    });
}


