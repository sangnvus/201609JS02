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
    public class Group
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string CreatorId { get; set; }

        [Required(ErrorMessage = "Bắt buộc")]
        [Display(Name = "Tên nhóm")]
        public string GroupName { get; set; }

        public List<GroupMembersViewModel> GroupMembers { get; set; }

        [Required(ErrorMessage = "Bắt buộc")]
        [Display(Name = "Thẻ")]
        public string Tag { get; set; }


        [Required(ErrorMessage = "Bắt buộc")]
        [Display(Name = "Kiểu nhóm")]
        public string GroupType { get; set; }

        [Display(Name = "Ngày lập")]
        public DateTime CreatedDate { get; set; }

        public string AvaImg { get; set; }

        public string CoverImg { get; set; }

        [Display(Name = "Trạng thái")]
        public bool Locked { get; set; }

        [Display(Name = "Miêu tả")]
        public string Description { get; set; }

        public List<string> ListJoinRequest { get; set; }

        public Group()
        {
            Locked = false;
            CreatedDate = DateTime.Now.AddHours(7);
            GroupMembers = new List<GroupMembersViewModel>();
            ListJoinRequest = new List<string>();
        }
    }
}