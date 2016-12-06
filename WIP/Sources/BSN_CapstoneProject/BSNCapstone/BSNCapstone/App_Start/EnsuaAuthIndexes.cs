using AspNet.Identity.MongoDB;
using BSNCapstone.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSNCapstone.App_Start
{
    public class EnsuaAuthIndexes
    {
        public static void Exist()
        {

            var context = ApplicationIdentityContext.Create();
            IndexChecks.EnsureUniqueIndexOnUserName(context.Users);
            IndexChecks.EnsureUniqueIndexOnRoleName(context.Roles);
            context.Books.Indexes.CreateOne(Builders<Book>.IndexKeys.Ascending("BookName"));
        }
    }
}