using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BSNCapstone.Models;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;

namespace BSNCapstone.ControllerHelpers
{
    public class ImageUploadHelper
    {
        public static Cloudinary GetCloudinaryAccount()
        {
            Account account = new Account("dsddvwiqz", "677568653855233", "_RCoQNjMqr8Nt7-FAgs5T_guiWM");
            var cloudinary = new Cloudinary(account);
            return cloudinary;
        }
    }
}