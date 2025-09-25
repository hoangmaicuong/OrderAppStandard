using Microsoft.AspNet.Identity;
using OrderApp.DataFactory;
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
        private OrderAppEntities db = new OrderAppEntities();
        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            var userExtension = db.UserExtension.FirstOrDefault(x => x.AspNetUserId == userId);
            ViewBag.CompanySlug = userExtension?.Company.Slug;
            return View();
        }
    }
}