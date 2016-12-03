function UploadPublisher() {
    var url = $("#UploadPublisher").data('url');
    var formData = new FormData();
    var totalFiles = document.getElementById("pubImage").files.length;
    for (var i = 0; i < totalFiles; i++) {
        var pubImage = document.getElementById("pubImage").files[i];
        formData.append("pubImage", pubImage);
    }
    formData.append("pubName", $("#pubName").val());
    formData.append("pubAddress", $("#pubAddress").val());
    formData.append("pubPhoneNo", $("#pubPhoneNo").val());
    $.ajax({
        type: "POST",
        url: url,
        data: formData,
        dataType: 'json',
        contentType: false,
        processData: false,

        success: function (result) {
            alert(result);
            window.location.reload();
        },
        error: function (err) {
            alert(err.statusText);
        }
    });
}

function DeletePublisher(id, i) {
    var url = $("#DeletePublisher").data('url');
    var deleteConfirm = confirm("Bạn có chắc chắn muốn xóa nhà xuất bản này? ");
    if (deleteConfirm) {
        var formData = new FormData();
        formData.append("id", id);
        $.ajax({
            type: "POST",
            url: url,
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,

            success: function (result) {
                alert(result);
                $('table tbody tr#' + i).fadeOut('slow', function () { $(this).remove(); })
            },
            error: function (err) {
                alert(err.statusText);
            }
        });
    }
}