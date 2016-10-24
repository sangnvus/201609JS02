using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Demo_Cloudinary_Mongo.Models;
using System.Runtime.Remoting.Contexts;
using MongoDB.Bson;

namespace Demo_Cloudinary_Mongo.Controllers
{
    public class HomeController : Controller
    {
        private readonly DBContext Context = new DBContext();

        public ActionResult ListPhoto()
        {
            //Configuration
            Account account = new Account("dsddvwiqz", "677568653855233", "_RCoQNjMqr8Nt7-FAgs5T_guiWM");
            var cloudinary = new Cloudinary(account);

            //List of photos from MongoDB
            List<MongoPhoto> listMgPhoto = Context.Photos.FindAll().ToList();
            return View(new MongoPhotoModel(cloudinary, listMgPhoto));
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            //Configuration
            Account account = new Account("dsddvwiqz", "677568653855233", "_RCoQNjMqr8Nt7-FAgs5T_guiWM");
            var cloudinary = new Cloudinary(account);

            if (file == null)
            {
                return View();
            }
            //Upload Image
            var uploadParam = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, file.InputStream),
            };

            var uploadResult = cloudinary.Upload(uploadParam);

            CloudinaryPhoto cloudinaryPhoto = new CloudinaryPhoto()
            {
                Bytes = (int)uploadResult.Length,
                CreatedAt = DateTime.Now,
                Format = uploadResult.Format,
                Height = uploadResult.Height,
                Path = uploadResult.Uri.AbsolutePath,
                PublicId = uploadResult.PublicId,
                ResourceType = uploadResult.ResourceType,
                SecureUrl = uploadResult.SecureUri.AbsoluteUri,
                Signature = uploadResult.Signature,
                Type = uploadResult.JsonObj["type"].ToString(),
                Url = uploadResult.Uri.AbsoluteUri,
                Version = Int32.Parse(uploadResult.Version),
                Width = uploadResult.Width,
            };

            //Save photo (publicID) to MongoDB
            MongoPhoto mongoPhoto = new MongoPhoto();
            mongoPhoto.PublicId = uploadResult.PublicId;
            Context.Photos.Insert(mongoPhoto);

            //Display a full image
            return PartialView("UploadSucceeded", new CloudinaryPhotoModel(cloudinary, cloudinaryPhoto));
        }
    }
}