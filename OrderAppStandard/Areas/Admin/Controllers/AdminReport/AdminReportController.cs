using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrderApp.Areas.Admin.Controllers.AdminReport
{
    [Authorize(Roles = "admin")]
    public class AdminReportController : Controller
    {
        // GET: Admin/AdminReport
        public ActionResult OrderReport()
        {
            return View();
        }
        public ActionResult ProductReport()
        {
            return View();
        }
        public ActionResult DayOfWeekReport()
        {
            return View();
        }
    }
}