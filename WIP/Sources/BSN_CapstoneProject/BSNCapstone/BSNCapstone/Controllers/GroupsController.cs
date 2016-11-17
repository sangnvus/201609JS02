using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BSNCapstone.App_Start;
using BSNCapstone.Models;
using BSNCapstone.ControllerHelpers;
using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.AspNet.Identity;
using BSNCapstone.ViewModels;

namespace BSNCapstone.Controllers
{
    [Authorize]
    public class GroupsController : Controller
    {
        private readonly ApplicationIdentityContext Context = ApplicationIdentityContext.Create();
        private readonly CloudinaryDotNet.Cloudinary cloudinary = ImageUploadHelper.GetCloudinaryAccount();
        //
        // GET: /Groups/
        public ActionResult Index()
        {
            ViewBag.groupNumber = GroupsControllerHelper.GetGroupNumber();
            ViewBag.groupJustCreated = GroupsControllerHelper.GetGroupJustCreatedNumber();
            var groups = Context.Groups.Find(_ => true).ToEnumerable();
            return View(groups);
        }

        //
        // POST: /Groups/Lock
        [HttpPost]
        public ActionResult Lock(string id)
        {
            var group = Context.Groups.Find(x => x.Id.Equals(new ObjectId(id))).FirstOrDefault();
            bool updateStatus = true;
            if (group.Locked != true)
            {
                updateStatus = true;
            }
            else
            {
                updateStatus = false;
            }
            var update = new BsonDocument("$set", new BsonDocument("Locked", updateStatus));
            Context.Groups.UpdateOneAsync(x => x.Id.Equals(new ObjectId(id)), update);
            return Json("");
        }

        //
        // GET: /Groups/id/MainPage/
        public ActionResult MainPage(string id)
        {
            ViewBag.cloudinary = cloudinary;
            ViewBag.currentUser = User.Identity.GetUserId();
            var group = Context.Groups.Find(x => x.Id.Equals(new ObjectId(id))).FirstOrDefault();
            return View(group);
        }

        //
        //GET: /Group/id/Members
        public ActionResult Members(string id)
        {
            var allUser = Context.Users.Find(_ => true).ToList();
            ViewBag.allUser = allUser;
            ViewBag.currentUser = User.Identity.GetUserId();
            ViewBag.cloudinary = cloudinary;
            var group = Context.Groups.Find(x => x.Id.Equals(new ObjectId(id))).FirstOrDefault();
            return View(group);
        }

        //
        //GET: /Groups/SetupMember/
        //[HttpPost]
        public ActionResult SetupMember(string groupId, string userId, int option)
        {
            var group = Context.Groups.Find(x => x.Id.Equals(new ObjectId(groupId))).FirstOrDefault();
            var updatedGroup = new Group();
            if (group.GroupMembers.Where(x => x.UserId.Equals(userId)).FirstOrDefault().RoleInGroup != "creator")
            {
                switch (option)
                {
                    case 1: // Thêm user vs role admin
                        var filter1 = Builders<Group>.Filter.Where(x => x.Id.Equals(new ObjectId(groupId)) && x.GroupMembers.Any(i => i.UserId.Equals(userId)));
                        var update1 = Builders<Group>.Update.Set(x => x.GroupMembers[-1].RoleInGroup, "admin");
                        Context.Groups.UpdateOneAsync(filter1, update1);
                        break;
                    case 2: // Chuyển role admin về user
                        var filter2 = Builders<Group>.Filter.Where(x => x.Id.Equals(new ObjectId(groupId)) && x.GroupMembers.Any(i => i.UserId.Equals(userId)));
                        var update2 = Builders<Group>.Update.Set(x => x.GroupMembers[-1].RoleInGroup, "user");
                        Context.Groups.UpdateOneAsync(filter2, update2);
                        break;
                    case 3: // Xóa user/admin khỏi nhóm
                        var filter3 = Builders<Group>.Filter.Where(x => x.Id.Equals(new ObjectId(groupId)));
                        var update3 = Builders<Group>.Update.PullFilter(x => x.GroupMembers, i => i.UserId.Equals(userId));
                        Context.Groups.UpdateOneAsync(filter3, update3);
                        break;
                }
                updatedGroup = Context.Groups.Find(x => x.Id.Equals(new ObjectId(groupId))).FirstOrDefault();
                return RedirectToAction("Members", updatedGroup);
                //return Json(new { url = Url.Action("Members", updatedGroup) });
            }
            else
            {
                updatedGroup = Context.Groups.Find(x => x.Id.Equals(new ObjectId(groupId))).FirstOrDefault();
                return Json(new { status = "error", message = "Can't interact with creator" });
            }
        }

