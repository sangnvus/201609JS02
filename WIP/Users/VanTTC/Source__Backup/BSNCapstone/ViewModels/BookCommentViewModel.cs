using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace BSNCapstone.ViewModels
{
    public class BookCommentViewModel
    {
        //[BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        public string UserId { get; set; }

        public DateTime CreatedTime { get; set; }

        public string Description { get; set; }

        public List<string> LinesDescription { get; set; }

        public BookCommentViewModel()
        {
            LinesDescription = new List<string>();
        }
    }
}