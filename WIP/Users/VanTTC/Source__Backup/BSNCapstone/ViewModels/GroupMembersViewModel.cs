using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSNCapstone.ViewModels
{
    public class GroupMembersViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string RoleInGroup { get; set; }
        public bool IsSelected { get; set; }
    }
}