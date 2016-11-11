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

    public class SliderPhotoModel : PhotoModel
    {
        public List<Slider> Sliders { get; set; }

        public SliderPhotoModel(Cloudinary cloudinary, List<Slider> sliders)
            : base(cloudinary)
        {
            Sliders = sliders;
        }
    }
    // HuyenPT. Create. Start. 02-11-2016
    public class PublisherPhotoModel : PhotoModel
    {
        public List<Publisher> Publishers { get; set; }

        public PublisherPhotoModel(Cloudinary cloudinary, List<Publisher> publishers)
            : base(cloudinary)
        {
            Publishers = publishers;
        }
    }
    // HuyenPT. Create. End. 02-11-2016
}