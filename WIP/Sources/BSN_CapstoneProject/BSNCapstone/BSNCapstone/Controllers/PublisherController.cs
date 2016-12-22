using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using BSNCapstone.Models;
using System.Runtime.Remoting.Contexts;
using MongoDB.Driver;
using MongoDB.Bson;
using System.IO;
using MongoDB.Driver.Builders;
using BSNCapstone.App_Start;
using BSNCapstone.ControllerHelpers;
using PagedList;

namespace BSNCapstone.Controllers
{
    public class PublisherController : Controller
    {
        private readonly ApplicationIdentityContext Context = ApplicationIdentityContext.Create();
        private readonly CloudinaryDotNet.Cloudinary cloudinary = ImageUploadHelper.GetCloudinaryAccount();
        //
        // GET: /Publisher/
        public ActionResult Index(int? page)
        {
            ViewBag.cloudinary = cloudinary;
            List<Publisher> publishers = Context.Publishers.Find(_ => true).ToList();
            int pageSize = 6;
            int pageNumber = (page ?? 1);
            return View(publishers.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase pubImage, string pubName, string pubAddress, string pubPhoneNo)
        {
            try
            {
                // Upload image to Cloudinary
                var allPublisher = Context.Publishers.Find(_ => true).ToList();
                List<string> errorMessage = new List<string>();
                if (pubName == "" || pubName == null)
                {
                    var message = "Tên NXB không được để trống";
                    errorMessage.Add(message);
                }
                if (pubAddress == "" || pubAddress == null)
                {
                    var message = "Địa chỉ không được để trống";
                    errorMessage.Add(message);
                }
                if (pubImage == null)
                {
                    var message = "Ảnh NXB không được để trống";
                    errorMessage.Add(message);
                }
                foreach (var pub in allPublisher)
                {
                    if (pub.Name.ToLower().Equals(pubName.ToLower()))
                    {
                        var message = "Tên NXB đã tồn tại";
                        errorMessage.Add(message);
                    }
                }
                if (errorMessage.Count() == 0)
                {
                    var uploadResult = ImageUploadHelper.GetUploadResult(pubImage);

                    // Save new publisher to MongoDB
                    Publisher publisher = new Publisher()
                    {
                        ImagePublicId = uploadResult.PublicId,
                        Name = pubName,
                        Address = pubAddress,
                        PhoneNumber = pubPhoneNo
                    };
                    Context.Publishers.InsertOneAsync(publisher);

                    return Json(new {
                        success = "Thêm nhà xuất bản thành công"
                    });
                }
                else
                {
                    return Json(errorMessage, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json("Upload Failed. Error occurred. Error details: " + ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            Context.Publishers.DeleteOneAsync(x => x.Id.Equals(new ObjectId(id)));
            return Json("File Delete Successfully!");
        }

        [HttpPost]
        public ActionResult Edit(string pubId, HttpPostedFileBase pubEditedImage, string pubEditedName, string pubEditedAddress, string pubEditedPhoneNo)
        {
            try
            {
                var allPublisher = Context.Publishers.Find(_ => true).ToList();
                List<string> errorMessage = new List<string>();
                if (pubEditedName == "" || pubEditedName == null)
                {
                    var message = "Tên NXB không được để trống";
                    errorMessage.Add(message);
                }
                if (pubEditedAddress == "" || pubEditedAddress == null)
                {
                    var message = "Địa chỉ không được để trống";
                    errorMessage.Add(message);
                }
                foreach (var pub in allPublisher)
                {
                    if (pub.Name.ToLower().Equals(pubEditedName.ToLower()) &&
                        allPublisher.Find(x => x.Name.ToLower().Equals(pubEditedName.ToLower())).Id != pubId)
                    {
                        var message = "Tên NXB đã tồn tại";
                        errorMessage.Add(message);
                    }
                }
                if (errorMessage.Count() == 0)
                {
                    var filter = Builders<Publisher>.Filter.Eq(x => x.Id, pubId);
                    // Check if user change image or not
                    if (pubEditedImage == null)
                    {
                        var update = Builders<Publisher>.Update.
                        Set(x => x.Name, pubEditedName).Set(x => x.Address, pubEditedAddress).
                        Set(x => x.PhoneNumber, pubEditedPhoneNo);
                        Context.Publishers.FindOneAndUpdate(filter, update);
                    }
                    else
                    {
                        // Upload image to Cloudinary
                        var uploadResult = ImageUploadHelper.GetUploadResult(pubEditedImage);
                        var update = Builders<Publisher>.Update.Set(x => x.ImagePublicId, uploadResult.PublicId).
                        Set(x => x.Name, pubEditedName).Set(x => x.Address, pubEditedAddress).
                        Set(x => x.PhoneNumber, pubEditedPhoneNo);
                        Context.Publishers.FindOneAndUpdate(filter, update);
                    }
                    return Json(new {
                        success = "Chỉnh sửa thông tin nhà xuất bản thành công!"
                    });
                }
                else
                {
                    return Json(errorMessage, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json("Rất tiếc! Đã xảy ra lỗi trong quá trình xử lý");
            }
        }
    }
}