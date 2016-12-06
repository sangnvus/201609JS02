function PostReport() {
    console.log($("input[name=optradio]:checked").val());
    var formData = new FormData();
    var url = $("#buttonPost").data('url');
    var userId = $("#buttonPost").data('userid');
    var option = $("#buttonPost").data('option');
    console.log(userId);
    var beReportedId = $("#buttonPost").data('bereportedid');
    formData.append("reportContent", $("input[name=optradio]:checked").val());
    formData.append("userId", userId);
    formData.append("beReportedId", beReportedId);
    formData.append("option", option);
    $.ajax({
        type: "POST",
        url: url,
        data: formData,
        dataType: 'json',
        contentType: false,
        processData: false,

        success: function (result) {
            $("#reportMessage").html(result);
        }
    });
}

function Reject() {
    var formData = new FormData();
    var url = $("#RejectButton").data('url');
    var reportId = $("#RejectButton").data('reportid');
    formData.append("id", reportId);
    $.ajax({
        type: "POST",
        url: url,
        data: formData,
        dataType: 'json',
        contentType: false,
        processData: false,

        success: function (result) {
            window.location.reload();
        }
    });
}

$(document).ready(function () {
    $('#ModalReport').on('hidden.bs.modal', function (e) {
        console.log("123");
        $(this)
            .find("input,textarea,select,span")
                .val('')
                .end()
            .find("input[type=checkbox], input[type=radio]")
                .prop("checked", "")
                .end();
        $("#reportMessage").html("");
    })
});