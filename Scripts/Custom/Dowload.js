$(document).ready(function () {
    $("#downloadButton").click(function () {
        // Make an AJAX call to the action method
        $.ajax({
            url: "/Client/DownloadExcelFile",
            type: "GET",
            responseType: 'blob',
            success: function (data) {
                debugger;
                // Create a temporary link and trigger the download
                var a = document.createElement("a");
                var url = window.URL.createObjectURL(data);
                a.href = url;
                a.download = "example.xlsx";
                document.body.appendChild(a);
                a.click();
                document.body.removeChild(a);
                window.URL.revokeObjectURL(url);
            }
        });
    });
});
