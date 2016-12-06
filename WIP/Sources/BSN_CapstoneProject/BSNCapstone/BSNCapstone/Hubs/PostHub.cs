using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using BSNCapstone.Models;
using WebMatrix.WebData;
using BSNCapstone.App_Start;
using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.AspNet.Identity;

/*
 * Giải thích về Hub:
 * Người dùng ấn tạo 1 bài post mới 
 * => call addPost method của client side
 * => addPost method của client side sẽ gọi đến addPost method của thằng Hub
 * => Add Post  (Hàm add post tương tự như trong các Controller vẫn làm)
 * => self.hub.client.addPost (client side) được gọi cho thằng caller để add post
 * và self.hub.client.newPost (client side) được gọi cho những thằng người dùng còn lại để load ra nhưng post mới
 * => Noty
 */


namespace BSNCapstone.Hubs
{
    public class PostHub : Hub
    {
        private readonly ApplicationIdentityContext con = ApplicationIdentityContext.Create();

        public void GetPosts()
        {
            //lấy dữ liểu posts
            List<Post> posts = con.Posts.Find(_ => true).SortByDescending(m => m.PostedDate).ToList();
            //list post truyển qua view
            List<Object> listPost = new List<object>();
            //list comment dùng để gán vào PostComments trong ret
            List<Object> listComment;
            // 22-11-2016
            List<Object> listLike;
            // 22-11-2016
            //list comment lấy từ icolection<PostComments> do icolection không lấy đc phần tử nên phải chuyển qua list
            List<Comment> listPostCmt = new List<Comment>();
            // 22-11-2016
            List<PostLike> listPostLike = new List<PostLike>();
            // 22-11-2016

            foreach (var item in posts)
            {
                //mỗi lần loop phải reset listComment để tránh trường hợp khi listPostCmt null -> vẫn lưu listComment cũ -> load cmt sai
                listComment = new List<object>();
                // 22-11-2016
                listLike = new List<object>();
                // 22-11-2016
                //lấy dữ liệu comment của từng post gán vào listComment
                listPostCmt = new List<Comment>(item.PostComments);
                // 22-11-2016
                listPostLike = new List<PostLike>(item.PostLikes);
                // 22-11-2016
                foreach (var cmt in listPostCmt)
                {
                    //tạo 1 colection với các biến giống với view để hiển thị comment
                    // HuyenPT. 06-12. Add. Start
                    var userComment = con.Users.Find(x => x.Id == cmt.CommentedBy).FirstOrDefault().UserName;
                    // HuyenPT. 06-12. Add. End
                    var comment = new
                    {
                        CommentId = cmt.CommentId,
                        // HuyenPT. 06-12. Update. Start
                        //CommentedBy = cmt.CommentedBy,
                        CommentedBy = userComment,
                        // HuyenPT. 06-12. Update. End
                        CommentedByAvatar = "/Images/profileimages/user.png",
                        CommentedDate = cmt.CommentedDate,
                        Message = cmt.Message,
                        PostId = cmt.PostId
                    };
                    listComment.Add(comment);
                }

                // 22-11-2016
                foreach (var postlike in listPostLike)
                {
                    var a = new
                    {
                        Likedby = postlike.LikedBy
                    };
                    listLike.Add(a);
                }
                // 22-11-2016

                //tạo 1 colection với các biến giống với view để hiển thị Post và comment
                // HuyenPT. 06-12. Add. Start
                /*
                 * Để nếu user đã edit tên thì sẽ luôn get ra tên hiện tại
                 */
                var userpost = con.Users.Find(x => x.Id == item.PostedById).FirstOrDefault();
                // HuyenPT. 06-12. Add. End
                var ret = new
                {
                    Message = item.Message,
                    PostedById = item.PostedById,
                    PostedBy = userpost.UserName,
                    PostedByAvatar = "/Images/profileimages/user.png",
                    PostedDate = item.PostedDate,
                    PostId = item.Id,
                    PostComments = listComment,
                    NumOfPostLike = listLike.Count
                };
                listPost.Add(ret);
            }

            // calls loadPosts javascript method at client side.
            Clients.All.loadPosts(listPost.ToArray());
        }

        public void AddPost(Post post)
        {
            //lưu post vào mongo trước sau đó mới hiển thị
            var newPost = new Post()
            {
                Message = post.Message,
                PostedById = Context.User.Identity.GetUserId(),
                // HuyenPT. 06-12. Delete. Start
                // HuyenPT. 06-12. Delete. End
                //PostedBy = Context.User.Identity.Name,\
                PostedDate = DateTime.Now
            };
            con.Posts.InsertOneAsync(newPost);

            //Hiển thị post vừa lưu
            //Lấy info của thằng post mới nhất <vừa lưu> để truyền qua view
            Post disPost = con.Posts.Find(_ => true).Limit(1).SortByDescending(m => m.PostedDate).FirstOrDefault();
            var userpost = con.Users.Find(x => x.Id == newPost.PostedById).FirstOrDefault().UserName;
            var ret = new
            {
                PostId = disPost.Id,
                Message = disPost.Message,
                //PostById = Context.User.Identity.GetUserId(),
                // HuyenPT. 06-12. Delete. Start
                //PostedById = disPost.PostedById,
                // HuyenPT. 06-12. Delete. End
                //PostedBy = Context.User.Identity.Name,
                // HuyenPT. 06-12. Edit. Start
                //PostedBy = disPost.PostedBy,
                PostedBy = userpost,
                // HuyenPT. 06-12. Edit. End
                PostedByAvatar = "/Images/profileimages/user.png",
                PostedDate = disPost.PostedDate
            };
            // addPost method is called for caller
            Clients.Caller.addPost(ret);
            // newPost method is called for other users
            Clients.Others.newPost(ret);
            Clients.Others.loadNewPosts(ret);
        }

