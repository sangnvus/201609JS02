
/*==================================== CLIENT SIDE MODEL ======================================*/

function Post(data, hub) {
    /*
        Sau khi gọi đến hàm getPost của Hub (Server Side) -> get ra list toàn bộ các post
        Theo vòng for trên view sẽ get ra từng item để hiển thị lên view (data)
        các item dạng self.something sẽ tương ứng với data-bind="something" trên view
    */
    var self = this;
    data = data || {};

    self.PostId = data.PostId;
    self.Message = ko.observable(data.Message || "");
    self.PostedById = data.PostedById || "";
    self.PostedByName = data.PostedByName || "";
    self.PostedByAvatar = data.PostedByAvatar || "";
    self.PostedDate = getTimeAgo(data.PostedDate);
    self.BookTag = data.BookTag || "";
    self.GroupName = data.GroupName || "";

    self.PostComments = ko.observableArray();
    self.NewComments = ko.observableArray();
    self.newCommentMessage = ko.observable();

    self.NumOfPostLike = data.NumOfPostLike;

    self.error = ko.observable();
    self.hub = hub;

    /* ---------------- COMMENT ---------------- */

    /* Desc:
    * gọi hàm AddComment bên hub
    * truyền vào postId và nội dung của comment
    */
    self.addComment = function () {
        self.hub.server.addComment({ "PostId": self.PostId, "Message": self.newCommentMessage() }).done(function (comment) {
            self.PostComments.push(new Comment(comment));
            self.newCommentMessage('');
        }).fail(function (err) {
            self.error(err);
        });
    }

    /* Desc:
    * khi other clients click vào link loadnewCmt thì load ra cmt mới và reset list newCmt về rỗng
    */
    self.loadNewComments = function () {
        self.PostComments(self.PostComments().concat(self.NewComments()));
        self.NewComments([]);
    }

    /* Desc:
    * Khi load post+cmt ra view cần hàm check này
    * check xem bài post đó có cmt hay ko, nếu có sẽ map vào (đoạn này chưa hiểu rõ)
    * nếu ko có hàm này thì khi F5 trang sẽ ko hiển thị cmt
    */
    if (data.PostComments) {
        var mappedPosts = $.map(data.PostComments, function (item) { return new Comment(item); });
        self.PostComments(mappedPosts);
    }

    /* ---------------- LIKE ---------------- */
    /* Desc:
    * gọi hàm AddPostLike bên hub và truyền vào Id của post để thêm lượt like post tương ứng
    */
    self.addPostLike = function () {
        self.error(null);
        self.hub.server.addPostLike({ "PostId": self.PostId }).fail(function (err) {
            self.error(err);
        });
    }

    /* ---------------- POST ---------------- */
    /* Desc:
    * gọi hàm DeletePost bên hub và truyền vào Id của post để delete post tương ứng
    */
    self.deletePost = function () {
        self.hub.server.deletePost({ "Id": self.PostId }).fail(function (err) {
            self.error(err);
        });
    }

    /* Desc:
    * gọi hàm EditPost bên hub và truyền vào Id + Message mới của post để update post tương ứng
    */
    self.editPost = function () {
        var bookTag = $("#hiddenEditedBookTag").val();
        self.hub.server.editPost({ "Message": self.Message(), "Id": self.PostId , "Booktag": bookTag}).fail(function (err) {
            self.error(err);
        });
    }
}

/* Desc:
* get ra các cmt -> gán vào self để để hiển thị lên view
*/
function Comment(data) {
    var self = this;
    data = data || {};

    // Persisted properties
    self.CommentId = data.CommentId;
    self.PostId = data.PostId;
    self.Message = ko.observable(data.Message || "");
    self.CommentedBy = data.CommentedBy || "";
    self.CommentedByAvatar = data.CommentedByAvatar || "";
    self.CommentedByName = data.CommentedByName || "";
    self.CommentedDate = getTimeAgo(data.CommentedDate);
    self.NumOfCmtLike = data.NumOfCmtLike;
    self.error = ko.observable();
}


