using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BSNCapstone.Models
{
    public class Comment
    {
        public int CommentId { get; set; } //thêm vào cho phần add like
        public string PostId { get; set; }
        [Required]
        public string Message { get; set; }
        public string CommentedBy { get; set; }
        public System.DateTime CommentedDate { get; set; }
    }
}