using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BSNCapstone.Models
{
    public class Slider
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public String Id { get; set; }

        [Required]
        public string PublicId { get; set; }

        public string Desc { get; set; }

        // HuyenPT. Add. Start. 06-12-2016
        public bool isShow { get; set; }
        // HuyenPT. Add. End. 06-12-2016
    }
}