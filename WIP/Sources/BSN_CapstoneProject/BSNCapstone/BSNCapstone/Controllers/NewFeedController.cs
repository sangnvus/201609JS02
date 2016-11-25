using BSNCapstone.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BSNCapstone.Models;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Microsoft.AspNet.Identity;

namespace BSNCapstone.Controllers
{
    [Authorize]
    public class NewFeedController : Controller
    {
        private readonly ApplicationIdentityContext Context = ApplicationIdentityContext.Create();

        public ActionResult Index()
        {
            ViewBag.currentUser = User.Identity.GetUserId();
            return View();
        }
    }
}