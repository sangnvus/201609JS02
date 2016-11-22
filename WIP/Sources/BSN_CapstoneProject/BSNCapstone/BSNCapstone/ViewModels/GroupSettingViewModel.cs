using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BSNCapstone.ViewModels
{
    public class GroupSettingViewModel
    {
        public string Id { get; set; }

        public string CreatorId { get; set; }

        [Required(ErrorMessage="Tên nhóm bắt buộc")]
        [Display(Name="Tên nhóm")]
        public string GroupName { get; set; }

        [Display(Name="Miêu tả nhóm")]
        public string Description { get; set; }

        public bool Lock { get; set; }

        public string Tag { get; set; }

        [Display(Name="Thể loại nhóm")]
        public string GroupType { get; set; }
    }
}