using AspNet.Identity.MongoDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bookaholic.App_Start
{
    public class EnsuaAuthIndexes
    {
        public static void Exist()
        {

            var context = ApplicationIdentityContext.Create();

            IndexChecks.EnsureUniqueIndexOnUserName(context.Users);

            IndexChecks.EnsureUniqueIndexOnRoleName(context.Roles);

        }
    }
}