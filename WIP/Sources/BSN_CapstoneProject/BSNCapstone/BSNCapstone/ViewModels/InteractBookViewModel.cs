using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace BSNCapstone.ViewModels
{
    public class InteractBookViewModel
    {
        public ObjectId Id { get; set; }

        public string BookId { get; set; }

        public DateTime InteractTime { get; set; }
    }
}