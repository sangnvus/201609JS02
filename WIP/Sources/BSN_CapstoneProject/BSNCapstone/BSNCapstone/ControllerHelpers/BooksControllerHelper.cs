using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BSNCapstone.App_Start;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using BSNCapstone.Models;
using BSNCapstone.ViewModels;
using MongoDB.Driver;

namespace BSNCapstone.ControllerHelpers
{
    public class BooksControllerHelper
    {
        //DangVH. Create. Start (02/11/2016)
        public static int GetBookNumber()
        {
            ApplicationIdentityContext Context = ApplicationIdentityContext.Create();
            return (int)Context.Books.Find(_ => true).Count();
        }
        //DangVH. Create. End (02/11/2016)

        public static List<Category> ListAllCategory()
        {
            ApplicationIdentityContext Context = ApplicationIdentityContext.Create();
            List<Category> categories = Context.Categories.Find(_ => true).ToList();
            return categories;
        }

        public static Models.Book GetCheckBoxValues()
        {
            ApplicationIdentityContext Context = ApplicationIdentityContext.Create();
            Book book = new Book();
            var allCategory = Context.Categories.Find(_ => true).ToEnumerable();
            var bookCategoriesViewModel = new List<BookCategoriesViewModel>();
            foreach (var category in allCategory)
            {
                bookCategoriesViewModel.Add(new BookCategoriesViewModel()
                {
                    CategoryId = category.Id,
                    CategoryName = category.CategoryName,
                    IsSelected = false
                });
            }
            book.Categories = bookCategoriesViewModel;
            return book;
        }
    }
}