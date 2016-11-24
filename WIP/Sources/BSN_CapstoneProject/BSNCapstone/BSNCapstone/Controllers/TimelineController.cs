﻿using BSNCapstone.App_Start;
using BSNCapstone.ControllerHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using MongoDB.Bson;
using BSNCapstone.Models;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Globalization;

namespace BSNCapstone.Controllers
{
    [Authorize]
    public class TimelineController : Controller
    {
        private readonly ApplicationIdentityContext Context = ApplicationIdentityContext.Create();
        private IdentityConfig.ApplicationUserManager _userManager;
        public IdentityConfig.ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<IdentityConfig.ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Timeline
        public ActionResult Index()
        {
           return View();
        }


        //
        // GET: /UserProfile/Edit/5
        public ActionResult EditProfile(string id)
        {
            var user = Context.Users.Find(x => x.Id.Equals(new ObjectId(id))).FirstOrDefault();
            UserProfile newprofile = new UserProfile()
            {
                id = user.Id,
                UserName = user.UserName,
                DOB = Convert.ToDateTime(user.DOB).ToString("dd/MM/yyyy"),
                Address = user.Address
            };
            return View(newprofile);
        }

        //
        // POST: /UserProfile/Edit/5
        [HttpPost]
        public async Task<ActionResult> EditProfile(UserProfile user1)
        {
            if (ModelState.IsValid)
            {
                var user2 = await UserManager.FindByIdAsync(user1.id);

                if (user2 == null)
                {
                    return HttpNotFound();
                }

                user2.Address = user1.Address;
                user2.DOB = DateTime.ParseExact(user1.DOB, "dd/MM/yyyy", CultureInfo.InvariantCulture).AddHours(7);
                user2.UserName = user1.UserName;
                await Context.Users.ReplaceOneAsync(x => x.Id.Equals(new ObjectId(user2.Id)), user2);
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
    }
}