using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BSNCapstone.Models
{
    public class MongoPhoto
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public String Id { get; set; }

        [Required]
        public string PublicId { get; set; }

        public string Desc { get; set; }
    }
}