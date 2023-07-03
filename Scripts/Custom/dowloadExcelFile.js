
function dowloadExcel() {
    Event.preventDefault();
    debugger;
    var form = $("#onlineform").serialize();
    $.post({
        url: "/Client/Dowload",
        data: form,
    });
}