using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSNCapstone.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public String PostId { get; set; }

        public string Message { get; set; }

        public int CommentedBy { get; set; }

        public System.DateTime CommentedDate { get; set; }

        public virtual Post Post { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
}