using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bookaholic.Models;
using MongoDB.Bson;
using System.Web.Mvc;

namespace Bookaholic.ViewModels
{
    public class BookCategoriesViewModel
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool IsSelected { get; set; }
    }
}