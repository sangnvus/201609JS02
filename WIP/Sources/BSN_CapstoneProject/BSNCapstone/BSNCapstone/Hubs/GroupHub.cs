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

        public void GetGroupPosts()
        {
            // get dữ liệu từ db
            List<Post> postsForShow = new List<Post>();
            List<Post> posts = con.Posts.Find(_ => true).SortByDescending(m => m.PostedDate).ToList();
            var currentUserId = Context.User.Identity.GetUserId();
            var user = con.Users.Find(x => x.Id.Equals(currentUserId)).FirstOrDefault();
            foreach (var following in user.Following)
            {
                if (posts.FindAll(x => x.PostedById.Equals(following)) != null)
                {
                    foreach (var post in posts.FindAll(x => x.PostedById.Equals(following)))
                    {
                        postsForShow.Add(post);
                    }
                }
            }
            if (posts.FindAll(x => x.PostedById.Equals(currentUserId)) != null)
            {
                foreach (var post in posts.FindAll(x => x.PostedById.Equals(currentUserId)))
                {
                    postsForShow.Add(post);
                }
            }

            postsForShow = postsForShow.OrderByDescending(x => x.PostedDate).ToList();

            /* Desc: lisPost
             * lấy list dữ liệu để truyền qua view 
             * do icolection không lấy đc phần tử nên phải chuyển qua list(ret)
             */
            List<Object> listPost = new List<object>();

            foreach (var item in postsForShow)
            {
                List<Comment> listPostCmt = new List<Comment>(item.PostComments);
                List<PostLike> listPostLike = new List<PostLike>(item.PostLikes);

                // lấy dữ liệu comment của từng post gán vào listComment để truyền vào ret
                List<Object> listComment = new List<object>();

                foreach (var cmt in listPostCmt)
                {
                    //tạo 1 colection với các biến giống với view để hiển thị comment
                    var userComment = con.Users.Find(x => x.Id == cmt.CommentedBy).FirstOrDefault().UserName;
                    var comment = new
                    {
                        CommentId = cmt.CommentId,
                        CommentedBy = userComment,
                        CommentedByAvatar = "/Images/profileimages/user.png",
                        CommentedDate = cmt.CommentedDate,
                        Message = cmt.Message,
                        PostId = cmt.PostId
                    };
                    listComment.Add(comment);
                }

                //tạo 1 colection với các biến giống với view để hiển thị Post và comment
                /*
                 * nếu user đã edit tên thì sẽ luôn get ra tên hiện tại
                 */
                var userpost = con.Users.Find(x => x.Id == item.PostedById).FirstOrDefault();
                var ret = new
                {
                    Message = item.Message,
                    PostedById = item.PostedById,
                    PostedByName = userpost.UserName,
                    PostedByAvatar = "/Images/profileimages/user.png",
                    PostedDate = item.PostedDate,
                    PostId = item.Id,
                    PostComments = listComment,
                    NumOfPostLike = listPostLike.Count
                };
                listPost.Add(ret);
            }

            // calls loadPosts javascript method at client side.
            Clients.All.loadPosts(listPost.ToArray());
        }
    }
}