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

namespace BSNCapstone.Controllers
{
    public class SliderController : Controller
    {
        private readonly ApplicationIdentityContext Context = ApplicationIdentityContext.Create();
        //
        // GET: /Slider/
        public ActionResult Index()
        {
            //Configuration
            Account account = new Account("dsddvwiqz", "677568653855233", "_RCoQNjMqr8Nt7-FAgs5T_guiWM");
            var cloudinary = new Cloudinary(account);

            //List of photos from MongoDB
            List<MongoPhoto> listMgPhoto = Context.Photos.Find(_ => true).ToList();
            return View(new MongoPhotoModel(cloudinary, listMgPhoto));
        }

        [HttpPost]
        public ActionResult Upload(string desc)
        {
            try
            {
                //Configuration
                Account account = new Account("dsddvwiqz", "677568653855233", "_RCoQNjMqr8Nt7-FAgs5T_guiWM");
                var cloudinary = new Cloudinary(account);
                var file = Request.Files[0];

                if (file == null)
                {
                    return Json(new { result = false });
                }
                //Upload Image
                var uploadParam = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, file.InputStream),
                };

                var uploadResult = cloudinary.Upload(uploadParam);

                //Save photo (publicID) to MongoDB
                MongoPhoto mongoPhoto = new MongoPhoto()
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