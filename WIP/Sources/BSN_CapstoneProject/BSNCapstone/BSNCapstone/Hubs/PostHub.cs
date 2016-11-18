using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using BSNCapstone.Models;
using WebMatrix.WebData;
using BSNCapstone.App_Start;
using MongoDB.Driver;

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
            //list post để truyển qua view
            List<Object> listPost = new List<object>();
            //list comment dùng để gán vào PostComments trong ret
            List<Object> listComment;
            //list comment lấy từ icolection<PostComments> do icolection không lấy đc phần tử nên phải chuyển qua list
            List<Comment> listPostCmt = new List<Comment>();

            foreach (var item in posts)
            {
                //mỗi lần loop phải reset listComment để tránh trường hợp khi listPostCmt null -> vẫn lưu listComment cũ -> load cmt sai
                listComment = new List<object>();
                //lấy dữ liệu comment của từng post gán vào listComment
                listPostCmt = new List<Comment>(item.PostComments);
                foreach (var cmt in listPostCmt)
                {
                    //tạo 1 colection với các biến giống với view để hiển thị comment
                    var comment = new
                    {
                        CommentedBy = cmt.CommentedBy,
                        CommentedByAvatar = "/Images/profileimages/user.png",
                        CommentedDate = cmt.CommentedDate,
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
                    PostedByAvatar = "/Images/profileimages/user.png",
                    PostedDate = item.PostedDate,
                    PostId = item.Id,
                    PostComments = listComment
                };
                listPost.Add(ret);
            }

            // calls loadPosts javascript method at client side.
            Clients.All.loadPosts(listPost.ToArray());
        }

        /* ----------------- ADD POST METHOD ------------------- */

        public void AddPost(Post post)
        {
            //lưu post vào mongo trước sau đó mới hiển thị
            var newPost = new Post()
            {
                Message = post.Message,
                PostedBy = Context.User.Identity.Name,
                PostedDate = DateTime.UtcNow
            };
            con.Posts.InsertOneAsync(newPost);

            //Hiển thị post vừa lưu
            //Lấy info của thằng post mới nhất <vừa lưu> để truyền qua view
            Post disPost = con.Posts.Find(_ => true).Limit(1).SortByDescending(m => m.PostedDate).FirstOrDefault();
            var ret = new
            {
                PostId = disPost.Id,
                Message = disPost.Message,
                PostedBy = Context.User.Identity.Name,
                PostedByAvatar = "/Images/profileimages/user.png",
                PostedDate = disPost.PostedDate,
            };
            // addPost method is called for caller
            Clients.Caller.addPost(ret);
            // newPost method is called for other users
            Clients.Others.newPost(ret);
        }

        /* ----------------- ADD COMMENTS METHOD ------------------- */

        public dynamic AddComment(Comment postcomment)
        {
            //Lưu db
            var newCmt = new Comment
            {
                CommentedBy = Context.User.Identity.Name,
                CommentedDate = DateTime.UtcNow,
                Message = postcomment.Message,
                PostId = postcomment.PostId
            };
            var filter = Builders<Post>.Filter.Eq(x => x.Id, postcomment.PostId); //tìm post có id = postcomment.PostId
            var update = Builders<Post>.Update.Push(x => x.PostComments, newCmt); //update newCmt vào PostComments
            con.Posts.FindOneAndUpdate(filter, update); //tìm document phù hợp rồi update

            //Hiển thị
            //Lấy thông tin đã update trong db truyển ra view hiển thị
            Post postCmt = con.Posts.Find(x => x.Id == postcomment.PostId).FirstOrDefault();
            Comment disCmt = new List<Comment>(postCmt.PostComments).LastOrDefault();
            var ret = new
            {
                CommentedBy = Context.User.Identity.Name,
                CommentedByAvatar = "/Images/profileimages/user.png",
                CommentedDate = disCmt.CommentedDate,
                Message = disCmt.Message,
                PostId = disCmt.PostId
            };
            Clients.Others.newComment(ret, postcomment.PostId);
            return ret;
        }
    }
}