﻿using System.Collections.Generic;
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
using PagedList;

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
                var authors = Context.Authors.Find(_ => true).ToEnumerable();
                return new MultiSelectList(authors, "Id", "AuthorName", selectedValues);
            }
        }
        //DangVH. Create. End (14/11/2016)


        //
        // GET: /Books/
        public ActionResult Index(string searchString, string currentFilter, int? page)
        {
            if (Context.Users.Find(x => x.Id.Equals(User.Identity.GetUserId())).FirstOrDefault().Roles.Contains("admin"))
            {
                //DangVH. Create. Start (02/11/2016)
                ViewBag.currentUser = Context.Users.Find(x => x.Id.Equals(new ObjectId(User.Identity.GetUserId()))).FirstOrDefault();
                ViewBag.bookNumber = BooksControllerHelper.GetBookNumber();
                //DangVH. Create. End (02/11/2016)
                ViewBag.allCategories = BooksControllerHelper.ListAllCategory();
                ViewBag.allPublishers = Context.Publishers.Find(_ => true).ToList();
                ViewBag.allAuthors = Context.Authors.Find(_ => true).ToList();
                var books = Context.Books.Find(x => x.Requested.Equals(false)).ToEnumerable();
                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }
                ViewBag.currentFilter = searchString;
                //DangVH. Create. Start (02/11/2016)
                if (!string.IsNullOrEmpty(searchString))
                {
                    var authors = Context.Authors.Find(_ => true).ToList().Where(x => x.AuthorName.Contains(searchString)).ToList();
                    List<string> searchAuthors = new List<string>();
                    foreach (var author in authors)
                    {
                        searchAuthors.Add(author.Id.ToString());
                    }
                    books = books.Where(x => x.BookName.Contains(searchString) || x.Authors.Intersect(searchAuthors).Any());
                }
                int pageSize = 6;
                int pageNumber = (page ?? 1);
                //DangVH. Create. End (02/11/2016)
                return View(books.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                ViewBag.errorMessage = "Bạn không có quyền truy cập vào chức năng này";
                return View("NotFoundError");
            }
        }

        //
        // GET: /Books/Details/5
        public ActionResult Details(string id)
        {
            //DangVH. Create. Start (02/11/2016)
            ViewBag.cloudinary = cloudinary;
            //DangVH. Create. End (02/11/2016)
            var allBook = Context.Books.Find(_ => true).ToList();
            var check = allBook.Where(x => x.Id.Equals(id)).Any();
            if (check == false)
            {
                ViewBag.errorMessage = "Không có kết quả!";
                return View("NotFoundError");
            }
            else
            {
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
                // Đoạn này để tính số lượt truy cập của sách
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
                // Hết đoạn tính số lượt truy cập sách
                // Xử lý sách đã xem và đánh giá gần đây của người dùng hiện tại
                var userInteractFilter = Builders<ApplicationUser>.Filter.Where(x => x.Id.Equals(User.Identity.GetUserId()));
                var userInteractUpdate = Builders<ApplicationUser>.Update.Push(x => x.Interacbook, new InteractBookViewModel
                {
                    Id = ObjectId.GenerateNewId(),
                    BookId = book.Id,
                    InteractTime = DateTime.Now.AddHours(7)
                });
                Context.Users.UpdateOneAsync(userInteractFilter, userInteractUpdate);
                // Hết phần xử lí sách đã xem và đánh giá gần đây của người dùng hiện tại
                ViewBag.listSameBook = BooksControllerHelper.SuggestBook(id, 4); // Lấy list sách có chung thể loại
                Random random = new Random((int)(DateTime.Now.Ticks));
                ViewBag.randomGroup = Context.Groups.Find(_ => true).ToList().OrderBy(x => random.Next()).Take(4).ToList(); // List nhóm ngẫu nhiên
                ViewBag.listGroupHaveTagIsCurrentBook = GroupsControllerHelper.SuggestGroup(id); // List nhóm có thẻ là sách hiện tại
                ViewBag.allAuthor = Context.Authors.Find(_ => true).ToList();
                ViewBag.allUser = Context.Users.Find(_ => true).ToList();
                return View(book);
            }
        }

        //
        // GET: /Books/Create
        [AllowAnonymous]
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
            if (Context.Users.Find(x => x.Id.Equals(User.Identity.GetUserId())).FirstOrDefault().Roles.Contains("admin"))
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
                        Requested = false,
                        Text = CommonHelper.SearchString(book.BookName.ToLower())
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
            else
            {
                ViewBag.errorMessage = "Bạn không có quyền truy cập vào chức năng này";
                return View("NotFoundError");
            }
        }

        //
        // GET: /Books/Edit/5
        public ActionResult Edit(string id)
        {
            if (Context.Users.Find(x => x.Id.Equals(User.Identity.GetUserId())).FirstOrDefault().Roles.Contains("admin"))
            {
                //DangVH. Create. Start (02/11/2016)
                ViewBag.cloudinary = cloudinary;
                //DangVH. Create. End (02/11/2016)
                var allBook = Context.Books.Find(_ => true).ToList();
                if (allBook.Where(x => x.Id.Equals(id)).Any() == false)
                {
                    ViewBag.errorMessage = "Không có kết quả!";
                    return View("NotFoundError");
                }
                else
                {
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
            }
            else
            {
                ViewBag.errorMessage = "Bạn không có quyền truy cập vào chức năng này";
                return View("NotFoundError");
            }
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
                    editBook.Requested = false;
                    editBook.Text = CommonHelper.SearchString(book.BookName.ToLower());
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

        //GET: /Books/BookRequest
        public ActionResult BookRequest(string searchString)
        {
            if (Context.Users.Find(x => x.Id.Equals(User.Identity.GetUserId())).FirstOrDefault().Roles.Contains("admin"))
            {
                var booksRequested = Context.Books.Find(x => x.Requested.Equals(true)).ToEnumerable();
                ViewBag.numberOfBook = BooksControllerHelper.GetBookNumber();
                ViewBag.allCategories = Context.Categories.Find(_ => true).ToList();
                if (!string.IsNullOrEmpty(searchString))
                {
                    booksRequested = booksRequested.Where(x => x.BookName.Contains(searchString) || x.RequestedBookAuthor.Contains(searchString));
                }
                return View(booksRequested);
            }
            else
            {
                ViewBag.errorMessage = "Bạn không có quyền truy cập vào chức năng này";
                return View("NotFoundError");
            }
        }

        [HttpPost]
        public ActionResult RequestAddBook(string bookName, string author, string categories)
        {
            try
            {
                var books = Context.Books.Find(_ => true).ToList();
                List<string> errorMessage = new List<string>();
                if (bookName == "" || bookName == null)
                {
                    var message = "Tên sách không được để trống";
                    errorMessage.Add(message);
                }
                if (author == "" || author == null)
                {
                    var message = "Tác giả không được để trống";
                    errorMessage.Add(message);
                }
                if (categories == "")
                {
                    var message = "Thể loại không được để trống";
                    errorMessage.Add(message);
                }
                foreach (var book in books) 
                {
                    if (book.BookName.ToLower().Equals(bookName.ToLower())) 
                    {
                        var message = "Sách đã tồn tại";
                        errorMessage.Add(message);
                    }
                }
                if (errorMessage.Count == 0)
                {
                    var requestBook = new Book() 
                    {
                        BookName = bookName,
                        RequestedBookAuthor = author,
                        Requested = true
                    };
                    //var authors = Context.Users.Find(x => x.Roles.Contains("author")).ToList();
                    //if (authors.Find(x => x.UserName.ToLower().Contains(author.ToLower())) == null)
                    //{
                    //    requestBook.Authors.Add((authors.Find(x => x.UserName.ToLower().Contains(author.ToLower()))).Id);
                    //}
                    var requestCategories = categories.Split(',').ToList();
                    foreach (var category in requestCategories)
                    {
                        requestBook.Categories.Add(category);
                    }
                    Context.Books.InsertOneAsync(requestBook);
                    return Json(new
                    {
                        successed = "Yêu cầu thành công"
                    });
                }
                else
                {
                    return Json(errorMessage, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
    }
}