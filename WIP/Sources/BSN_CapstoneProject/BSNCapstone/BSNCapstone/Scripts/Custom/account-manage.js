function AuthorConfirm(id) {
    var url = $("#AuthorConfirm").data('url');
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
            window.location.reload();
        },
        error: function (err) {
            alert(err.statusText);
        }
    });
}