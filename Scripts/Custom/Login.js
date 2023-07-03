
function Login() {
    var data = {
        UserID: $("#UserID").val(),
        Password: $("#Password").val()
    }
    if ($("#UserID").val() == '' && $("#Password").val() == '') {
        $("#UserID").attr("required", "true").css({ "border": "1px solid Red", "box-shadow": "4px 3px #6acceb" });
        $("#Password").attr("required", "true").css({ "border": "1px solid Red", "box-shadow": "4px 3px #6acceb" });
        return;
    }
    if ($("#UserID").val() == '') {
        $("#UserID").attr("required", "true").css({ "border": "1px solid Red", "box-shadow": "4px 3px #6acceb" });
        $("#Password").removeAttr('style');
        return
    }
    if ($("#Password").val() == '') {
        $("#UserID").removeAttr('style');
        $("#Password").attr("required", "true").css({ "border": "1px solid Red", "box-shadow": "4px 3px #6acceb" });
        return;
    }
    /*$('#loading').show();*/
    $.ajax({
        url: "/Home/validateLogin",
        type: "POST",
        datatype: "JSON",
        contentType: "application/json;charset:utf-8",
        data: JSON.stringify(data),
        success: function (result) {
            debugger;
            if (result.status == "1") {
                /*$('#loading').hide();*/
                $("#msg").html(result.message).css("color", "green");
                window.location.href = result.url;
                return;
            }
            if (result.status == "0") {
                alert(result.message)
                /*$('#loading').hide();*/
                $("#UserID").val('')
                $("#Password").val('')

            }
        },
        error: function (result) {
            alert(result)

        }
    });

}