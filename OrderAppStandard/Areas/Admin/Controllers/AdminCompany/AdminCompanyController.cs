using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrderApp.Areas.Admin.Controllers.AdminCompany
{
    [Authorize(Roles = "admin")]
    public class AdminCompanyController : Controller
    {
        // GET: Admin/AdminCompany
        public ActionResult Index()
        {
            return View();
        }
    }
}