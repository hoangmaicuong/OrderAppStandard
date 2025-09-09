using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrderApp.Areas.Admin.Controllers.AdminCategory
{
    [Authorize]
    public class AdminCategoryController : Controller
    {
        // GET: Admin/AdminCategory
        public ActionResult Index()
        {
            return View();
        }
    }
}