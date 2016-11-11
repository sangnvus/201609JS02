using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BSNCapstone.App_Start;
using BSNCapstone.Models;
using BSNCapstone.ControllerHelpers;
using BSNCapstone.ViewModels;
using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.AspNet.Identity;

namespace BSNCapstone.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private readonly ApplicationIdentityContext Context = ApplicationIdentityContext.Create();
        //DangVH. Create. Start (02/11/2016)
        private readonly CloudinaryDotNet.Cloudinary cloudinary = ImageUploadHelper.GetCloudinaryAccount();
        //DangVH. Create. End (02/11/2016)
        //
        // GET: /Books/
        public ActionResult Index(string searchString)
        {
            //DangVH. Create. Start (02/11/2016)
            ViewBag.currentUser = User.Identity.GetUserName();
            ViewBag.bookNumber = BooksControllerHelper.GetBookNumber();
            //DangVH. Create. End (02/11/2016)
            ViewBag.allCategories = BooksControllerHelper.ListAllCategory();
            ViewBag.allPublishers = Context.Publishers.Find(_ => true).ToList();
            var books = Context.Books.Find(_ => true).ToEnumerable();
            //DangVH. Create. Start (02/11/2016)
            if (!string.IsNullOrEmpty(searchString))
            {
                books = books.Where(x => x.BookName.Contains(searchString) || x.Authors.Contains(searchString));
            }
            //DangVH. Create. End (02/11/2016)
            return View(books);
        }

        //
        // GET: /Books/Details/5
        public ActionResult Details(string id)
        {
            //DangVH. Create. Start (02/11/2016)
            ViewBag.cloudinary = cloudinary;
            //DangVH. Create. End (02/11/2016)
            var book = Context.Books.Find(x => x.Id.Equals(new ObjectId(id))).FirstOrDefault();
            var allCategory = Context.Categories.Find(_ => true).ToEnumerable();
            //List<Category> listCategory = new List<Category>();
            //foreach (var cat in book.Categories)
            //{
            //    listCategory.Add(new Category()
            //    {
            //        CategoryName = allCategory.Where(x => x.Id.Equals(cat.CategoryId)).First().CategoryName
            //    });
            //}
            //ViewBag.ListCategory = listCategory;
            return View(book);
        }

        private MultiSelectList GetPubsCats(string[] selectedValues, int option)
        {
            if (option == 1)
            {
                var publishers = Context.Publishers.Find(_ => true).ToEnumerable();
                return new MultiSelectList(publishers, "Id", "Name", selectedValues);
            }
            else
            {
                var categories = Context.Categories.Find(_ => true).ToEnumerable();
                return new MultiSelectList(categories, "Id", "CategoryName", selectedValues);
            }
        }

        //
        // GET: /Books/Create
        public ActionResult Create()
        {
            //Book book = BooksControllerHelper.GetCheckBoxValues();
            ViewBag.bookNumber = BooksControllerHelper.GetBookNumber();
            //DangVH. Create. Start (10/11/2016)
            //var publishers = Context.Publishers.Find(_ => true).ToEnumerable();
            ViewBag.CategoryId = GetPubsCats(null, 2);
            ViewBag.PublisherId = GetPubsCats(null, 1);
            //DangVH. Create. End (10/11/2016)
            //return PartialView("_Create", book);
            return View();
        }

        //
        // POST: /Books/Create
        [HttpPost]
        public ActionResult Create(Book book)
        {
            if (ModelState.IsValid)
            {
                // TODO: Add insert logic here
                //var selectedCategories = book.Categories.Where(x => x.IsSelected).Select(x => x.CategoryId).ToList();
                //DangVH. Create. Start (01/11/2016)
                var file = Request.Files[0];
                var uploadResult = ImageUploadHelper.GetUploadResult(file);
                //DangVH. Create. End (01/11/2016)
                var addBook = new Book()
                {
                    BookName = book.BookName,
                    Authors = book.Authors,
                    ReleaseDay = book.ReleaseDay.ToLocalTime(),
                    Description = book.Description,
                    ImgPublicId = uploadResult.PublicId
                };

                foreach (var publisherId in book.Publishers)
                {
                    addBook.Publishers.Add(publisherId);
                }

                foreach (var categoryId in book.Categories)
                {
                    addBook.Categories.Add(categoryId);
                }
                //foreach (var categoryId in selectedCategories)
                //{
                //    addBook.Categories.Add(new BookCategoriesViewModel()
                //    {
                //        CategoryId = categoryId
                //    });
                //}
                Context.Books.InsertOneAsync(addBook);
                return RedirectToAction("Index");
            }
            //Book newbook = BooksControllerHelper.GetCheckBoxValues();
            ViewBag.bookNumber = BooksControllerHelper.GetBookNumber();
            return View();
        }

        //
        // GET: /Books/Edit/5
        public ActionResult Edit(string id)
        {
            //DangVH. Create. Start (02/11/2016)
            ViewBag.cloudinary = cloudinary;
            //DangVH. Create. End (02/11/2016)
            ViewBag.bookNumber = BooksControllerHelper.GetBookNumber();
            var book = Context.Books.Find(x => x.Id.Equals(new ObjectId(id))).FirstOrDefault();
            var allCategory = Context.Categories.Find(_ => true).ToEnumerable();
            var bookCategoriesViewModel = new List<BookCategoriesViewModel>();
            var selectedPublisherValue = book.Publishers.ToString();
            var selectedCategoryValue = book.Categories.ToString();
            ViewBag.publisherId = GetPubsCats(selectedPublisherValue.Split(','), 1);
            ViewBag.categoryId = GetPubsCats(selectedCategoryValue.Split(','), 2);
            //foreach (var category in allCategory)
            //{
            //    bookCategoriesViewModel.Add(new BookCategoriesViewModel()
            //    {
            //        CategoryId = category.Id,
            //        CategoryName = category.CategoryName,
            //        IsSelected = book.Categories.Where(x => x.CategoryId == category.Id).Any()
            //    });
            //}
            //book.Categories = bookCategoriesViewModel;
            return View(book);
        }

        //
        // POST: /Books/Edit/5
        [HttpPost]
        public ActionResult Edit(Book book)
        {
            if (ModelState.IsValid)
            {
                // TODO: Add update logic here
                //DangVH. Create. Start (02/11/2016)
                var file = Request.Files[0];
                var uploadResult = ImageUploadHelper.GetUploadResult(file);
                //DangVH. Create. End (02/11/2016)
                //var selectedCategories = book.Categories.Where(x => x.IsSelected).Select(x => x.CategoryId).ToList();
                var editBook = Context.Books.Find(x => x.Id.Equals(new ObjectId(book.Id))).FirstOrDefault();
                editBook.BookName = book.BookName;
                editBook.Authors = book.Authors;
                editBook.ReleaseDay = book.ReleaseDay.ToLocalTime();
                editBook.Description = book.Description;
                editBook.ImgPublicId = uploadResult.PublicId;
                editBook.Categories.Clear();
                editBook.Publishers.Clear();
                //foreach (var categoryId in selectedCategories)
                //{
                //    editBook.Categories.Add(new BookCategoriesViewModel()
                //    {
                //        CategoryId = categoryId
                //    });
                //}
                foreach (var publisherId in book.Publishers)
                {
                    editBook.Publishers.Add(publisherId);
                }

                foreach (var categoryId in book.Categories)
                {
                    editBook.Categories.Add(categoryId);
                }
                Context.Books.ReplaceOneAsync(x => x.Id.Equals(new ObjectId(book.Id)), editBook);
                return RedirectToAction("Index");
            }
            ViewBag.bookNumber = BooksControllerHelper.GetBookNumber();
            return View();
        }

        //DangVH. Delete. Create (09/11/2016)
        //
        // GET: /Books/Delete/5
        //public ActionResult Delete(string id)
        //{
        //    var book = Context.Books.Find(x => x.Id.Equals(new ObjectId(id))).FirstOrDefault();
        //    var allCategory = Context.Categories.Find(_ => true).ToEnumerable();
        //    List<Category> listCategory = new List<Category>();
        //    foreach (var cat in book.Categories)
        //    {
        //        listCategory.Add(new Category()
        //        {
        //            CategoryName = allCategory.Where(x => x.Id.Equals(cat.CategoryId)).First().CategoryName
        //        });
        //    }
        //    ViewBag.ListCategory = listCategory;
        //    return View(book);
        //}
        //DangVH. Delete. End (09/11/2016)

        //
        // POST: /Books/Delete/5
        //DangVH. Update. Start (09/11/2016)
        //[HttpPost, ActionName("Delete")]
        [HttpPost]
        //DangVH. Update. End (09/11/2016)
        public ActionResult DeleteConfirm(string id)
        {
            Context.Books.DeleteOneAsync(x => x.Id.Equals(new ObjectId(id)));
            return Json("Xóa thành công");
        }
    }
}