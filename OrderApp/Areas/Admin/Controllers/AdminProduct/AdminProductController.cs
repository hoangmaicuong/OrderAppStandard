using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrderApp.Areas.Admin.Controllers.AdminProduct
{
    [Authorize]
    public class AdminProductController : Controller
    {
        // GET: Admin/AdminProduct
        public ActionResult Index()
        {
            return View();
        }
    }
}