using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSNCapstone.Models
{
    public class Post
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string UserID { get; set; }

        public string GroupId { get; set; }

        public DateTime Time { get; set; }

        public string BookId { get; set; }

        public string Content { get; set; }

        public List<Post> Posts { get; set; }
    }
}