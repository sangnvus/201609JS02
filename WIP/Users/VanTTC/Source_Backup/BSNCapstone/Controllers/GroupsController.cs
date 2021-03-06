﻿using System;
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
                        message = "Xin chờ để được chấp thuận";
                    }
                    else
                    {
                        message = "Bạn đã yêu cầu";
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
                case 7: // Chuyền 1 member thành admin
                    var filter7 = Builders<Group>.Filter.Where(x => x.Id.Equals(new ObjectId(groupId)) && x.GroupMembers.Any(i => i.UserId.Equals(userId)));
                    var update7 = Builders<Group>.Update.Set(x => x.GroupMembers[-1].RoleInGroup, "admin");
                    Context.Groups.UpdateOneAsync(filter7, update7);
                    break;
                case 8: // Chuyển role admin về user
                    var filter8 = Builders<Group>.Filter.Where(x => x.Id.Equals(new ObjectId(groupId)) && x.GroupMembers.Any(i => i.UserId.Equals(userId)));
                    var update8 = Builders<Group>.Update.Set(x => x.GroupMembers[-1].RoleInGroup, "user");
                    Context.Groups.UpdateOneAsync(filter8, update8);
                    break;
                case 9: // Xóa user/admin khỏi nhóm
                    var filter9 = Builders<Group>.Filter.Where(x => x.Id.Equals(new ObjectId(groupId)));
                    var update9 = Builders<Group>.Update.PullFilter(x => x.GroupMembers, i => i.UserId.Equals(userId));
                    Context.Groups.UpdateOneAsync(filter9, update9);
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
            ViewBag.group = group;
            var allUser = Context.Users.Find(_ => true).ToList();
            ViewBag.allUser = allUser;
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
                return RedirectToAction("MainPage", "Groups", editGroup.Id);
            }
            var group = Context.Groups.Find(x => x.Id.Equals(new ObjectId(groupSetting.Id))).FirstOrDefault();
            ViewBag.cloudinary = cloudinary;
            ViewBag.group = group;
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
        public ActionResult Create(string groupName, string groupTag, string groupDesc, string groupType)
        {
            try
            {
                // TODO: Add insert logic here
                var groups = Context.Groups.Find(_ => true).ToList();
                List<string> errorMessage = new List<string>();
                if (groupName == "" || groupName == null)
                {
                    var message = "Bạn phải điền tên nhóm";
                    errorMessage.Add(message);
                }
                if (groupTag == "" || groupTag == null)
                {
                    var message = "Bạn phải chọn thẻ nhóm";
                    errorMessage.Add(message);
                }
                if (groupType == "undefined")
                {
                    var message = "Bạn phải chọn thể loại nhóm";
                    errorMessage.Add(message);
                }
                foreach (var group in groups)
                {
                    if (group.GroupName.ToLower().Equals(groupName.ToLower()))
                    {
                        var message = "Tên nhóm đã tồn tại";
                        errorMessage.Add(message);
                    }
                }
                if (errorMessage.Count() == 0)
                {
                    var newGroup = new Group()
                    {
                        GroupName = groupName,
                        CreatorId = User.Identity.GetUserId(),
                        Tag = groupTag,
                        GroupType = groupType,
                        Description = groupDesc
                    };
                    newGroup.GroupMembers.Add(new GroupMembersViewModel()
                    {
                        UserId = User.Identity.GetUserId(),
                        RoleInGroup = "creator"
                    });
                    Context.Groups.InsertOneAsync(newGroup);
                    var justAddedGroup = Context.Groups.Find(x => x.GroupName.Equals(newGroup.GroupName)).FirstOrDefault().Id;
                    return Json(new
                    {
                        redirectUrl = Url.Action("MainPage", "Groups", new { id = justAddedGroup })
                    });
                }
                else
                {
                    return Json(errorMessage, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
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

        [HttpPost]
        public ActionResult Report()
        {
            return Json("");
        }
    }
}
