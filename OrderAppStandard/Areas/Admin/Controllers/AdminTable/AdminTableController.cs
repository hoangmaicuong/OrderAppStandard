using Microsoft.AspNet.Identity;
using OrderApp.DataFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

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
        public ActionResult TableOfCurrentOrderList(int tableId = 0, Guid? tableToken = null)
        {
            var table = db.Table.FirstOrDefault(x => x.TableId == tableId && x.TableToken == tableToken);
            if (table == null)
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
            ViewBag.TableId = tableId;
            ViewBag.TableToken = tableToken;
            ViewBag.TableName = table.TableName;

            return View();
        }
    }
}