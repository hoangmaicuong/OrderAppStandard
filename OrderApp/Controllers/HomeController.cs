using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrderApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(int tableId = 0, Guid? tableToken = null)
        {
            ViewBag.TableId = tableId;
            ViewBag.TableToken = tableToken;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}