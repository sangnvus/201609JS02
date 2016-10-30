using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Demo_Cloudinary_Mongo.Models
{
    public class Model
    {
        public Cloudinary Cloudinary { get; set; }

        public Model(Cloudinary cloudinary)
        {
            Cloudinary = cloudinary;
        }
    }

    public class CloudinaryPhotoModel : Model
    {
        public CloudinaryPhoto CloudinaryPhoto { get; set; }

        public CloudinaryPhotoModel(Cloudinary cloudinary, CloudinaryPhoto cloudinayPhoto)
            : base(cloudinary)
        {
            CloudinaryPhoto = cloudinayPhoto;
        }
    }

    public class MongoPhotoModel : Model
    {
        public List<MongoPhoto> MongoPhotos { get; set; }

        public MongoPhotoModel(Cloudinary cloudinary, List<MongoPhoto> mongoPhotos)
            : base(cloudinary)
        {
            MongoPhotos = mongoPhotos;
        }
    }
}