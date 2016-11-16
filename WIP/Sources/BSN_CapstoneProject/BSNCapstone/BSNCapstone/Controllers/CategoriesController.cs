using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BSNCapstone.App_Start;
using BSNCapstone.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using BSNCapstone.ControllerHelpers;

namespace BSNCapstone.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly ApplicationIdentityContext Context = ApplicationIdentityContext.Create();
        //
        // GET: /Categories/
        public ActionResult Index()
        {
            var categories = Context.Categories.Find(_ => true).ToEnumerable();
            return View(categories);
        }

        //DangVH. Delete. Start (16/11/2016)
        //
        // GET: /Categories/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        //
        // GET: /Categories/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}
        //DangVH. Delete. End (16/11/2016)

        //
        // POST: /Categories/Create
        [HttpPost]
        public ActionResult Create(string categoryName)
        {
            //DangVH. Update. Start (16/11/2016)
            //if (ModelState.IsValid)
            //{
            //    // TODO: Add insert logic here
            //    Context.Categories.InsertOne(category);
            //    return RedirectToAction("Index");
            //}
            //return View();
            try
            {
                if (categoryName == "")
                {
                    return Json("Tên thể loại bắt buộc");
                }
                Category category = new Category()
                {
                    CategoryName = categoryName
                };
                // TODO: Add insert logic here
                Context.Categories.InsertOne(category);
                return Json("Thêm thành công");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
            //DangVH. Update. End (16/11/2016)
        }

        //
        // GET: /Categories/Edit/5
        public ActionResult Edit(string id)
        {
            var category = Context.Categories.Find(x => x.Id.Equals(new ObjectId(id))).FirstOrDefault();
            return View(category);
        }

        //
        // POST: /Categories/Edit/5
        [HttpPost]
        public ActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                // TODO: Add update logic here
                Context.Categories.ReplaceOneAsync(x => x.Id.Equals(new ObjectId(category.Id)), category);
                return RedirectToAction("Index");
            }
            return View();
        }

        //
        // GET: /Categories/Delete/5
        public ActionResult Delete(string id)
        {
            var category = Context.Categories.Find(x => x.Id.Equals(new ObjectId(id))).FirstOrDefault();
            return View(category);
        }

        //
        // POST: /Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(string id)
        {
            // TODO: Add delete logic here
            Context.Categories.DeleteOneAsync(x => x.Id.Equals(new ObjectId(id)));
            return RedirectToAction("Index");
        }
    }
}

