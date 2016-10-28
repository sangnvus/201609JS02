using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BSNCapstone.Models;
using MongoDB.Bson;
using System.Web.Mvc;

namespace BSNCapstone.ViewModels
{
    public class BookCategoriesViewModel
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool IsSelected { get; set; }
    }
}