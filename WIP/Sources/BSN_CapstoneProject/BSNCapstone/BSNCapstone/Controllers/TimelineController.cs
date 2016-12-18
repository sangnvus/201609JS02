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
using BSNCapstone.ViewModels;

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

        public ActionResult Details(string id) 
        {
            List<string> list = new List<string>();
            foreach (var x in ReportContentViewModel.EnumToList<ReportContentViewModel.ReportUser>())
            {
                list.Add(ReportContentViewModel.GetEnumDescription(x));
            }
            Console.Write(list);
            ViewBag.reportContentList = list;
            var user = Context.Users.Find(x => x.Id.Equals(new ObjectId(id))).FirstOrDefault();
            ViewBag.currentUser = User.Identity.GetUserId();
            ViewBag.cloudinary = cloudinary;
            ViewBag.allBook = Context.Books.Find(_ => true).ToList();
            ViewBag.allUser = Context.Users.Find(_ => true).ToList();
            ViewBag.allAuthor = Context.Authors.Find(_ => true).ToList();
            ViewBag.listInteractBook = BooksControllerHelper.LastestBookInteracted(id);
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
                Gender = user.Gender,
                Address = user.Address
            };
            ViewBag.cloudinary = cloudinary;
            ViewBag.user = user;
            ViewBag.currentUser = User.Identity.GetUserId();
            ViewBag.allUser = Context.Users.Find(_ => true).ToList();
            ViewBag.allAuthor = Context.Authors.Find(_ => true).ToList();
            ViewBag.listInteractBook = BooksControllerHelper.LastestBookInteracted(id);
            return View(newprofile);
        }

        //
        // POST: /UserProfile/Edit/5
        [HttpPost]
        public async Task<ActionResult> EditProfile(UserProfile user1)
        {
            var allUser = Context.Users.Find(_ => true).ToList();
            if (user1.UserName != null)
            {
                foreach (var user in allUser)
                {
                    if (user.UserName.ToLower().Equals(user1.UserName.ToLower()) &&
                        allUser.Find(x => x.UserName.ToLower().Equals(user1.UserName.ToLower())).Id != user1.id)
                    {
                        ModelState.AddModelError("UserName", "Tên đã tồn tại");
                    }
                }
            }
            if (user1.DOB == null)
            {
                ModelState.AddModelError("DOB", "Ngày sinh không được để trống");
            }
            if (ModelState.IsValid)
            {
                var user2 = await UserManager.FindByIdAsync(user1.id);

                if (user2 == null)
                {
                    return HttpNotFound();
                }

                user2.Address = user1.Address;
                user2.DOB = DateTime.ParseExact(user1.DOB, "dd/MM/yyyy", CultureInfo.InvariantCulture).AddHours(7);
                user2.Gender = user1.Gender;
                user2.UserName = user1.UserName;
                await Context.Users.ReplaceOneAsync(x => x.Id.Equals(new ObjectId(user2.Id)), user2);
                return RedirectToAction("Index", "Home");
            }
            ViewBag.cloudinary = cloudinary;
            ViewBag.user = Context.Users.Find(x => x.Id.Equals(new ObjectId(user1.id))).FirstOrDefault();
            ViewBag.currentUser = User.Identity.GetUserId();
            ViewBag.allUser = allUser;
            ViewBag.allAuthor = Context.Authors.Find(_ => true).ToList();
            ViewBag.listInteractBook = BooksControllerHelper.LastestBookInteracted(user1.id);
            return View(user1);
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

        [HttpPost]
        public ActionResult FollowHandle(string followerId, string followedUserId, int option)
        {
            int message = 0;
            switch (option)
            {
                case 1:
                    // Thêm Id vào list follower (Về phía người được follow)
                    var followerAddFilter = Builders<ApplicationUser>.Filter.Where(x => x.Id.Equals(new ObjectId(followedUserId)));
                    var followerAddUpdate = Builders<ApplicationUser>.Update.Push(x => x.Follower, followerId);
                    Context.Users.UpdateOneAsync(followerAddFilter, followerAddUpdate);
                    // Thêm Id vào list following (Về phía người đi follow) 
                    var followingAddFilter = Builders<ApplicationUser>.Filter.Where(x => x.Id.Equals(new ObjectId(followerId)));
                    var followingAddUpdate = Builders<ApplicationUser>.Update.Push(x => x.Following, followedUserId);
                    Context.Users.UpdateOneAsync(followingAddFilter, followingAddUpdate);
                    message = 1;
                    break;
                case 2:
                    // Bỏ Id khỏi list follower (Về phía người được follow)
                    var followerRemoveFilter = Builders<ApplicationUser>.Filter.Where(x => x.Id.Equals(new ObjectId(followedUserId)));
                    var followerRemoveUpdate = Builders<ApplicationUser>.Update.Pull(x => x.Follower, followerId);
                    Context.Users.UpdateOneAsync(followerRemoveFilter, followerRemoveUpdate);
                    // Bỏ Id khỏi list following (Về phía người đi follow)
                    var followingRemoveFilter = Builders<ApplicationUser>.Filter.Where(x => x.Id.Equals(new ObjectId(followerId)));
                    var followingRemoveUpdate = Builders<ApplicationUser>.Update.Pull(x => x.Following, followedUserId);
                    Context.Users.UpdateOneAsync(followingRemoveFilter, followingRemoveUpdate);
                    message = 2;
                    break;
            }
            return Json(message);
        }

        [HttpPost]
        public ActionResult Lock(string id, int option, string reportId)
        {
            bool updateStatus = true;
            if (option == 1)
            {
                updateStatus = false;
            }
            else if (option == 2)
            {
                updateStatus = true;
                var reportUpdate = new BsonDocument("$set", new BsonDocument("Status", false));
                Context.Reports.UpdateOneAsync(x => x.Id.Equals(new ObjectId(reportId)), reportUpdate);
            }
            else
            {
                updateStatus = true;
            }
            var update = new BsonDocument("$set", new BsonDocument("Locked", updateStatus));
            Context.Users.UpdateOneAsync(x => x.Id.Equals(new ObjectId(id)), update);
            return Json("");
        }
    }
}