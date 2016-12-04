function Lock(groupId) {
    var url = $("#LockButton").data('url');
    console.log("abc");
    var formData = new FormData();
    formData.append("id", groupId);
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
};

function GroupRequestHandle(groupId, userId, option) {
    var url = $("#GroupHandleRequestUrl").data('url');
    var formData = new FormData();
    formData.append("groupId", groupId);
    formData.append("userId", userId);
    formData.append("option", option);
    $.ajax({
        type: "POST",
        url: url,
        data: formData,
    dataType: 'json',
    contentType: false,
    processData: false,

    success: function (result) {
        if (result != "") {
            window.alert(result);
        }
        else {
            window.location.reload();
        }
    }
});
};


function AddMember() {
    var url = $("#AddNewMember").data('url');
    var groupId = $("#AddNewMember").data('groupId');
    var user = $("#addMemberSelect").val();
    var formData = new FormData();
    formData.append("addUser", user);
    formData.append("groupId", groupId);
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
        }
    });
}

$(document).ready(function () {
    //Change Avatar
    var groupId = $("#ImageChangeUrl").data('id');
    var groupUrl = $("#ImageChangeUrl").data('url');
    $("#imgInpAva").change(function () {
        var avatarImg = this;
        if (avatarImg.files && avatarImg.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $('.avatar-img').attr('src', e.target.result);
            }
            reader.readAsDataURL(avatarImg.files[0]);
        }
        var formData = new FormData();
        formData.append("id", groupId);
        formData.append("file", avatarImg.files[0]);
        formData.append("option", 1);
        $.ajax({
            type: "POST",
            url: groupUrl,
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,

            success: function (result) {
                alert(result);
            },
            error: function (err) {
                alert(err.statusText);
            }
        });
    });

    //Change Cover
    $("#imgInpCover").change(function () {
        var coverImg = this;
        if (coverImg.files && coverImg.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $('.cover-img').attr('src', e.target.result);
            }
            reader.readAsDataURL(coverImg.files[0]);
        }
        var formData = new FormData();
        formData.append("id", groupId);
        formData.append("file", coverImg.files[0]);
        formData.append("option", 2);
        $.ajax({
            type: "POST",
            url: groupUrl,
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,

            success: function (result) {
                alert(result);
            },
            error: function (err) {
                alert(err.statusText);
            }
        });
    });
});