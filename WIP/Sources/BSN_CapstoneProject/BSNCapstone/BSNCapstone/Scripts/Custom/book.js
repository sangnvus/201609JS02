function RequestAddBook() {
    console.log("request add book");
    var url = $("#RequestAddBookButton").data('url');
    var bookName = $("#bookname").val();
    var author = $("#author").val();
    var categories = $("#AddCategories").val();
    var formData = new FormData();
    formData.append("bookName", bookName);
    formData.append("author", author);
    formData.append("categories", categories);
    $.ajax({
        type: "POST",
        url: url,
        data: formData,
        dataType: 'json',
        contentType: false,
        processData: false,

        success: function (result) {
            document.getElementById('bookname-error').innerHTML = null;
            document.getElementById('bookname').style.border = "1px solid #ccc";
            document.getElementById('author-error').innerHTML = null;
            document.getElementById('author').style.border = "1px solid #ccc";
            document.getElementById('category-error').innerHTML = null;
            $.each(result, function (index, x) {
                if (x == "Tên sách không được để trống") {
                    document.getElementById('bookname-error').innerHTML = x;
                    document.getElementById('bookname').style.border = "1px solid red";
                }
                if (x == "Tác giả không được để trống") {
                    document.getElementById('author-error').innerHTML = x;
                    document.getElementById('author').style.border = "1px solid red";
                }
                if (x == "Thể loại không được để trống") {
                    document.getElementById('category-error').innerHTML = x;
                }
                if (x == "Sách đã tồn tại") {
                    document.getElementById('bookname-error').innerHTML = x;
                    document.getElementById('bookname').style.border = "1px solid red";
                }
                if (index == "successed") {
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

function DeleteConfirm(id, i) {
    var url = $("#DeleteConfirmButton").data('url');
    var deleteConfirm = confirm("Bạn có chắc muốn xóa quyển sách này ko?");
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
};

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

    $(document).ready(function () {
        var showChar = 200;
        var ellipsestext = "...";
        var moretext = "More";
        var lesstext = "Less";
        $('.more').each(function () {
            var content = $.trim($(this).html());
            if (content.length > 500) {
                var c = content.substr(0, showChar);
                console.log(c);
                var h = content.substr(showChar - 1, content.length - showChar);
                var html = c + '<span class="moreelipses">' + ellipsestext +
                        '</span>&nbsp;<span class="morecontent"><span>' + h + '</span>&nbsp;&nbsp;<a href="" class="morelink">' + moretext + '</a></span>';
                $(this).html(html);
            }
        });
        $(".morelink").click(function () {
            if ($(this).hasClass("less")) {
                $(this).removeClass("less");
                $(this).html(moretext);
            } else {
                $(this).addClass("less");
                $(this).html(lesstext);
            }
            console.log("abc");
            $(this).parent().prev().toggle();
            $(this).prev().toggle();
            return false;
        });
    });
});