        public dynamic AddComment(Comment postcomment)
        {
            /* Lưu cmt vào DB */
            // tìm comment cuối cùng trong post rồi +1 để làm id cho cmt hiện tại
            Post findPost = con.Posts.Find(x => x.Id == postcomment.PostId).FirstOrDefault();

            int disId = 0;
            if (findPost.PostComments.Count != 0)
            {
                disId = ++findPost.PostComments.Last().CommentId;
            }
            var newCmt = new Comment
            {
                CommentId = disId,
                // HuyenPT. 06-12. Edit. Start
                //CommentedBy = Context.User.Identity.Name,
                CommentedBy = Context.User.Identity.GetUserId(),
                // HuyenPT. 06-12. Edit. End
                CommentedDate = DateTime.UtcNow,
                Message = postcomment.Message,
                PostId = postcomment.PostId
            };

            //tìm post có id = postcomment.PostId
            var filter = Builders<Post>.Filter.Eq(x => x.Id, postcomment.PostId);
            //update newCmt vào PostComments
            var update = Builders<Post>.Update.Push(x => x.PostComments, newCmt);
            //tìm document phù hợp rồi update
            con.Posts.FindOneAndUpdate(filter, update);

            /* Truyền cmt ra view thông qua ret */
            Post postCmt = con.Posts.Find(x => x.Id == postcomment.PostId).FirstOrDefault();
            Comment disCmt = new List<Comment>(postCmt.PostComments).LastOrDefault();
            // HuyenPT. 06-12. Add.  Start
            /*
             * Để sau khi user edit tên thì sẽ luôn load ra tên hiện tại
             */
            var userComment = con.Users.Find(x => x.Id == newCmt.CommentedBy).FirstOrDefault().UserName;
            // HuyenPT. 06-12. Add.  End
            var ret = new
            {
                CommentId = disCmt.CommentId,
                // HuyenPT. 06-12. Edit. Start
                //CommentedBy = Context.User.Identity.Name,
                CommentedBy = userComment,
                // HuyenPT. 06-12. Edit. End
                CommentedByAvatar = "/Images/profileimages/user.png",
                CommentedDate = disCmt.CommentedDate,
                Message = disCmt.Message,
                PostId = disCmt.PostId
            };

            //Clients.Caller.addCommet
            Clients.Others.newComment(ret, postcomment.PostId);
            Clients.Others.loadNewComments();
            return ret;
        }

        public dynamic AddPostLike(PostLike like)
        {
            bool isUnlike = false;
            Post p = con.Posts.Find(x => x.Id == like.PostId).FirstOrDefault();
            List<PostLike> listLike = new List<PostLike>(p.PostLikes);
            for (int i = 0; i < listLike.Count; i++)
            {
                if (listLike[i].LikedBy.Equals(Context.User.Identity.GetUserId()))
                {
                    isUnlike = true;
                }
            }

            var newLike = new PostLike
            {
                PostId = like.PostId,
                // HuyenPT. 06-12. Update. Start
                //LikedBy = Context.User.Identity.Name
                LikedBy = Context.User.Identity.GetUserId()
                // HuyenPT. 06-12. Update. End
            };
            if (!isUnlike)
            {
                //tìm post có id = postcomment.PostId
                var filter = Builders<Post>.Filter.Eq(x => x.Id, like.PostId);
                //update newLike vào PostLikes
                var update = Builders<Post>.Update.Push(x => x.PostLikes, newLike);
                //tìm document phù hợp rồi update
                con.Posts.FindOneAndUpdate(filter, update);

                //Hiển thị
                //Lấy thông tin đã update trong db truyển ra view hiển thị
                Post post = con.Posts.Find(x => x.Id == like.PostId).FirstOrDefault();
                PostLike dislike = new List<PostLike>(post.PostLikes).FirstOrDefault();
                var userLike = con.Users.Find(x => x.Id == newLike.LikedBy).FirstOrDefault().UserName;
                var ret = new
                {
                    NumOfPostLike = post.PostLikes.Count
                };

                GetPosts();
                //Clients.All.loadNewLikes(post);
                return ret;
            }
            else
            {
                //tìm post có id = postcomment.PostId
                var filter = Builders<Post>.Filter.Eq(x => x.Id, like.PostId);
                //update newLike vào PostLikes
                var update = Builders<Post>.Update.PullFilter(x => x.PostLikes,
                    Builders<PostLike>.Filter.Eq(x => x.LikedBy, newLike.LikedBy));
                //tìm document phù hợp rồi update
                con.Posts.FindOneAndUpdate(filter, update);

                //Hiển thị
                //Lấy thông tin đã update trong db truyển ra view hiển thị
                Post post = con.Posts.Find(x => x.Id == like.PostId).FirstOrDefault();
                var ret = new
                {
                    NumOfPostLike = post.PostLikes.Count
                };

                GetPosts();
                //Clients.All.loadNewLikes(post);
                return ret;
            }

        }

        public void DeletePost(Post post)
        {
            con.Posts.FindOneAndDeleteAsync(x => x.Id.Equals(new ObjectId(post.Id)));
            GetPosts();  //update hàm client delete post để cải thiện performance
        }
    }
}