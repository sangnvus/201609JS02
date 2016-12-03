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
    public class GroupsControllerHelper
    {
        public static int GetGroupNumber()
        {
            ApplicationIdentityContext Context = ApplicationIdentityContext.Create();
            return (int)Context.Groups.Find(_ => true).Count();
        }

        public static int GetGroupJustCreatedNumber()
        {
            ApplicationIdentityContext Context = ApplicationIdentityContext.Create();
            var groups = Context.Groups.Find(_ => true).ToList();
            var a = groups.Where(x => x.CreatedDate.ToShortDateString().Equals(DateTime.Now.ToShortDateString()));
            return (int)a.Count();
        }
    }
}