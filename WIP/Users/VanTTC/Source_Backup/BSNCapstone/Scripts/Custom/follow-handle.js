function FollowHandle(option) {
    var url = $("#FollowHandleUrl").data('url');
    var followerId = $("#FollowHandleUrl").data('followerid');
    var followedUserId = $("#FollowHandleUrl").data('followeduserid');
    var formData = new FormData();
    formData.append("followerId", followerId);
    formData.append("followedUserId", followedUserId);
    formData.delete("option", option);
    if (option == 1) {
        formData.append("option", option);
        $("#follow").toggle();
        $("#unfollow").toggle();
    } else {
        formData.append("option", option);
        $("#unfollow").toggle();
        $("#follow").toggle();
    }
    $.ajax({
        type: "POST",
        url: url,
        data: formData,
        dataType: 'json',
        contentType: false,
        processData: false,

        success: function (result) {
        }
    });
};

function FollowHandleHomePage(authorId, option) {
    var url = $("#FollowHandleHomePage").data('url');
    var followerId = $("#FollowHandleHomePage").data('followerid');
    var formData = new FormData();
    console.log("abc");
    formData.append("followerId", followerId);
    formData.append("followedUserId", authorId);
    formData.append("option", option);
    $.ajax({
        type: "POST",
        url: url,
        data: formData,
        dataType: 'json',
        contentType: false,
        processData: false,

        success: function (result) {
        }
    });
};

$(".buttonFo").click(function () {
    $(this).toggle();
    $(this).siblings(".buttonFo").toggle();
    console.log($(this).data("option"));
    console.log($(this).data("author"));
    var option = $(this).data("option");
    var authorId = $(this).data("author");
    FollowHandleHomePage(authorId, option);
});