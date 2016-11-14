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

namespace BSNCapstone.Hubs
{
    public class PostHub : Hub
    {
        private string imgFolder = "/Images/profileimages/";
        private string defaultAvatar = "user.png";
        private readonly ApplicationIdentityContext Context = ApplicationIdentityContext.Create();
        
        // GET api/WallPost
        public void GetPosts()
        {
            //lấy dữ liểu posts
            List<Post> posts = Context.Posts.Find(_ => true).SortByDescending(m => m.PostedDate).ToList();
            //list post truyển qua view
            List<Object> listPost = new List<object>();
            //list comment dùng để gán vào PostComments trong ret
            List<Object> listComment;
            //list comment lấy từ icolection<PostComments> do icolection không lấy đc phần tử nên phải chuyển qua list
            List<Comment> listPostCmt = new List<Comment>();

            foreach(var item in posts)
            {
                //mỗi lần loop phải reset listComment để tránh trường hợp khi listPostCmt null -> vẫn lưu listComment cũ -> load cmt sai
                listComment = new List<object>();
                //lấy dữ liệu comment của từng post gán vào listComment
                listPostCmt = new List<Comment>(item.PostComments);
                foreach(var cmt in listPostCmt)
                {
                    //tạo 1 colection với các biến giống với view để hiển thị comment
                    var comment = new
                    {
                        CommentedBy = cmt.CommentedBy,
                        CommentedByName = cmt.UserProfile.UserName,
                        CommentedByAvatar = cmt.UserProfile.AvatarExt,
                        CommentedDate = cmt.CommentedDate,
                        CommentId = cmt.Id,
                        Message = cmt.Message,
                        PostId = cmt.PostId
                    };
                    listComment.Add(comment);
                }

                //tạo 1 colection với các biến giống với view để hiển thị Post và comment
                var ret = new
                {
                    Message = item.Message,
                    PostedBy = item.PostedBy,
                    PostedByName = item.UserProfile.UserName,
                    PostedByAvatar = item.UserProfile.AvatarExt,
                    PostedDate = item.PostedDate,
                    PostId = item.Id,
                    PostComments = listComment
                };
                listPost.Add(ret);
            }

            //truyền qua view hiển thị
            Clients.All.loadPosts(listPost.ToArray());
        }

        /*--------------Add post-----------------------*/
        public void AddPost(Post post)
        {
            //lấy thông tin user
            var user = Context.UserProfiles.Find(x => x.UserId == post.PostedBy).FirstOrDefault();
            string avatar = imgFolder + (String.IsNullOrEmpty(user.AvatarExt) ? defaultAvatar : post.PostedBy + "." + post.UserProfile.AvatarExt);

            //lưu post vào mongo trước sau đó mới hiển thị
            var newPost = new Post() 
            {
                Message = post.Message,
                PostedBy = post.PostedBy,
                PostedDate = DateTime.UtcNow,
                UserProfile = new UserProfile() //mongodb ko tự gen đc id cho user @_@, _id:null, dùng tạm userId
                {
                    UserId = post.PostedBy,
                    UserName = user.UserName,
                    AvatarExt = avatar
                }
            };
            Context.Posts.InsertOneAsync(newPost);

            //Hiển thị post vừa lưu
            //Lấy info của thằng post mới nhất <vừa lưu> để truyền qua view
            Post disPost = Context.Posts.Find(_ => true).Limit(1).SortByDescending(m => m.PostedDate).FirstOrDefault();
            var ret = new
                    {
                        PostId = disPost.Id,
                        Message = disPost.Message,
                        PostedBy = disPost.PostedBy,
                        PostedByName = disPost.UserProfile.UserName,
                        PostedByAvatar = avatar,
                        PostedDate = disPost.PostedDate,
                    };
            Clients.Caller.addPost(ret);
            Clients.Others.newPost(ret);
        }

        /*--------------Add comment -----------------------*/
        public dynamic AddComment(Comment postcomment)
        {
            var user = Context.UserProfiles.Find(x => x.UserId == postcomment.CommentedBy).FirstOrDefault();

            //postcomment.CommentedBy = WebSecurity.CurrentUserId;
            postcomment.CommentedBy = postcomment.CommentedBy;
            postcomment.CommentedDate = DateTime.UtcNow;
            string avatar = imgFolder + (String.IsNullOrEmpty(user.AvatarExt) ? 
                defaultAvatar : postcomment.CommentedBy + "." + postcomment.UserProfile.AvatarExt);

            //Lưu db
            var newCmt = new Comment
            {
                Id = postcomment.Id,
                CommentedBy = postcomment.CommentedBy,
                CommentedDate = postcomment.CommentedDate,
                Message = postcomment.Message,
                PostId = postcomment.PostId,
                UserProfile = new UserProfile
                {
                    UserId = postcomment.CommentedBy,
                    UserName = user.UserName,
                    AvatarExt = avatar,
                }
            };
            var filter = Builders<Post>.Filter.Eq(x => x.Id, postcomment.PostId); //tìm document có id = postcomment.PostId
            var update = Builders<Post>.Update.Push(x => x.PostComments, newCmt); //update newCmt vào PostComments
            Context.Posts.FindOneAndUpdate(filter,update); //tìm document phù hợp rồi update

            //Hiển thị
            //Lấy thông tin đã update trong db truyển ra view hiển thị
            Post postCmt = Context.Posts.Find(x => x.Id == postcomment.PostId).FirstOrDefault();
            Comment disCmt = new List<Comment>(postCmt.PostComments).LastOrDefault();
            var ret = new
               {
                   CommentedBy = disCmt.CommentedBy,
                   CommentedByName = disCmt.UserProfile.UserName,
                   CommentedByAvatar = avatar,
                   CommentedDate = disCmt.CommentedDate,
                   CommentId = disCmt.Id,
                   Message = disCmt.Message,
                   PostId = disCmt.PostId
               };
            Clients.Others.newComment(ret, postcomment.PostId);

             return ret;
        }
    }
}