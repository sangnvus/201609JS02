﻿function Upload() {
    var url = $("#Upload").data('url');
    var formData = new FormData();
    var totalFiles = document.getElementById("slide").files.length;
    for (var i = 0; i < totalFiles; i++) {
        var file = document.getElementById("slide").files[i];
        formData.append("slide", file);
    }
    formData.append("desc", $("#description").val());
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


function Delete(id, i) {
    var url = $("#Delete").data('url');
    var deleteConfirm = confirm("Bạn có chắc chắn muốn xóa slide này không? ");
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
        })
    }
}

function Show(id) {
    var url = $("#Show").data('url');
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
            window.location.reload();
        },
        error: function (err) {
            alert(err.statusText);
        }
    })
}