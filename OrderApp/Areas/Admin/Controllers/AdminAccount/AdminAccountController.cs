using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrderApp.Areas.Admin.Controllers.AdminAccount
{
    [Authorize]
    public class AdminAccountController : Controller
    {
        // GET: Admin/AdminAccount
        public ActionResult Index()
        {
            return View();
        }
    }
}