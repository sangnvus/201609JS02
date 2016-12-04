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
using System;

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
        private MultiSelectList GetPubsCatsAus(string[] selectedValues, int option)
        {
            if (option == 1)
            {
                var publishers = Context.Publishers.Find(_ => true).ToEnumerable();
                return new MultiSelectList(publishers, "Id", "Name", selectedValues);
            }
            else if (option == 2)
            {
                var categories = Context.Categories.Find(_ => true).ToEnumerable();
                return new MultiSelectList(categories, "Id", "CategoryName", selectedValues);
            }
            else
            {
                var authors = Context.Users.Find(x => x.Roles.Contains("author")).ToEnumerable();
                return new MultiSelectList(authors, "Id", "UserName", selectedValues);
            }
        }
        //DangVH. Create. End (14/11/2016)


        //
        // GET: /Books/
        public ActionResult Index(string searchString)
        {
            //DangVH. Create. Start (02/11/2016)
            ViewBag.currentUser = Context.Users.Find(x => x.Id.Equals(new ObjectId(User.Identity.GetUserId()))).FirstOrDefault();
            ViewBag.bookNumber = BooksControllerHelper.GetBookNumber();
            //DangVH. Create. End (02/11/2016)
            ViewBag.allCategories = BooksControllerHelper.ListAllCategory();
            ViewBag.allPublishers = Context.Publishers.Find(_ => true).ToList();
            ViewBag.allAuthors = Context.Users.Find(x => x.Roles.Contains("author")).ToList();
            var books = Context.Books.Find(_ => true).ToEnumerable();
            //DangVH. Create. Start (02/11/2016)
            if (!string.IsNullOrEmpty(searchString))
            {
                var authors = Context.Users.Find(x => x.Roles.Contains("author")).ToList().Where(x => x.UserName.Contains(searchString)).ToList();
                List<string> searchAuthors = new List<string>();
                foreach (var author in authors)
                {
                    searchAuthors.Add(author.Id);
                }
                books = books.Where(x => x.BookName.Contains(searchString) || x.Authors.Intersect(searchAuthors).Any());
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
            foreach (var comment in book.Comments)
            {
                comment.LinesDescription = comment.Description.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
            }
            book.Comments = book.Comments.OrderByDescending(x => x.CreatedTime).ToList();
            Console.Write(book);
            var allCategory = Context.Categories.Find(_ => true).ToEnumerable();
            ViewBag.allCategories = BooksControllerHelper.ListAllCategory();
            ViewBag.allPublishers = Context.Publishers.Find(_ => true).ToList();
            ViewBag.avarageRatingPoint = BooksControllerHelper.GetAverageRatingPoint(book.RatingPoint, book.RateTime);
            var booksStatistic = Context.BooksStatistic.Find(_ => true).ToList();
            var date = DateTime.Now;
            var builder = Builders<BookStatistic>.Filter;
            var filter = builder.Eq("BookId", book.Id) & builder.Eq("EachDate", date.Date.AddHours(7));
            if (Context.BooksStatistic.Find(filter).FirstOrDefault() != null)
            {
                int count = Context.BooksStatistic.Find(filter).FirstOrDefault().Count + 1;
                var update = Builders<BookStatistic>.Update.Set(x => x.Count, count);
                Context.BooksStatistic.UpdateOneAsync(filter, update);
            }
            else
            {
                var bookStatistic = new BookStatistic()
                {
                    BookId = book.Id,
                    EachDate = date.Date.AddHours(7)
                };
                Context.BooksStatistic.InsertOneAsync(bookStatistic);
            }
            ViewBag.listSameBook = BooksControllerHelper.SuggestBook(id, 4);
            Random random = new Random((int)(DateTime.Now.Ticks));
            ViewBag.randomGroup = Context.Groups.Find(_ => true).ToList().OrderBy(x => random.Next()).Take(4).ToList();
            ViewBag.currentUser = Context.Users.Find(x => x.Id.Equals(new ObjectId(User.Identity.GetUserId()))).FirstOrDefault();
            ViewBag.allUser = Context.Users.Find(_ => true).ToList();
            return View(book);
        }

        //
        // GET: /Books/Create
        public ActionResult Create()
        {
            ViewBag.bookNumber = BooksControllerHelper.GetBookNumber();
            ViewBag.CategoryId = GetPubsCatsAus(null, 2);
            ViewBag.PublisherId = GetPubsCatsAus(null, 1);
            ViewBag.AuthorId = GetPubsCatsAus(null, 3);
            return View();
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
                if (book.BookName != null && eachBook.BookName.ToLower().Equals(book.BookName.ToLower()))
                {
                    ModelState.AddModelError("BookName", "Tên sách đã tồn tại");
                }
            } 
            if (book.Authors.Count == 0)
            {
                ModelState.AddModelError("Authors", "Tác giả bắt buộc");
            }
            if (book.Categories.Count == 0)
            {
                ModelState.AddModelError("Categories", "Thể loại sách bắt buộc");
            }
            if (book.Publishers.Count == 0)
            {
                ModelState.AddModelError("Publishers", "Nhà xuất bản bắt buộc");
            }
            if (ModelState.IsValid)
            {
                var uploadResult = ImageUploadHelper.GetUploadResult(file);
                var addBook = new Book()
                {
                    BookName = book.BookName,
                    ReleaseDay = book.ReleaseDay.ToLocalTime(),
                    Description = book.Description,
                    ImgPublicId = uploadResult.PublicId,
                    Requested = false
                };
                foreach (var publisherId in book.Publishers)
                {
                    addBook.Publishers.Add(publisherId);
                }
                foreach (var categoryId in book.Categories)
                {
                    addBook.Categories.Add(categoryId);
                }
                foreach (var authorId in book.Authors)
                {
                    addBook.Authors.Add(authorId);
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
            var selectedAuthorValue = book.Authors.ToString();
            ViewBag.publisherId = GetPubsCatsAus(selectedPublisherValue.Split(','), 1);
            ViewBag.categoryId = GetPubsCatsAus(selectedCategoryValue.Split(','), 2);
            ViewBag.authorId = GetPubsCatsAus(selectedAuthorValue.Split(','), 3);
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
            var selectedPublisherValue = book.Publishers.ToString();
            var selectedCategoryValue = book.Categories.ToString();
            var selectedAuthorValue = book.Authors.ToString();
            ViewBag.publisherId = GetPubsCatsAus(selectedPublisherValue.Split(','), 1);
            ViewBag.categoryId = GetPubsCatsAus(selectedCategoryValue.Split(','), 2);
            ViewBag.authorId = GetPubsCatsAus(selectedAuthorValue.Split(','), 3);
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
            if (book.BookName != null)
            {
                foreach (var eachBook in listBook)
                {
                    if (eachBook.BookName.ToLower().Equals(book.BookName.ToLower()) &&
                        listBook.Find(x => x.BookName.ToLower().Equals(book.BookName.ToLower())).Id != book.Id)
                    {
                        ModelState.AddModelError("BookName", "Tên sách đã tồn tại");
                    }
                }
            }
            if (book.Authors.Count == 0)
            {
                ModelState.AddModelError("Authors", "Tác giả bắt buộc");
            }
            if (book.Categories.Count == 0)
            {
                ModelState.AddModelError("Categories", "Thể loại sách bắt buộc");
            }
            if (book.Publishers.Count == 0)
            {
                ModelState.AddModelError("Publishers", "Nhà xuất bản bắt buộc");
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
                    editBook.Authors.Clear();
                    editBook.Categories.Clear();
                    editBook.Publishers.Clear();
                    foreach (var publisherId in book.Publishers)
                    {
                        editBook.Publishers.Add(publisherId);
                    }
                    foreach (var categoryId in book.Categories)
                    {
                        editBook.Categories.Add(categoryId);
                    }
                    foreach (var authorId in book.Authors) 
                    {
                        editBook.Authors.Add(authorId);
                    }
                    //DangVH. Create. End (14/11/2016)
                    Context.Books.ReplaceOneAsync(x => x.Id.Equals(new ObjectId(book.Id)), editBook);
                    return RedirectToAction("Index");
                }
            }
            ViewBag.bookNumber = BooksControllerHelper.GetBookNumber(); 
            var selectedPublisherValue = book.Publishers.ToString();
            var selectedCategoryValue = book.Categories.ToString();
            var selectedAuthorValue = book.Authors.ToString();
            ViewBag.publisherId = GetPubsCatsAus(selectedPublisherValue.Split(','), 1);
            ViewBag.categoryId = GetPubsCatsAus(selectedCategoryValue.Split(','), 2);
            ViewBag.AuthorId = GetPubsCatsAus(selectedCategoryValue.Split(','), 3);
            ViewBag.cloudinary = cloudinary;
            return View(book);
        }

        //
        // POST: /Books/Delete/5
        [HttpPost]
        public ActionResult DeleteConfirm(string id)
        {
            Context.Books.DeleteOneAsync(x => x.Id.Equals(new ObjectId(id)));
            Context.BooksStatistic.DeleteManyAsync(x => x.BookId.Equals(id));
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

        [HttpPost]
        public ActionResult CommentBook(string commentedUser, string bookId, string commentContent)
        {
            BookCommentViewModel comment = new BookCommentViewModel() 
            {
                Id = ObjectId.GenerateNewId(),
                UserId = commentedUser,
                CreatedTime = DateTime.Now.AddHours(7),
                Description = commentContent
            };
            var filter = Builders<Book>.Filter.Where(x => x.Id.Equals(new ObjectId(bookId)));
            var update = Builders<Book>.Update.Push(x => x.Comments, comment);
            Context.Books.UpdateOneAsync(filter, update);
            return Json("Bình luận thành công");
        }
    }
}