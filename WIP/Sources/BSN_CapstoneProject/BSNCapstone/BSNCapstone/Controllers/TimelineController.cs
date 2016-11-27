using BSNCapstone.App_Start;
using BSNCapstone.ControllerHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using MongoDB.Bson;
using BSNCapstone.Models;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.AspNet.Identity;

namespace BSNCapstone.Controllers
{
    [Authorize]
    public class TimelineController : Controller
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

        // GET: Timeline
        //public ActionResult Index(string id)
        //{
        //    var user = Context.Users.Find(x => x.Id.Equals(new ObjectId(id))).FirstOrDefault();
        //    return View(user);
        //}

        public ActionResult Details(string id) 
        {
            var user = Context.Users.Find(x => x.Id.Equals(new ObjectId(id))).FirstOrDefault();
            ViewBag.currentUser = User.Identity.GetUserId();
            ViewBag.cloudinary = cloudinary;
            return View(user);
        }

        //
        // GET: /UserProfile/Edit/5
        public ActionResult EditProfile(string id)
        {
            var user = Context.Users.Find(x => x.Id.Equals(new ObjectId(id))).FirstOrDefault();
            UserProfile newprofile = new UserProfile()
            {
                id = user.Id,
                UserName = user.UserName,
                DOB = Convert.ToDateTime(user.DOB).ToString("dd/MM/yyyy"),
                Address = user.Address
            };
            ViewBag.cloudinary = cloudinary;
            ViewBag.user = user;
            ViewBag.currentUser = User.Identity.GetUserId();
            return View(newprofile);
        }

        //
        // POST: /UserProfile/Edit/5
        [HttpPost]
        public async Task<ActionResult> EditProfile(UserProfile user1)
        {
            if (ModelState.IsValid)
            {
                var user2 = await UserManager.FindByIdAsync(user1.id);

                if (user2 == null)
                {
                    return HttpNotFound();
                }

                user2.Address = user1.Address;
                user2.DOB = DateTime.ParseExact(user1.DOB, "dd/MM/yyyy", CultureInfo.InvariantCulture).AddHours(7);
                user2.UserName = user1.UserName;
                await Context.Users.ReplaceOneAsync(x => x.Id.Equals(new ObjectId(user2.Id)), user2);
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult ChangeImage(string id, HttpPostedFileBase file, int option)
        {
            try
            {
                var uploadResult = ImageUploadHelper.GetUploadResult(file);
                switch (option)
                {
                    case 1:
                        var update1 = new BsonDocument("$set", new BsonDocument("Avatar", uploadResult.PublicId));
                        Context.Users.UpdateOneAsync(x => x.Id.Equals(new ObjectId(id)), update1);
                        break;
                    case 2:
                        var update2 = new BsonDocument("$set", new BsonDocument("Cover", uploadResult.PublicId));
                        Context.Users.UpdateOneAsync(x => x.Id.Equals(new ObjectId(id)), update2);
                        break;
                }
                return Json("Đổi ảnh hoàn tất");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
    }
}