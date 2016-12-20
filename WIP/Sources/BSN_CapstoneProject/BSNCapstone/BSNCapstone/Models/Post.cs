using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BSNCapstone.Models
{
    public class Post
    {
        public Post()
        {
            this.PostComments = new HashSet<Comment>();
            this.PostLikes = new HashSet<PostLike>();
        }

        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Required]
        public string Message { get; set; }

        public string PostedById { get; set; }

        public System.DateTime PostedDate { get; set; }

        public string GroupId { get; set; }

        public virtual ICollection<Comment> PostComments { get; set; }

        public virtual ICollection<PostLike> PostLikes { get; set; }
    }
}