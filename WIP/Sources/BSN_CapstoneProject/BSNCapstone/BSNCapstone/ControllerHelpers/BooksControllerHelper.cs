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
        private static ApplicationIdentityContext Context()
        {
            ApplicationIdentityContext context = ApplicationIdentityContext.Create();
            return context;
        }

        public static int GetBookNumber()
        {
            return (int)BooksControllerHelper.Context().Books.Find(_ => true).Count();
        }
        //DangVH. Create. End (02/11/2016)

        public static List<Category> ListAllCategory()
        {
            ApplicationIdentityContext Context = ApplicationIdentityContext.Create();
            List<Category> categories = Context.Categories.Find(_ => true).ToList();
            return categories;
        }

        public static double GetAverageRatingPoint(int ratingPoint, int ratingTime)
        {
            double result = 0;
            if (ratingTime == 0)
            {
                result = ratingPoint;
            }
            else
            {
                //result = (ratingPoint / ratingTime);
                double x = 0;
                //x = Math.Round((double)(ratingPoint / ratingTime), 1);
                x = (double)ratingPoint / ratingTime;
                result = Math.Round(x, 1);
            }
            return result;
        }

        public static List<BooksSuggestViewModel> SuggestBook(string bookId, int option)
        {
            var Context = BooksControllerHelper.Context();
            List<Book> listBook = Context.Books.Find(_ => true).ToList();
            List<BooksSuggestViewModel> suggestBooks = new List<BooksSuggestViewModel>();
            switch (option)
            {
                case 1:
                    DateTime startTime = DateTime.Now.AddDays(-1);
                    DateTime endTime = DateTime.Now;
                    List<BooksSuggestViewModel> newList = new List<BooksSuggestViewModel>();
                    var b = Context.BooksStatistic.Find(x => x.EachDate.Equals(startTime.Date.AddHours(7))).ToList();
                    var c = Context.BooksStatistic.Find(x => x.EachDate.Equals(endTime.Date.AddHours(7))).ToList();
                    if (b.Count() == 0)
                    {
                        foreach (var c1 in c)
                        {
                            newList.Add(new BooksSuggestViewModel()
                            {
                                BookId = c1.BookId,
                                Count = c1.Count,
                                BookName = listBook.Find(x => x.Id.Equals(c1.BookId)).BookName,
                                AuthorName = listBook.Find(x => x.Id.Equals(c1.BookId)).Authors,
                                ImgPublicId = listBook.Find(x => x.Id.Equals(c1.BookId)).ImgPublicId,
                                RateTime = listBook.Find(x => x.Id.Equals(c1.BookId)).RateTime
                            });
                        }
                    }
                    else
                    {
                        foreach (var b1 in b)
                        {
                            foreach (var c1 in c)
                            {
                                if (b1.BookId == c1.BookId)
                                {
                                    newList.Add(new BooksSuggestViewModel()
                                    {
                                        BookId = b1.BookId,
                                        Count = b1.Count + c1.Count,
                                        BookName = listBook.Find(x => x.Id.Equals(c1.BookId)).BookName,
                                        AuthorName = listBook.Find(x => x.Id.Equals(c1.BookId)).Authors,
                                        ImgPublicId = listBook.Find(x => x.Id.Equals(c1.BookId)).ImgPublicId,
                                        RateTime = listBook.Find(x => x.Id.Equals(c1.BookId)).RateTime
                                    });
                                }
                                else
                                {
                                    newList.Add(new BooksSuggestViewModel()
                                    {
                                        BookId = b1.BookId,
                                        Count = b1.Count,
                                        BookName = listBook.Find(x => x.Id.Equals(b1.BookId)).BookName,
                                        AuthorName = listBook.Find(x => x.Id.Equals(b1.BookId)).Authors,
                                        ImgPublicId = listBook.Find(x => x.Id.Equals(b1.BookId)).ImgPublicId,
                                        RateTime = listBook.Find(x => x.Id.Equals(b1.BookId)).RateTime
                                    });
                                }
                            }
                        }
                    }
                    suggestBooks = newList.OrderByDescending(x => x.Count).ToList();
                    suggestBooks = suggestBooks.GroupBy(x => x.BookId).Select(y => y.First()).ToList().Take(4).ToList();
                    Console.Write(suggestBooks);
                    break;
                case 2:
                    listBook = listBook.OrderByDescending(x => x.AvarageRating).Take(4).ToList();
                    foreach (var book in listBook)
                    {
                        suggestBooks.Add(new BooksSuggestViewModel() 
                        { 
                            BookId = book.Id,
                            BookName = book.BookName,
                            AuthorName = book.Authors,
                            ImgPublicId = book.ImgPublicId,
                            RateTime = book.RateTime
                        });
                    }
                    break;
                case 3:
                    listBook = listBook.OrderByDescending(x => x.ReleaseDay).Take(4).ToList();
                    foreach (var book in listBook)
                    {
                        suggestBooks.Add(new BooksSuggestViewModel() 
                        {
                            BookId = book.Id,
                            BookName = book.BookName,
                            AuthorName = book.Authors,
                            ImgPublicId = book.ImgPublicId,
                            RateTime = book.RateTime
                        });
                    }
                    break;
                case 4:
                    var modelBook = Context.Books.Find(x => x.Id.Equals(new ObjectId(bookId))).FirstOrDefault();
                    foreach (var book in listBook)
                    {
                        int count = 0;
                        foreach (var category in modelBook.Categories)
                        {
                            if (book.Categories.Contains(category) && !book.Id.Equals(new ObjectId(bookId)))
                            {
                                count = count + 1;
                            }                       
                        }
                        if (book.Id != bookId) 
                        {
                            suggestBooks.Add(new BooksSuggestViewModel() 
                            {
                                BookId = book.Id,
                                BookName = book.BookName,
                                AuthorName = book.Authors,
                                ImgPublicId = book.ImgPublicId,
                                RateTime = book.RateTime,
                                Count = count
                            });
                        }
                    }
                    Console.Write(suggestBooks);
                    suggestBooks = suggestBooks.OrderByDescending(x => x.Count).Take(4).ToList();
                    break;
            }
            //return suggestBooks;
            return suggestBooks;
        }

        class BookCompare : IEqualityComparer<Book>
        {
            public bool Equals(Book x, Book y)
            {
                if (x.Id == y.Id) return true;
                return false; 
            }

            public int GetHashCode(Book book)
            {
                return book.Id.GetHashCode();
            }
        }
    }
}