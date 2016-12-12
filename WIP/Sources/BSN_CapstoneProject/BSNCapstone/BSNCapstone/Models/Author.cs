using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSNCapstone.Models
{
    public class Author
    {
        //[BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        public string UserId { get; set; }

        public string AuthorName { get; set; }

        public string AuthorImg { get; set; }

        public Author()
        {
            UserId = "";
            AuthorImg = "";
        }
    }
}