using BSNCapstone.App_Start;
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
using Microsoft.AspNet.Identity;
using BSNCapstone.ViewModels;
using PagedList;

namespace BSNCapstone.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationIdentityContext Context = ApplicationIdentityContext.Create();
        //
        // GET: /Reports/
        public ActionResult Index(int? page)
        {
            ViewBag.allUser = Context.Users.Find(_ => true).ToList();
            ViewBag.allGroup = Context.Groups.Find(_ => true).ToList();
            List<Report> user = Context.Reports.Find(x => x.ReportedUserId != null && x.Status.Equals(true)).ToList();
            List<Report> group = Context.Reports.Find(x => x.ReportedGroupId != null && x.Status.Equals(true)).ToList();
            int pageSize = 6;
            int pageNumber = (page ?? 1);
            ReportContentViewModel report = new ReportContentViewModel() 
            {
                ListReportUser = user.ToPagedList(pageNumber, pageSize),
                ListReportGroup = group.ToPagedList(pageNumber, pageSize)
            };
            return View(report);
        }

        [HttpPost]
        public ActionResult GetReport(string reportContent, string userId, string beReportedId, int option)
        {
            try
            {
                if (reportContent == "undefined")
                {
                    return Json("Xin hãy lựa chọn lý do báo cáo");
                }
                else
                {
                    var report = new Report()
                    {
                        UserId = userId,
                        Content = reportContent,
                        Status = true
                    };
                    if (option == 1)
                    {
                        report.ReportedUserId = beReportedId;
                        report.ReportedGroupId = null;
                    }
                    else
                    {
                        report.ReportedUserId = null;
                        report.ReportedGroupId = beReportedId;
                    }
                    Context.Reports.InsertOne(report);
                    return Json("Cảm ơn bạn đã báo cáo");
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Reject(string id)
        {
            Context.Reports.DeleteOneAsync(x => x.Id.Equals(new ObjectId(id)));
            return Json("");
        }
	}
}