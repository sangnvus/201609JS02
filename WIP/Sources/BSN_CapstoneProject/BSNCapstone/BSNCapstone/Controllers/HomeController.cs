using BSNCapstone.App_Start;
using BSNCapstone.ControllerHelpers;
using BSNCapstone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using MongoDB.Driver;
using BSNCapstone.ViewModels;

namespace BSNCapstone.Controllers
{
    [Authorize]
    public class HomeController : Controller

    {
        private readonly ApplicationIdentityContext Context = ApplicationIdentityContext.Create();
        private readonly CloudinaryDotNet.Cloudinary cloudinary = ImageUploadHelper.GetCloudinaryAccount();
        private IdentityConfig.ApplicationUserManager _userManager;
        public IdentityConfig.ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<IdentityConfig.ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        public ActionResult Index()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            ViewBag.listHotBook = BooksControllerHelper.SuggestBook("", 1);
            ViewBag.listNewBook = BooksControllerHelper.SuggestBook("", 3);
            ViewBag.allUsers = Context.Users.Find(_ => true).ToList();
            ViewBag.allAuthors = Context.Authors.Find(_ => true).ToList();
            ViewBag.cloudinary = cloudinary;
            var listGroup = new List<Group>();
            foreach (var group in Context.Groups.Find(_ => true).ToList())
            {
                foreach (var member in group.GroupMembers)
                {
                    if (member.UserId == User.Identity.GetUserId())
                    {
                        listGroup.Add(group);
                    }
                }
            }
            Random random = new Random((int)(DateTime.Now.Ticks));
            ViewBag.randomAuthor = Context.Authors.Find(x => x.UserId != user.Id.ToString()).ToList().OrderBy(x => random.Next()).Take(2).ToList();
            ViewBag.randomPublisher = Context.Publishers.Find(_ => true).ToList().OrderBy(x => random.Next()).Take(2).ToList();
            ViewBag.listGroup = listGroup;
            return View(user);
        }

        [HttpPost]
        public ActionResult GetListBookTag(string input)
        {
            var books = Context.Books.AsQueryable().Where(p => p.BookName.ToLower().Contains(input)).Select(p => new { p.BookName }).Distinct();
            return Json(books, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LockedPage()
        {
            ViewBag.allAuthors = Context.Authors.Find(_ => true).ToList();
            ViewBag.cloudinary = cloudinary;
            var user = UserManager.FindById(User.Identity.GetUserId());
            ViewBag.listHotBook = BooksControllerHelper.SuggestBook("", 1);
            Random random = new Random((int)(DateTime.Now.Ticks));
            ViewBag.randomAuthor = Context.Authors.Find(x => x.UserId != user.Id.ToString()).ToList().OrderBy(x => random.Next()).Take(2).ToList();
            ViewBag.randomPublisher = Context.Publishers.Find(_ => true).ToList().OrderBy(x => random.Next()).Take(2).ToList();
            return View(user);
        }

    }
}