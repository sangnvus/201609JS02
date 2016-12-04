using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSNCapstone.ViewModels
{
    public class BooksSuggestViewModel
    {
        public string BookId { get; set; }
        public int Count { get; set; }
        public string BookName { get; set; }
        public List<string> AuthorName { get; set; }
        public string ImgPublicId { get; set; }
        public int RateTime { get; set; }

        public BooksSuggestViewModel()
        {
            AuthorName = new List<string>();
        }
    }
}