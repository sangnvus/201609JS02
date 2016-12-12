using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BSNCapstone.Controllers;
using BSNCapstone.Models;
using System.Web.Mvc;
using BSNCapstone.App_Start;
using System.Collections.Generic;

namespace BSNCapstone.Tests.Controllers
{
    [TestClass]
    public class BooksControllerTest
    {
        private readonly ApplicationIdentityContext Context = ApplicationIdentityContext.Create();
        private ModelStateDictionary modelState;
        [TestInitialize]
        public void Intialize()
        {
            modelState = new ModelStateDictionary();
        }

        [TestMethod]
        public void TestDetailView()
        {
            //var list = new List<string> 
            //{
            //    "test1", "test2"
            //};
            //var book = new Book
            //{
            //    Id = "testId",
            //    BookName = "testBookName",
            //    Categories = list,
            //    Publishers = list,
            //    Authors = list,
            //    ReleaseDay = DateTime.Now
            //};
            var controller = new BooksController();
            var result = controller.Details("testId") as ViewResult;
            Assert.AreEqual("Details", result.ViewName);
        }

        [TestMethod]
        public void TestIndexView()
        {
            var controller = new BooksController();
            var result = controller.Index("", "", 1) as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestCreateView()
        {
            var controller = new BooksController();
            var book = new Book
            {
                BookName = "",

            };
        }
    }
}
