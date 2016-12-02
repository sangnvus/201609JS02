using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using BSNCapstone.ViewModels;

namespace BSNCapstone.Models
{
    public class Book
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Required(ErrorMessage = "Tên sách bắt buộc")]
        //DangVH. Update. Start (14/11/2016)
        //[Display(Name = "Book Name")]
        [Display(Name = "Tên sách")]
        public string BookName { get; set; }
        //DangVH. Update. End (14/11/2016)

        [Required(ErrorMessage = "Tác giả bắt buộc")]
        //DangVH. Update. Start (14/11/2016)
        //[Display(Name = "Authors")]
        [Display(Name = "Tác giả")]
        public string Authors { get; set; }
        //DangVH. Update. End (14/11/2016)

        [Required(ErrorMessage = "Nhà xuất bản bắt buộc")]
        //DangVH. Update. Start (14/11/2016)
        //[Display(Name = "Publisher")]
        //public string Publishers { get; set; }
        [Display(Name = "Nhà xuất bản")]
        public List<string> Publishers { get; set; }
        //DangVH. Update. End (14/11/2016)

        [Required(ErrorMessage = "Ngày xuất bản bắt buộc")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:dd/MM/yyyy}")]
        //DangVH. Update. Start (14/11/2016)
        //[Display(Name = "Release Day")]
        [Display(Name = "Ngày xuất bản")]
        //DangVH. Update. End (14/11/2016)
        public DateTime ReleaseDay { get; set; }

        //DangVH. Update. Start (14/11/2016)
        //[Display(Name = "Description")]
        [Display(Name = "Miêu tả")]
        //DangVH. Update. End (14/11/2016)
        public string Description { get; set; }

        //DangVH. Create. Start (02/11/2016)
        //DangVH. Update. Start (14/11/2016)
        //[Display(Name = "Book Image")]
        [Display(Name = "Ảnh")]
        //DangVH. Update. End (14/11/2016)
        //DangVH. Create. End (02/11/2016)
        public string ImgPublicId { get; set; }

        //DangVH. Update. Start (14/11/2016)
        [Required]
        [Display(Name = "Thể loại")]
        //public List<BookCategoriesViewModel> Categories { get; set; }
        public List<string> Categories { get; set; }
        //DangVH. Update. End (14/11/2016)

        //DangVH. Create. Start (17/11/2016)
        public int RatingPoint { get; set; }

        public int RateTime { get; set; }
        //DangVH. Create. End (17/11/2016)

        public double AvarageRating { get; set; }

        public List<BookCommentViewModel> Comments { get; set; }

        public Book()
        {
            //DangVH. Update. Start (14/11/2016)
            //Categories = new List<BookCategoriesViewModel>();
            Comments = new List<BookCommentViewModel>();
            Publishers = new List<string>();
            Categories = new List<string>();
            RateTime = 0;
            RatingPoint = 0;
            AvarageRating = 0;
            //DangVH. Update. End (14/11/2016)
        }
    }
}