        //
        // POST: /Groups/JoinLeaveGroup
        public ActionResult GroupRequestHandle(string groupId, string userId, int option)
        {
            var group = Context.Groups.Find(x => x.Id.Equals(new ObjectId(groupId))).FirstOrDefault();
            string message = "";
            switch (option)
            {
                case 1: // Tham gia nhóm (gửi request đến cho admin của nhóm)
                    if (group.ListJoinRequest.Find(x => x.Equals(userId)) == null)
                    {
                        var filter1 = Builders<Group>.Filter.Where(x => x.Id.Equals(new ObjectId(groupId)));
                        var update1 = Builders<Group>.Update.Push(x => x.ListJoinRequest, userId);
                        Context.Groups.UpdateOneAsync(filter1, update1);
                        message = "Wait for approved";
                    }
                    else
                    {
                        message = "Already requested";
                    }
                    break;
                case 2: // Rời nhóm 
                    var filter2 = Builders<Group>.Filter.Where(x => x.Id.Equals(new ObjectId(groupId)));
                    var update2 = Builders<Group>.Update.PullFilter(x => x.GroupMembers, i => i.UserId.Equals(userId));
                    Context.Groups.UpdateOneAsync(filter2, update2);
                    message = "Hope you change your mind";
                    break;
                case 3: // Chấp thuận 1 yêu cầu vào nhóm của user
                    // Bỏ user được chấp thuận khỏi list request
                    var filter3 = Builders<Group>.Filter.Where(x => x.Id.Equals(new ObjectId(groupId)));
                    var update3a = Builders<Group>.Update.Pull(x => x.ListJoinRequest, userId);
                    Context.Groups.UpdateOneAsync(filter3, update3a);
                    // Thêm user được chấp thuận vào list GroupMember
                    var update3b = Builders<Group>.Update.Push(x => x.GroupMembers, new GroupMembersViewModel()
                    {
                        UserId = userId,
                        RoleInGroup = "user"
                    });
                    Context.Groups.UpdateOneAsync(filter3, update3b);
                    break;
                case 4: // Từ chối 1 yêu cầu vào nhóm của user 
                    var filter4 = Builders<Group>.Filter.Where(x => x.Id.Equals(new ObjectId(groupId)));
                    var update4 = Builders<Group>.Update.Pull(x => x.ListJoinRequest, userId);
                    Context.Groups.UpdateOneAsync(filter4, update4);
                    break;
                case 5: // Chấp thuận tất cả các yêu cầu vào nhóm
                    var filter5 = Builders<Group>.Filter.Where(x => x.Id.Equals(new ObjectId(groupId)));
                    foreach (var request in group.ListJoinRequest)
                    {
                        var update5a = Builders<Group>.Update.Push(x => x.GroupMembers, new GroupMembersViewModel()
                        {
                            UserId = request,
                            RoleInGroup = "user"
                        });
                        Context.Groups.UpdateOneAsync(filter5, update5a);
                    }
                    var update5b = Builders<Group>.Update.PullAll(x => x.ListJoinRequest, group.ListJoinRequest);
                    Context.Groups.UpdateOneAsync(filter5, update5b);
                    break;
                case 6: // Từ chối tất cả các yêu cầu vào nhóm
                    var filter6 = Builders<Group>.Filter.Where(x => x.Id.Equals(new ObjectId(groupId)));
                    var update6 = Builders<Group>.Update.PullAll(x => x.ListJoinRequest, group.ListJoinRequest);
                    Context.Groups.UpdateOneAsync(filter6, update6);
                    break;
            }
            return Json(message);
        }

