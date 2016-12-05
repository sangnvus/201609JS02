using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace BSNCapstone.Models
{
    public class Report
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string UserId { get; set; }

        public string ReportedUserId { get; set; }

        public string ReportedGroupId { get; set; }

        public string Content { get; set; }

        public bool Status { get; set; }
    }
}