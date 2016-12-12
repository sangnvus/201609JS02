using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BSNCapstone.Models;
using BSNCapstone.Controllers;
using BSNCapstone.App_Start;
using System.Web.Mvc;
using BSNCapstone.ControllerHelpers;

namespace BSNCapstone.Tests.Controllers
{
    [TestClass]
    public class PublisherControllerTest
    {
        [TestMethod]
        public void TestCreatePublisher()
        {
            var controller = new PublisherController();
        }

        [TestMethod]
        public void TestIndexPublisher()
        {
            var controller = new PublisherController();
            var result = controller.Index(1);
            Assert.IsNotNull(result);
        }
    }
}
