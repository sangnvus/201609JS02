using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bookaholic.App_Start;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using Bookaholic.Models;
using MongoDB.Driver;
using Bookaholic.ViewModels;

namespace Bookaholic.ControllerHelpers
{
    public class CategoriesControllerHelper
    {
        public static int CheckCategory(string Id)
        {
            int status = 0;
            ApplicationIdentityContext Context = ApplicationIdentityContext.Create();
            var books = Context.Books.Find(_ => true).ToEnumerable();
            foreach (var book in books)
            {
                if (book.Categories.Where(x => x.CategoryId.Equals(Id)) == null)
                {
                    status = 1;
                }
                else status = 0;
            }
            return status;
        }
    }
}