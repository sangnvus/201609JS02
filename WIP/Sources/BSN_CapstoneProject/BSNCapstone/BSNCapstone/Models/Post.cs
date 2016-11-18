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
        public string Id { get; set; }

        public string Message { get; set; }

        // HuyenPT. Update. Start. 19-11-2016
        //public int PostedBy { get; set; }
        public string PostedBy { get; set; }
        // HuyenPT. Update. End. 19-11-2016

        public System.DateTime PostedDate { get; set; }

        public virtual ICollection<Comment> PostComments { get; set; }
        // HuyenPT. Delete. Start. 19-11-2016
        //public virtual UserProfile UserProfile { get; set; }
        // HuyenPT. Delete. End. 19-11-2016
    }
}