        //
        // GET: /Groups/id/Setting
        public ActionResult Setting(string id)
        {
            var group = Context.Groups.Find(x => x.Id.Equals(new ObjectId(id))).FirstOrDefault();
            var groupSetting = new GroupSettingViewModel()
            {
                Id = group.Id,
                CreatorId = group.CreatorId,
                GroupName = group.GroupName,
                Description = group.Description,
                Lock = group.Locked,
                Tag = group.Tag,
                GroupType = group.GroupType
            };
            ViewBag.currentUser = User.Identity.GetUserId();
            ViewBag.cloudinary = cloudinary;
            //var listMember = new List<GroupMembersViewModel>();
            //listMember = group.GroupMembers;
            //var listRequest = new List<string>();
            //listRequest = group.ListJoinRequest;
            //ViewBag.listMember = listMember;
            //ViewBag.listRequest = listRequest;
            ViewBag.group = group;
            var allUser = Context.Users.Find(_ => true).ToList();
            ViewBag.allUser = allUser;
            //var groupMembersViewModel = new List<GroupMembersViewModel>();
            //foreach (var user in allUser)
            //{
            //    groupMembersViewModel.Add(new GroupMembersViewModel()
            //    {
            //        UserId = user.Id,
            //        UserName = user.UserName,
            //        IsSelected = group.GroupMembers.Where(x => x.UserId == user.Id).Any()
            //    });
            //}
            //group.GroupMembers = groupMembersViewModel;
            return View(groupSetting);
        }

        //
        // POST: /Groups/id/Setting
        [HttpPost]
        public ActionResult Setting(GroupSettingViewModel groupSetting)
        {
            ViewBag.currentUser = User.Identity.GetUserId();
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                var filter = Builders<Group>.Filter.Where(x => x.Id.Equals(new ObjectId(groupSetting.Id)));
                var update = Builders<Group>.Update.
                    Set(x => x.GroupName, groupSetting.GroupName).
                    Set(x => x.Description, groupSetting.Description).
                    Set(x => x.GroupType, groupSetting.GroupType);
                Context.Groups.UpdateOneAsync(filter, update);
                var editGroup = Context.Groups.Find(x => x.Id.Equals(new ObjectId(groupSetting.Id))).FirstOrDefault();
                return RedirectToAction("Setting", "Groups", editGroup.Id);
            }
            ViewBag.cloudinary = cloudinary;
            return View(groupSetting);
        }

        //
        // GET: /Groups/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Groups/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Groups/Create
        [HttpPost]
        public ActionResult Create(Group group)
        {
            if (ModelState.IsValid)
            {
                // TODO: Add insert logic here
                var newGroup = new Group()
                {
                    GroupName = group.GroupName,
                    CreatorId = User.Identity.GetUserId(),
                    Tag = group.Tag,
                    GroupType = group.GroupType,
                    Description = group.Description
                };
                newGroup.GroupMembers.Add(new GroupMembersViewModel()
                {
                    UserId = User.Identity.GetUserId(),
                    RoleInGroup = "creator"
                });
                Context.Groups.InsertOneAsync(newGroup);
                return RedirectToAction("Index");
            }
            return View();
        }

        //
        // GET: /Groups/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Groups/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Groups/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Groups/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //DangVH. Create. Start (17/11/2016)
        [HttpPost]
        public ActionResult ChangeImage(string id, HttpPostedFileBase file, int option)
        {
            try
            {
                var uploadResult = ImageUploadHelper.GetUploadResult(file);
                switch (option)
                {
                    case 1:
                        var update1 = new BsonDocument("$set", new BsonDocument("AvaImg", uploadResult.PublicId));
                        Context.Groups.UpdateOneAsync(x => x.Id.Equals(new ObjectId(id)), update1);
                        break;
                    case 2:
                        var update2 = new BsonDocument("$set", new BsonDocument("CoverImg", uploadResult.PublicId));
                        Context.Groups.UpdateOneAsync(x => x.Id.Equals(new ObjectId(id)), update2);
                        break;
                }
                return Json("Đổi ảnh hoàn tất");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
        //DangVH. Create. End (17/11/2016)
    }
}
