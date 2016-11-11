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
        [Display(Name = "Tên sách")]
        public string BookName { get; set; }

        [Required(ErrorMessage = "Tác giả bắt buộc")]
        [Display(Name = "Tác giả")]
        public string Authors { get; set; }

        [Required(ErrorMessage = "Nhà xuất bản bắt buộc")]
        [Display(Name = "Nhà xuất bản")]
        //public string Publishers { get; set; }
        public List<string> Publishers { get; set; }

        [Required(ErrorMessage = "Ngày phát hành bắt buộc")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Ngày phát hành")]
        public DateTime ReleaseDay { get; set; }

        [Display(Name = "Miêu tả")]
        public string Description { get; set; }

        //DangVH. Create. Start (02/11/2016)
        [Display(Name = "Ảnh")]
        //DangVH. Create. End (02/11/2016)
        public string ImgPublicId { get; set; }

        [Display(Name = "Thể loại")]
        //public List<BookCategoriesViewModel> Categories { get; set; }
        public List<string> Categories { get; set; }

        public Book()
        {
            //Categories = new List<BookCategoriesViewModel>();
            Categories = new List<string>();
            Publishers = new List<string>();
        }
    }
}