using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BSNCapstone.Models;
using BSNCapstone.Controllers;
using BSNCapstone.App_Start;
using System.Web.Mvc;

namespace BSNCapstone.Tests.Controllers
{
    [TestClass]
    public class CategoryControllerTest
    {
        [TestMethod]
        public void TestCreateCategory()
        {
            var controller = new CategoriesController();
            var result = controller.Create("demo category");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestCreateFailedCategory()
        {
            var controller = new CategoriesController();
            var result = controller.Create(string.Empty) as JsonResult;
            Assert.AreEqual("Tên thể loại bắt buộc", result.Data);
        }
    }
}
