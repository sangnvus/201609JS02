using BSNCapstone.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BSNCapstone.Models;
using MongoDB.Driver;
using MongoDB.Driver.Builders;


namespace BSNCapstone.Controllers
{
    [Authorize]
    public class NewFeedController : Controller
    {
        private readonly ApplicationIdentityContext Context = ApplicationIdentityContext.Create();
        //
        // GET: /Recommendation/
        public ActionResult Index()
        {
            //sort posts ascending by time of posts
            var posts = Context.Posts.Find(_ => true).SortByDescending(m => m.Time).ToEnumerable();
            return View(posts);
        }

        [HttpPost]
        public ActionResult CreatePost(string postContent, string bookTag)
        {
            //System.Globalization.CultureInfo.CurrentCulture.ClearCachedData();
            var newPost = new Post()
            {
                UserID = User.Identity.Name,
                Content = postContent,
                Time = DateTime.Now,
                BookId = bookTag
            };
            Context.Posts.InsertOneAsync(newPost);
            return RedirectToAction("Index");
        }
    }
}