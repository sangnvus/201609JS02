$(document).ready(function () {
    $("input[name=rating]").click(function () {
        var rating = $("input[name=rating]:checked").val();
        var url = $("#RatingBook").data('url');
        var id = $("#RatingBook").data('id');
        var formData = new FormData();
        formData.append("id", id);
        formData.append("rating", rating);
        console.log(rating);
        $.ajax({
            type: "POST",
            url: url,
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,

            success: function (result) {
                window.alert(result);
                window.location.reload();
            }
        });
    });

    $('textarea#comment').keydown(function (e) {
        if (e.keyCode === 13 && e.ctrlKey) {
            console.log("enterKeyDown+ctrl");
            $(this).val(function (i, val) {
                console.log(val);
                return val + "\n";
            });
        }
    }).keypress(function(e){
        if (e.keyCode === 13 && !e.ctrlKey) {
            var formData = new FormData();
            var url = $("#UserComment").data('url');
            var userId = $("#UserComment").data('id');
            var bookId = $("#RatingBook").data('id');
            formData.append("commentedUser", userId);
            formData.append("bookId", bookId);
            formData.append("commentContent", $("#comment").val());
            console.log($("#comment").val());
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
        return false;  
        } 
    })
});
