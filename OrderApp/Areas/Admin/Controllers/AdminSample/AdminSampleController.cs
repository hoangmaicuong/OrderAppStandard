using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace OrderApp.Areas.Admin.Controllers._1Sample
{
    //[Authorize(Roles = "admin")]
    public class AdminSampleController : Controller
    {
        // GET: Admin/AdminSample
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Detail(int id = 0)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult SinglePage()
        {
            return View();
        }
        public ActionResult SinglePageList()
        {
            return View();
        }
    }
}