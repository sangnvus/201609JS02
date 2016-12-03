using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace BSNCapstone.Models
{
    public class BookStatistic
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string BookId { get; set; }

        [DataType(DataType.Date)]
        public DateTime EachDate { get; set; }

        public int Count { get; set; }

        public BookStatistic()
        {
            Count = 1;
        }
    }
}