/*==================================== VIEW MODEL LAYER ======================================*/

function viewModel() {
    var self = this;
    self.posts = ko.observableArray();
    self.newMessage = ko.observable();
    self.error = ko.observable();
    self.newPosts = ko.observableArray();   // SignalR related
    self.hub = $.connection.postHub;        // Reference the proxy for the hub. 

    /*
    * khi vào view sẽ gọi hàm này đầu tiên để call function getPosts bên Hub
    */
    self.init = function () {
        self.error(null);
        self.hub.server.getNewFeedPosts().fail(function (err) {
            self.error(err);
        });
    }

    /* ---------------- POST ---------------- */

    /* Desc:
    * khi ấn button Đăng sẽ gọi đến hàm này -> gọi method addPost của hub
    */
    self.addPost = function () {
        self.error(null);
        var bookTag = $("#hiddenBookTag").val();
        self.hub.server.addPost({ "Message": self.newMessage(), "BookTag": bookTag }).fail(function (err) {
            self.error(err);
        });
    }

    /* Desc:
    * concat nối thêm các newPost vào posts (array of objs)
    * sau đó refresh list newPost về list rỗng
    */
    self.hub.client.loadNewPosts = function () {
        self.posts(self.newPosts().concat(self.posts()));
        self.newPosts([]);
    }

    /* Desc:
    * Sau khi get ra list post thì hàm này được gọi từ hub 
    * khi A post 1 bài post mới thì nó cũng sẽ hiển thị lên newfeed của B
    */
    self.hub.client.loadPosts = function (data) {
        var mappedPosts = $.map(data, function (item) { return new Post(item, self.hub); });
        self.posts(mappedPosts);
    }

    /* Desc:
    * splice: new post được chèn vào vị trí 0 và ko remove post nào cả -> post mới nhất được hiển thị đầu tiên
    * sau khi save post mới, hàm này được gọi đến caller để hiển thị post mới ở trên đầu
    */
    self.hub.client.addPost = function (post) {
        self.posts.splice(0, 0, new Post(post, self.hub));
        self.newMessage('');
    }

    /* Desc:
    * tương tự như addPost gọi cho caller thì newPost được gọi cho các clients khác để update thêm post mới
    */
    self.hub.client.newPost = function (post) {
        self.newPosts.splice(0, 0, new Post(post, self.hub));
    }

    self.hub.client.error = function (err) {
        self.error(err);
    }

    /* ---------------- COMMENT ---------------- */
    /* Desc:
    * 1 client đăng cmt. để cmt đó hiển thị trên view của client khác thì clients khác cần phải get đc các new cmt
    * 
    */
    self.hub.client.newComment = function (comment, postId) {
        //check in existing posts
        var posts = $.grep(self.posts(), function (item) {
            return item.PostId === postId;
        });
        if (posts.length > 0) {
            posts[0].NewComments.push(new Comment(comment));
        }
        else {
            //check in new posts (not displayed yet)
            posts = $.grep(self.newPosts(), function (item) {
                return item.PostId === postId;
            });
            if (posts.length > 0) {
                posts[0].NewComments.push(new Comment(comment));
            }
        }
    }

    /* ---------------- LIKE ---------------- */
    self.hub.client.newLike = function (ret) {
        self.NumOfLike = ret.NumOfLike;
    }

    return self;
};

/*
* hiển thị thời gian dưới dạng x phút/ngày/năm trước
*/
function getTimeAgo(varDate) {
    if (varDate) {
        return $.timeago(varDate.toString().slice(-1) == 'Z' ? varDate : varDate + 'Z');
    }
    else {
        return '';
    }
}

/*
* Activate Knockout and start Hub
*/
var vmPost = new viewModel();
ko.applyBindings(vmPost);
$.connection.hub.start().done(function () {
    vmPost.init();
});
