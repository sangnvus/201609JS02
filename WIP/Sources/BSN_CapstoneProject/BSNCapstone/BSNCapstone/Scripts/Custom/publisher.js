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
            document.getElementById('pubname-error').innerHTML = null;
            document.getElementById('pubName').style.border = "1px solid #ccc";
            document.getElementById('pubName').style.borderRadius = "4px";
            document.getElementById('pubaddress-error').innerHTML = null;
            document.getElementById('pubAddress').style.border = "1px solid #ccc";
            document.getElementById('pubAddress').style.borderRadius = "4px";
            document.getElementById('pubimg-error').innerHTML = null;
            $.each(result, function (index, x) {
                if (x == "Tên NXB không được để trống") {
                    document.getElementById('pubname-error').innerHTML = x;
                    document.getElementById('pubName').style.border = "1px solid red";
                }
                if (x == "Địa chỉ không được để trống") {
                    document.getElementById('pubaddress-error').innerHTML = x;
                    document.getElementById('pubAddress').style.border = "1px solid red";
                }
                if (x == "Ảnh NXB không được để trống") {
                    document.getElementById('pubimg-error').innerHTML = x;
                }
                if (x == "Tên NXB đã tồn tại") {
                    document.getElementById('pubname-error').innerHTML = x;
                    document.getElementById('pubName').style.border = "1px solid red";
                }
                if (result.success == x) {
                    alert(x);
                    window.location.reload();
                }
            });
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

function EditPublisher(pubId) {
    var url = $("#EditPublisher").data('url');
    var formData = new FormData();
    var totalFiles = document.getElementById("pubEditedImage").files.length;
    // HuyenPT. Update. Start. 06-12-2016
    /*for (var i = 0; i < totalFiles; i++) {
        var pubImage = document.getElementById("pubEditedImage").files[i];
        formData.append("pubEditedImage", pubImage);
    }*/
    var pubEditedImage = totalFiles > 0 ? document.getElementById("pubEditedImage").files[0] : null;
    formData.append("pubEditedImage", pubEditedImage);
    // HuyenPT. Update. End. 06-12-2016
    formData.append("pubId", pubId);
    formData.append("pubEditedName", $("#pubEditedName").val());
    formData.append("pubEditedAddress", $("#pubEditedAddress").val());
    formData.append("pubEditedPhoneNo", $("#pubEditedPhoneNo").val());
    $.ajax({
        type: "POST",
        url: url,
        data: formData,
        dataType: 'json',
        contentType: false,
        processData: false,

        success: function (result) {
            document.getElementById('pubeditedname-error').innerHTML = null;
            document.getElementById('pubEditedName').style.border = "1px solid #ccc";
            document.getElementById('pubEditedName').style.borderRadius = "4px";
            document.getElementById('pubeditedaddress-error').innerHTML = null;
            document.getElementById('pubEditedAddress').style.border = "1px solid #ccc";
            document.getElementById('pubEditedAddress').style.borderRadius = "4px";
            document.getElementById('pubeditedimg-error').innerHTML = null;
            $.each(result, function (index, x) {
                if (x == "Tên NXB không được để trống") {
                    $("#" + pubId).find("#pubeditedname-error").html(x);
                    $("#" + pubId).find("#pubEditedName").css("border", "1px solid red");
                }
                if (x == "Địa chỉ không được để trống") {
                    $("#" + pubId).find("#pubeditedaddress-error").html(x);
                    $("#" + pubId).find("#pubEditedAddress").css("border", "1px solid red");
                }
                if (x == "Tên NXB đã tồn tại") {
                    $("#" + pubId).find("#pubeditedname-error").html(x);
                    $("#" + pubId).find("#pubEditedName").css("border", "1px solid red");
                }
                if (result.success == x) {
                    alert(x);
                    window.location.reload();
                }
            });
        },
        error: function (err) {
            alert(err.statusText);
        }
    });
}

$(document).ready(function () {
    $('#myModal').on('hidden.bs.modal', function (e) {
        $(this)
            .find("input,textarea,select,span")
                .val('')
                .end()
            .find("input[type=checkbox], input[type=radio]")
                .prop("checked", "")
                .end();
        document.getElementById('pubname-error').innerHTML = null;
        document.getElementById('pubName').style.border = "1px solid #ccc";
        document.getElementById('pubName').style.borderRadius = "4px";
        document.getElementById('pubaddress-error').innerHTML = null;
        document.getElementById('pubimg-error').innerHTML = null;
        document.getElementById('pubAddress').style.border = "1px solid #ccc";
        document.getElementById('pubAddress').style.border = "4px";
    })
});