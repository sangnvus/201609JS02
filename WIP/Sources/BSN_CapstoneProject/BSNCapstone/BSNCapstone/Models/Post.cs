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
        public Post()
        {
            this.PostComments = new HashSet<Comment>();
        }

        [BsonRepresentation(BsonType.ObjectId)]
        public String Id { get; set; }

        public string Message { get; set; }

        public int PostedBy { get; set; }

        public System.DateTime PostedDate { get; set; }

        public virtual ICollection<Comment> PostComments { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
}