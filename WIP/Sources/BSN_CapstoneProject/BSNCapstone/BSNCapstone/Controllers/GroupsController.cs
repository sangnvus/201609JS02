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
using PagedList;

namespace BSNCapstone.Controllers
{
    [Authorize]
    public class GroupsController : Controller
    {
        private readonly ApplicationIdentityContext Context = ApplicationIdentityContext.Create();
        private readonly CloudinaryDotNet.Cloudinary cloudinary = ImageUploadHelper.GetCloudinaryAccount();
        //Lấy giá trị cho Multiselectlist
        private MultiSelectList GetUserForAdd(string[] selectedValues, string groupId)
        {
            var groupMembers = Context.Groups.Find(x => x.Id.Equals(new ObjectId(groupId))).FirstOrDefault().GroupMembers;
            List<ApplicationUser> userForAdd = new List<ApplicationUser>();
            var userId = User.Identity.GetUserId();
            var followers = Context.Users.Find(x => x.Id.Equals(new ObjectId(userId))).FirstOrDefault().Follower;
            foreach (var item in followers)
            {
                if (groupMembers.Find(x => x.UserId.Equals(item)) == null)
                {
                    userForAdd.Add(Context.Users.Find(x => x.Id.Equals(new ObjectId(item))).FirstOrDefault());
                }
            }
            Console.Write(userForAdd);
            return new MultiSelectList(userForAdd, "Id", "UserName", selectedValues);
        }

        private List<string> GroupReportContent()
        {
            List<string> list = new List<string>();
            foreach (var x in ReportContentViewModel.EnumToList<ReportContentViewModel.ReportGroup>())
            {
                list.Add(ReportContentViewModel.GetEnumDescription(x));
            }
            return list;
        }

        //
        // GET: /Groups/
        public ActionResult Index(string searchString, string currentFilter, int? page)
        {
            ViewBag.groupNumber = GroupsControllerHelper.GetGroupNumber();
            ViewBag.groupJustCreated = GroupsControllerHelper.GetGroupJustCreatedNumber();
            var groups = Context.Groups.Find(_ => true).ToEnumerable();
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.currentFilter = searchString;
            ViewBag.groups = Context.Groups.Find(_ => true).ToList();
            ViewBag.books = Context.Books.Find(_ => true).ToList();
            if (!string.IsNullOrEmpty(searchString))
            {
                groups = groups.Where(x => x.GroupName.Contains(searchString) || x.Tag.Contains(searchString));
            }
            int pageSize = 6;
            int pageNumber = (page ?? 1);
            return View(groups.ToPagedList(pageNumber, pageSize));
        }

        //
        // POST: /Groups/Lock
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
            Context.Groups.UpdateOneAsync(x => x.Id.Equals(new ObjectId(id)), update);
            return Json("");
        }

        //
        // GET: /Groups/id/MainPage/
        public ActionResult MainPage(string id)
        {
            ViewBag.cloudinary = cloudinary;
            ViewBag.currentUser = User.Identity.GetUserId();
            ViewBag.allUser = Context.Users.Find(_ => true).ToList();
            ViewBag.groupReport = GroupReportContent();
            var group = Context.Groups.Find(x => x.Id.Equals(new ObjectId(id))).FirstOrDefault();
            if (group.Locked == true)
            {
                return RedirectToAction("LockedPage", "Home");
            }
            else
            {
                return View(group);
            }
        }

        //
        //GET: /Group/id/Members
        public ActionResult Members(string id)
        {
            var allUser = Context.Users.Find(_ => true).ToList();
            var group = Context.Groups.Find(x => x.Id.Equals(new ObjectId(id))).FirstOrDefault();
            ViewBag.allUser = allUser;
            ViewBag.currentUser = User.Identity.GetUserId();
            ViewBag.userForAdd = GetUserForAdd(null, group.Id);
            ViewBag.groupReport = GroupReportContent();
            ViewBag.cloudinary = cloudinary;
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
            ViewBag.groupReport = GroupReportContent();
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

        [HttpPost]
        public ActionResult AddNewMember(string addUser, string groupId)
        {
            if (addUser == "")
            {
                return Json("Không có người dùng nào được thêm vào nhóm.");
            }
            else
            {
                var filter = Builders<Group>.Filter.Where(x => x.Id.Equals(new ObjectId(groupId)));
                var newMembers = addUser.Split(new[] { "," }, StringSplitOptions.None).ToList();
                foreach (var member in newMembers)
                {
                    var update = Builders<Group>.Update.Push(x => x.GroupMembers, new GroupMembersViewModel()
                    {
                        UserId = member.ToString(),
                        RoleInGroup = "user"
                    });
                    Context.Groups.UpdateOneAsync(filter, update);
                }
            }
            return Json("Thêm thành công.");
        }
    }
}
