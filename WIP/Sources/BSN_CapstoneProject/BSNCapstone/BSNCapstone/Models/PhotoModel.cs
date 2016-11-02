using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace BSNCapstone.Models
{
    public class PhotoModel
    {
        public Cloudinary Cloudinary { get; set; }

        public PhotoModel(Cloudinary cloudinary)
        {
            Cloudinary = cloudinary;
        }
    }

    public class MongoPhotoModel : PhotoModel
    {
        public List<Slider> Sliders { get; set; }

        public MongoPhotoModel(Cloudinary cloudinary, List<Slider> sliders)
            : base(cloudinary)
        {
            Sliders = sliders;
        }
    }
}