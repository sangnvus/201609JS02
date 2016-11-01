using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BSNCapstone.App_Start;
using BSNCapstone.Models;
using BSNCapstone.ControllerHelpers;
using BSNCapstone.ViewModels;
using MongoDB.Driver;
using MongoDB.Bson;

namespace BSNCapstone.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private readonly ApplicationIdentityContext Context = ApplicationIdentityContext.Create();
        //
        // GET: /Books/
        public ActionResult Index(string searchString)
        {
            ViewBag.allCategories = BooksControllerHelper.ListAllCategory();
            var books = Context.Books.Find(_ => true).ToEnumerable();
            return View(books);
        }

        //
        // GET: /Books/Details/5
        public ActionResult Details(string id)
        {
            var book = Context.Books.Find(x => x.Id.Equals(new ObjectId(id))).FirstOrDefault();
            var allCategory = Context.Categories.Find(_ => true).ToEnumerable();
            List<Category> listCategory = new List<Category>();
            foreach (var cat in book.Categories)
            {
                listCategory.Add(new Category()
                {
                    CategoryName = allCategory.Where(x => x.Id.Equals(cat.CategoryId)).First().CategoryName
                });
            }
            ViewBag.ListCategory = listCategory;
            return View(book);
        }

        //
        // GET: /Books/Create
        public ActionResult Create()
        {
            Book book = BooksControllerHelper.GetCheckBoxValues();
            return View(book);
        }

        //
        // POST: /Books/Create
        [HttpPost]
        public ActionResult Create(Book book)
        {
            if (ModelState.IsValid)
            {
                // TODO: Add insert logic here
                var selectedCategories = book.Categories.Where(x => x.IsSelected).Select(x => x.CategoryId).ToList();
                //DangVH. Create. Start (01/11/2016)
                var file = Request.Files[0];
                var uploadResult = ImageUploadHelper.GetUploadResult(file);
                //DangVH. Create. End (01/11/2016)
                var addBook = new Book()
                {
                    BookName = book.BookName,
                    Authors = book.Authors,
                    Publishers = book.Publishers,
                    ReleaseDay = book.ReleaseDay.ToLocalTime(),
                    Description = book.Description,
                    ImgPublicId = uploadResult.PublicId
                };

                foreach (var categoryId in selectedCategories)
                {
                    addBook.Categories.Add(new BookCategoriesViewModel()
                    {
                        CategoryId = categoryId
                    });
                }
                Context.Books.InsertOneAsync(addBook);
                return RedirectToAction("Index");
            }
            Book newbook = BooksControllerHelper.GetCheckBoxValues();
            return View(newbook);
        }

        //
        // GET: /Books/Edit/5
        public ActionResult Edit(string id)
        {
            var book = Context.Books.Find(x => x.Id.Equals(new ObjectId(id))).FirstOrDefault();
            var allCategory = Context.Categories.Find(_ => true).ToEnumerable();
            var bookCategoriesViewModel = new List<BookCategoriesViewModel>();
            foreach (var category in allCategory)
            {
                bookCategoriesViewModel.Add(new BookCategoriesViewModel()
                {
                    CategoryId = category.Id,
                    CategoryName = category.CategoryName,
                    IsSelected = book.Categories.Where(x => x.CategoryId == category.Id).Any()
                });
            }
            book.Categories = bookCategoriesViewModel;
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
                var selectedCategories = book.Categories.Where(x => x.IsSelected).Select(x => x.CategoryId).ToList();
                var editBook = Context.Books.Find(x => x.Id.Equals(new ObjectId(book.Id))).FirstOrDefault();
                editBook.BookName = book.BookName;
                editBook.Authors = book.Authors;
                editBook.Publishers = book.Publishers;
                editBook.ReleaseDay = book.ReleaseDay.ToLocalTime();
                editBook.Description = book.Description;
                editBook.Categories.Clear();
                foreach (var categoryId in selectedCategories)
                {
                    editBook.Categories.Add(new BookCategoriesViewModel()
                    {
                        CategoryId = categoryId
                    });
                }
                Context.Books.ReplaceOneAsync(x => x.Id.Equals(new ObjectId(book.Id)), editBook);
                return RedirectToAction("Index");
            }
            return View();
        }

        //
        // GET: /Books/Delete/5
        public ActionResult Delete(string id)
        {
            var book = Context.Books.Find(x => x.Id.Equals(new ObjectId(id))).FirstOrDefault();
            var allCategory = Context.Categories.Find(_ => true).ToEnumerable();
            List<Category> listCategory = new List<Category>();
            foreach (var cat in book.Categories)
            {
                listCategory.Add(new Category()
                {
                    CategoryName = allCategory.Where(x => x.Id.Equals(cat.CategoryId)).First().CategoryName
                });
            }
            ViewBag.ListCategory = listCategory;
            return View(book);
        }

        //
        // POST: /Books/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(string id)
        {
            Context.Books.DeleteOneAsync(x => x.Id.Equals(new ObjectId(id)));
            return RedirectToAction("Index");
        }
    }
}