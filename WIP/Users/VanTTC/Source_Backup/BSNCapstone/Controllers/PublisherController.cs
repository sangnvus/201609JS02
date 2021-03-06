﻿using System;
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
    public class PublisherController : Controller
    {
        private readonly ApplicationIdentityContext Context = ApplicationIdentityContext.Create();
        private readonly CloudinaryDotNet.Cloudinary cloudinary = ImageUploadHelper.GetCloudinaryAccount();
        //
        // GET: /Publisher/
        public ActionResult Index()
        {
            List<Publisher> publishers = Context.Publishers.Find(_ => true).ToList();
            return View(new PublisherPhotoModel(cloudinary, publishers));
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase pubImage, string pubName, string pubAddress, string pubPhoneNo)
        {
            try
            {
                // Upload image to Cloudinary
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

                return Json("File Uploaded Successfully!");
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
    }
}