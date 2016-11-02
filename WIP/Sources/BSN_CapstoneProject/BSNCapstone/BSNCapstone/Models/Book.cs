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

        [Required]
        [Display(Name = "Book Name")]
        public string BookName { get; set; }

        [Required]
        [Display(Name = "Authors")]
        public string Authors { get; set; }

        [Required]
        [Display(Name = "Publishers")]
        public string Publishers { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Release Day")]
        public DateTime ReleaseDay { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        //DangVH. Create. Start (02/11/2016)
        [Display(Name = "Book Image")]
        //DangVH. Create. End (02/11/2016)
        public string ImgPublicId { get; set; }

        public List<BookCategoriesViewModel> Categories { get; set; }

        public Book()
        {
            Categories = new List<BookCategoriesViewModel>();
        }
    }
}