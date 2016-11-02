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

namespace BSNCapstone.Controllers
{
    public class SliderController : Controller
    {
        private readonly ApplicationIdentityContext Context = ApplicationIdentityContext.Create();
        //DangVH. Create. Start (02/11/2016)
        private readonly CloudinaryDotNet.Cloudinary cloudinary = ImageUploadHelper.GetCloudinaryAccount();
        //DangVH. Create. End (02/11/2016)
        //
        // GET: /Slider/
        public ActionResult Index()
        {
            //Configuration
            //DangVH. Delete. Start (02/11/2016)
            //Account account = new Account("dsddvwiqz", "677568653855233", "_RCoQNjMqr8Nt7-FAgs5T_guiWM");
            //var cloudinary = new Cloudinary(account);
            //var cloudinary = ImageUploadHelper.GetCloudinaryAccount();
            //DangVH. Delete. End (02/11/2016)

            //List of photos from MongoDB
            List<Slider> listMgPhoto = Context.Photos.Find(_ => true).ToList();
            return View(new MongoPhotoModel(cloudinary, listMgPhoto));
        }

        [HttpPost]
        public ActionResult Upload(string desc)
        {
            try
            {
                //Configuration
                //DangVH. Delete. Start (02/11/2016)
                //Account account = new Account("dsddvwiqz", "677568653855233", "_RCoQNjMqr8Nt7-FAgs5T_guiWM");
                //var cloudinary = new Cloudinary(account);
                //var cloudinary = ImageUploadHelper.GetCloudinaryAccount();
                //DangVH. Delete. End (02/11/2016)
                var file = Request.Files[0];

                if (file == null)
                {
                    return Json(new { result = false });
                }
                //Upload Image
                //DangVH. Update. Start (01/11/2016)
                    //var uploadParam = new ImageUploadParams()
                    //{
                    //    File = new FileDescription(file.FileName, file.InputStream),
                    //};

                    //var uploadResult = cloudinary.Upload(uploadParam);
                var uploadResult = ImageUploadHelper.GetUploadResult(file);
                //DangVH. Update. End (01/11/2016)

                //Save photo (publicID) to MongoDB
                Slider mongoPhoto = new Slider()
                {
                    PublicId = uploadResult.PublicId,
                    Desc = desc
                };
                Context.Photos.InsertOneAsync(mongoPhoto);

                return Json("File Uploaded Successfully!");
            }
            catch (Exception ex)
            {
                return Json("Error occurred. Error details: " + ex.Message);
            }
        }

        [HttpPost]
        public ActionResult DeleteConfirmed(string id)
        {
            Context.Photos.DeleteOneAsync(x => x.Id.Equals(new ObjectId(id)));
            return Json("File Delete Successfully!");
        }
	}
}