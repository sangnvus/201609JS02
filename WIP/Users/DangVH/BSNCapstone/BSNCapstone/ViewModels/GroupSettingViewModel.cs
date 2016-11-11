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

        [Required]
        public string GroupName { get; set; }

        public string Description { get; set; }

        public bool Lock { get; set; }

        public string Tag { get; set; }

        public string GroupType { get; set; }
    }
}