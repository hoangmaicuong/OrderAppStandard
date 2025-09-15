using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrderApp.Areas.Admin.Controllers.AdminTable
{
    [Authorize]
    public class AdminTableController : Controller
    {
        // GET: Admin/AdminTable
        public ActionResult Index()
        {
            return View();
        }
    }
}