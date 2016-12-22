function CreateGroup() {
    var url = $("#CreateGroupButton").data('url');
    var type = $('input[type=radio][name=grouptype]:checked').val();
    var tag = "";
    if ($('input[type=radio][name=tag]:checked').val() == "AuthorTag_chosen") {
        tag = $('#AuthorTag').val();
    } else {
        tag = $('#BookTag').val();
    }
    console.log(tag);
    var formData = new FormData();
    formData.append("groupName", $("#groupname").val());
    formData.append("groupTag", tag);
    formData.append("groupDesc", $("#groupdesc").val());
    formData.append("groupType", type);
    $.ajax({
        type: "POST",
        url: url,
        data: formData,
        dataType: 'json',
        contentType: false,
        processData: false,

        success: function (result) {
            console.log(result.redirectUrl);
            document.getElementById('groupname-error').innerHTML = null;
            document.getElementById('groupname').style.border = "1px solid #ccc";
            document.getElementById('groupname').style.borderRadius = "4px";
            document.getElementById('grouptag-error').innerHTML = null;
            document.getElementById('grouptype-error').innerHTML = null;
            $.each(result, function (index, x) {
                if (x == "Tên nhóm đã tồn tại") {
                    document.getElementById('groupname-error').innerHTML = x;
                    document.getElementById('groupname').style.border = "1px solid red";
                }
                if (x == "Bạn phải điền tên nhóm") {
                    document.getElementById('groupname-error').innerHTML = x;
                    document.getElementById('groupname').style.border = "1px solid red";
                }
                if (x == "Bạn phải chọn thẻ nhóm") {
                    document.getElementById('grouptag-error').innerHTML = x;
                    document.getElementById('grouptag').style.border = "1px solid red";
                }
                if (x == "Bạn phải chọn thể loại nhóm") {
                    document.getElementById('grouptype-error').innerHTML = x;
                }
                if (result.redirectUrl == x) {
                    window.location.href = x;
                }
            });
        },

        error: function (err) {
            alert(err.statusText);
        }
    });

    $(document).ready(function () {
        $('#createGroupModal').on('hidden.bs.modal', function (e) {
            $(this)
                .find("input,textarea,select,span")
                    .val('')
                    .end()
                .find("input[type=checkbox], input[type=radio]")
                    .prop("checked", "")
                    .end();
            document.getElementById('groupname-error').innerHTML = null;
            document.getElementById('groupname').style.border = "1px solid #ccc";
            document.getElementById('groupname').style.borderRadius = "4px";
            document.getElementById('grouptag-error').innerHTML = null;
            document.getElementById('grouptype-error').innerHTML = null;
        })
    });
};