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

namespace BSNCapstone.Controllers
{
    [Authorize]
    public class HomeController : Controller

    {
        private readonly ApplicationIdentityContext Context = ApplicationIdentityContext.Create();
        //private readonly CloudinaryDotNet.Cloudinary cloudinary = ImageUploadHelper.GetCloudinaryAccount();
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
            List<Book> ListBook = Context.Books.AsQueryable<Book>().OrderByDescending(x=>x.ReleaseDay).Take(4).ToList();
            //var userid = User.Identity.GetUserId();
            //ApplicationUser currentUser = UserManager.FindByIdAsync(userid);
            var user = UserManager.FindById(User.Identity.GetUserId());

            ViewBag.avatar = user.Avatar;
            ViewBag.Id = user.Id;
            ViewBag.Name = user.UserName;
            return View(ListBook);
        }


    }
}