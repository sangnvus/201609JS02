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

namespace BSNCapstone.Hubs
{
    public class GroupHub : Hub
    {
        private readonly ApplicationIdentityContext con = ApplicationIdentityContext.Create();

        public void GetGroupPosts(Group group)
        {
            List<Object> listPost = new List<object>();

            if (con.Posts.Find(x => x.GroupId == group.Id) != null) {
                // get dữ liệu từ db
                List<Post> posts = con.Posts.Find(x => x.GroupId == group.Id).SortByDescending(m => m.PostedDate).ToList();
                List<Comment> listPostCmt = new List<Comment>();
                List<PostLike> listPostLike = new List<PostLike>();

                // truyền qua view
                List<Object> listComment;

                foreach (var item in posts)
                {
                    //mỗi lần loop phải reset listComment để tránh trường hợp khi listPostCmt null -> vẫn lưu listComment cũ -> load cmt sai
                    listComment = new List<object>();

                    //lấy dữ liệu comment của từng post gán vào listComment
                    listPostCmt = new List<Comment>(item.PostComments);
                    listPostLike = new List<PostLike>(item.PostLikes);

                    foreach (var cmt in listPostCmt)
                    {
                        //tạo 1 colection với các biến giống với view để hiển thị comment
                        var userComment = con.Users.Find(x => x.Id == cmt.CommentedBy).FirstOrDefault();
                        var comment = new
                        {
                            CommentId = cmt.CommentId,
                            CommentedBy = userComment.UserName,
                            CommentedByAvatar = userComment.Avatar,
                            CommentedDate = cmt.CommentedDate,
                            Message = cmt.Message,
                            PostId = cmt.PostId
                        };
                        listComment.Add(comment);
                    }

                    //tạo 1 colection với các biến giống với view để hiển thị Post và comment
                    var userpost = con.Users.Find(x => x.Id == item.PostedById).FirstOrDefault();
                    var ret = new
                    {
                        Message = item.Message,
                        PostedById = item.PostedById,
                        PostedByName = userpost.UserName,
                        PostedByAvatar = userpost.Avatar,
                        PostedDate = item.PostedDate,
                        PostId = item.Id,
                        PostComments = listComment,
                        NumOfPostLike = listPostLike.Count,
                        GroupId = item.GroupId
                    };
                    listPost.Add(ret);
                }

                // calls loadPosts javascript method at client side.
                Clients.Caller.loadPosts(listPost.ToArray());
            }
            else
            {
                Clients.Caller.loadPosts(listPost.ToArray());
            }
            
        }

        public void AddPost(Post post)
        {
            var currentUserId = Context.User.Identity.GetUserId();
            //lưu post vào mongo trước sau đó mới hiển thị
            var newPost = new Post()
            {
                Message = post.Message,
                PostedById = currentUserId,
                PostedDate = DateTime.Now,
                GroupId = post.GroupId
            };
            con.Posts.InsertOneAsync(newPost);

            //Hiển thị post vừa lưu
            //Lấy info của post mới nhất <vừa lưu> để truyền qua view
            Post disPost = con.Posts.Find(_ => true).Limit(1).SortByDescending(m => m.PostedDate).FirstOrDefault();
            var userpost = con.Users.Find(x => x.Id == newPost.PostedById).FirstOrDefault();
            var ret = new
            {
                PostId = disPost.Id,
                Message = disPost.Message,
                PostedByName = userpost.UserName,
                PostedById = disPost.PostedById,
                PostedByAvatar = userpost.Avatar,
                PostedDate = disPost.PostedDate,
                GroupId = disPost.GroupId
            };

            Clients.Caller.addPost(ret);
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
                CommentedBy = Context.User.Identity.GetUserId(),
                CommentedDate = DateTime.UtcNow,
                Message = postcomment.Message,
                PostId = postcomment.PostId
            };

            /*
             * Add comment vào post tương ứng trong db
             */
            var filter = Builders<Post>.Filter.Eq(x => x.Id, postcomment.PostId);
            var update = Builders<Post>.Update.Push(x => x.PostComments, newCmt);
            con.Posts.FindOneAndUpdate(filter, update);

            /*
             * Truyền cmt ra view thông qua ret 
             */
            Post postCmt = con.Posts.Find(x => x.Id == postcomment.PostId).FirstOrDefault();
            Comment disCmt = new List<Comment>(postCmt.PostComments).LastOrDefault();
            /*
             * Để sau khi user edit tên thì sẽ luôn load ra tên hiện tại
             */
            var userComment = con.Users.Find(x => x.Id == newCmt.CommentedBy).FirstOrDefault();
            var ret = new
            {
                CommentId = disCmt.CommentId,
                CommentedBy = userComment.UserName,
                CommentedByAvatar = userComment.Avatar,
                CommentedDate = disCmt.CommentedDate,
                Message = disCmt.Message,
                PostId = disCmt.PostId
            };

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
                LikedBy = Context.User.Identity.GetUserId()
            };
            if (!isUnlike)
            {
                var filter = Builders<Post>.Filter.Eq(x => x.Id, like.PostId);
                var update = Builders<Post>.Update.Push(x => x.PostLikes, newLike);
                con.Posts.FindOneAndUpdate(filter, update);

                //Lấy thông tin đã update trong db truyển ra view hiển thị
                Post post = con.Posts.Find(x => x.Id == like.PostId).FirstOrDefault();
                PostLike dislike = new List<PostLike>(post.PostLikes).FirstOrDefault();
                var userLike = con.Users.Find(x => x.Id == newLike.LikedBy).FirstOrDefault().UserName;
                var ret = new
                {
                    NumOfPostLike = post.PostLikes.Count
                };

                Group group = con.Groups.Find(x => x.Id == p.GroupId).FirstOrDefault();
                GetGroupPosts(group);
                //Clients.All.loadNewLikes(post);
                return ret;
            }
            else
            {
                var filter = Builders<Post>.Filter.Eq(x => x.Id, like.PostId);
                var update = Builders<Post>.Update.PullFilter(x => x.PostLikes,
                    Builders<PostLike>.Filter.Eq(x => x.LikedBy, newLike.LikedBy));
                con.Posts.FindOneAndUpdate(filter, update);

                //Lấy thông tin đã update trong db truyển ra view hiển thị
                Post post = con.Posts.Find(x => x.Id == like.PostId).FirstOrDefault();
                var ret = new
                {
                    NumOfPostLike = post.PostLikes.Count
                };

                Group group = con.Groups.Find(x => x.Id == p.GroupId).FirstOrDefault();
                GetGroupPosts(group);
                //Clients.All.loadNewLikes(post);
                return ret;
            }

        }

        public void DeletePost(Post post)
        {
            Post p = con.Posts.Find(x => x.Id == post.Id).FirstOrDefault();
            Group group = con.Groups.Find(x => x.Id == p.GroupId).FirstOrDefault();
            con.Posts.FindOneAndDeleteAsync(x => x.Id.Equals(new ObjectId(post.Id)));
            GetGroupPosts(group);
        }

        public void EditPost(Post post)
        {
            var filter = Builders<Post>.Filter.Eq(x => x.Id, post.Id);
            var update = Builders<Post>.Update.Set(x => x.Message, post.Message);
            con.Posts.FindOneAndUpdate(filter, update);
        }
    }
}