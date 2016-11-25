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

        //DangVH. Create. Start (14/11/2016)
        //Lấy giá trị cho multiselectlist 
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
        //DangVH. Create. End (14/11/2016)


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
            ViewBag.allCategories = BooksControllerHelper.ListAllCategory();
            ViewBag.allPublishers = Context.Publishers.Find(_ => true).ToList();
            ViewBag.avarageRatingPoint = BooksControllerHelper.GetAverageRatingPoint(book.RatingPoint, book.RateTime);
            //DangVH. Delete. Start (14/11/2016)
            //List<Category> listCategory = new List<Category>();
            //foreach (var cat in book.Categories)
            //{
            //    listCategory.Add(new Category()
            //    {
            //        CategoryName = allCategory.Where(x => x.Id.Equals(cat.CategoryId)).First().CategoryName
            //    });
            //}
            //ViewBag.ListCategory = listCategory;
            //DangVH. Delete. End (14/11/2016)
            return View(book);
        }

        //
        // GET: /Books/Create
        public ActionResult Create()
        {
            //DangVH. Delete. Start (14/11/2016)
            //Book book = BooksControllerHelper.GetCheckBoxValues();
            //return PartialView("_Create", book);
            //DangVH. Delete. End (14/11/2016)
            //DangVH. Create. Start (14/11/2016)
            ViewBag.bookNumber = BooksControllerHelper.GetBookNumber();
            ViewBag.CategoryId = GetPubsCats(null, 2);
            ViewBag.PublisherId = GetPubsCats(null, 1);
            return View();
            //DangVH. Create. End (14/11/2016)
        }

        //
        // POST: /Books/Create
        [HttpPost]
        public ActionResult Create(Book book)
        {
            var listBook = Context.Books.Find(_ => true).ToList();
            var file = Request.Files[0];
            if (file.ContentLength == 0)
            {
                ModelState.AddModelError("ImgPublicId", "Ảnh sách bắt buộc");
            }
            foreach (var eachBook in listBook)
            {
                if (eachBook.BookName.ToLower().Equals(book.BookName.ToLower()))
                {
                    ModelState.AddModelError("BookName", "Tên sách đã tồn tại");
                }
            }
            if (ModelState.IsValid)
            {
                // TODO: Add insert logic here
                //DangVH. Delete. Start (14/11/2016)
                //var selectedCategories = book.Categories.Where(x => x.IsSelected).Select(x => x.CategoryId).ToList();
                //DangVH. Delete. End (14/11/2016)
                //DangVH. Create. Start (01/11/2016)
                //var file = Request.Files[0];
                var uploadResult = ImageUploadHelper.GetUploadResult(file);
                //DangVH. Create. End (01/11/2016)
                var addBook = new Book()
                {
                    BookName = book.BookName,
                    Authors = book.Authors,
                    //DangVH. Delete. Start (14/11/2016)
                    //Publishers = book.Publishers,
                    //DangVH. Delete. End (14/11/2016)
                    ReleaseDay = book.ReleaseDay.ToLocalTime(),
                    Description = book.Description,
                    ImgPublicId = uploadResult.PublicId
                };
                //DangVH. Delete. Start (14/11/2016)
                //foreach (var categoryId in selectedCategories)
                //{
                //    addBook.Categories.Add(new BookCategoriesViewModel()
                //    {
                //        CategoryId = categoryId
                //    });
                //}
                //DangVH. Delete. End (14/11/2016)
                //DangVh. Create. Start (14/11/2016)
                foreach (var publisherId in book.Publishers)
                {
                    addBook.Publishers.Add(publisherId);
                }

                foreach (var categoryId in book.Categories)
                {
                    addBook.Categories.Add(categoryId);
                }
                //DangVH. Create. End (14/11/2016)
                Context.Books.InsertOneAsync(addBook);
                return RedirectToAction("Index");
            }
            //DangVH. Update. Start (14/11/2016)
            //Book newbook = BooksControllerHelper.GetCheckBoxValues();
            ViewBag.bookNumber = BooksControllerHelper.GetBookNumber();
            var selectedPublisherValue = book.Publishers.ToString();
            var selectedCategoryValue = book.Categories.ToString();
            ViewBag.publisherId = GetPubsCats(selectedPublisherValue.Split(','), 1);
            ViewBag.categoryId = GetPubsCats(selectedCategoryValue.Split(','), 2);
            //DangVH. Update. End (14/11/2016)
            return View(book);
        }

        //
        // GET: /Books/Edit/5
        public ActionResult Edit(string id)
        {
            //DangVH. Create. Start (02/11/2016)
            ViewBag.cloudinary = cloudinary;
            //DangVH. Create. End (02/11/2016)
            var book = Context.Books.Find(x => x.Id.Equals(new ObjectId(id))).FirstOrDefault();
            //DangVH. Delete. Start (14/11/2016)
            //var allCategory = Context.Categories.Find(_ => true).ToEnumerable();
            //var bookCategoriesViewModel = new List<BookCategoriesViewModel>();
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
            //DangVH. Delete. End (14/11/2016)
            //DangVH. Create. Start (14/11/2016)
            var selectedPublisherValue = book.Publishers.ToString();
            var selectedCategoryValue = book.Categories.ToString();
            ViewBag.publisherId = GetPubsCats(selectedPublisherValue.Split(','), 1);
            ViewBag.categoryId = GetPubsCats(selectedCategoryValue.Split(','), 2);
            ViewBag.bookNumber = BooksControllerHelper.GetBookNumber();
            //DangVH. Create. End (14/11/2016)
            return View(book);
        }

        //
        // POST: /Books/Edit/5
        [HttpPost]
        public ActionResult Edit(Book book)
        {
            var listBook = Context.Books.Find(_ => true).ToList();
            foreach (var eachBook in listBook)
            {
                if (eachBook.BookName.ToLower().Equals(book.BookName.ToLower()))
                {
                    ModelState.AddModelError("BookName", "Tên sách đã tồn tại");
                }
            }
            if (ModelState.IsValid)
            {
                // TODO: Add update logic here
                //DangVH. Create. Start (02/11/2016)
                {
                    var file = Request.Files[0];
                    var uploadResult = ImageUploadHelper.GetUploadResult(file);
                    //DangVH. Create. End (02/11/2016)
                    var editBook = Context.Books.Find(x => x.Id.Equals(new ObjectId(book.Id))).FirstOrDefault();
                    editBook.BookName = book.BookName;
                    editBook.Authors = book.Authors;
                    editBook.ReleaseDay = book.ReleaseDay.ToLocalTime();
                    editBook.Description = book.Description;
                    if (file.ContentLength == 0)
                    {
                        editBook.ImgPublicId = book.ImgPublicId;
                    }
                    else
                    {
                        editBook.ImgPublicId = uploadResult.PublicId;
                    }
                    editBook.Categories.Clear();
                    //DangVH. Create. Start (14/11/2016)
                    editBook.Publishers.Clear();
                    foreach (var publisherId in book.Publishers)
                    {
                        editBook.Publishers.Add(publisherId);
                    }

                    foreach (var categoryId in book.Categories)
                    {
                        editBook.Categories.Add(categoryId);
                    }
                    //DangVH. Create. End (14/11/2016)
                    Context.Books.ReplaceOneAsync(x => x.Id.Equals(new ObjectId(book.Id)), editBook);
                    return RedirectToAction("Index");
                }
            }
            ViewBag.bookNumber = BooksControllerHelper.GetBookNumber(); 
            var selectedPublisherValue = book.Publishers.ToString();
            var selectedCategoryValue = book.Categories.ToString();
            ViewBag.publisherId = GetPubsCats(selectedPublisherValue.Split(','), 1);
            ViewBag.categoryId = GetPubsCats(selectedCategoryValue.Split(','), 2);
            ViewBag.cloudinary = cloudinary;
            return View(book);
        }

        //DangVH. Delete. Start (14/11/2016)
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
        //DangVH. Delete. End (14/11/2016)

        //
        // POST: /Books/Delete/5
        [HttpPost]
        public ActionResult DeleteConfirm(string id)
        {
            Context.Books.DeleteOneAsync(x => x.Id.Equals(new ObjectId(id)));
            //DangVH. Update. Start (14/11/2016)
            //return RedirectToAction("Index");
            return Json("Xóa thành công");
            //DangVH. Update. End (14/11/2016)
        }

        //DangVH. Create. Start (17/11/2016)
        //Rating book
        [HttpPost]
        public ActionResult RatingBook(string id, int rating)
        {
            var book = Context.Books.Find(x => x.Id.Equals(new ObjectId(id))).FirstOrDefault();
            int ratingPoint = book.RatingPoint + rating;
            int ratingTime = book.RateTime + 1;
            var filter = Builders<Book>.Filter.Where(x => x.Id.Equals(book.Id));
            var update = Builders<Book>.Update.Set(x => x.RatingPoint, ratingPoint).
                Set(x => x.RateTime, ratingTime).
                Set(x => x.AvarageRating, BooksControllerHelper.GetAverageRatingPoint(ratingPoint, ratingTime));
            Context.Books.UpdateOneAsync(filter, update);
            return Json("Cảm ơn bạn đã đánh giá :D");
        }
        //DangVH. Create. End (17/11/2016)
    }
}