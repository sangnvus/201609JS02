using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSNCapstone.Models
{
    public class UserProfile
    {
        public UserProfile()
        {
            this.PostComments = new HashSet<Comment>();
            this.Posts = new HashSet<Post>();
        }

        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string AvatarExt { get; set; }

        public virtual ICollection<Comment> PostComments { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}