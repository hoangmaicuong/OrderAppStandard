using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrderApp.Areas.Admin.Controllers.AdminOrder
{
    [Authorize]
    public class AdminOrderController : Controller
    {
        // GET: Admin/AdminOrder
        public ActionResult Index()
        {
            return View();
        }
    }
}