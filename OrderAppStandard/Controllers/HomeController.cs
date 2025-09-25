using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrderApp.Controllers
{
    public class HomeController : Controller
    {
        [Route("{CompanySlug}")]
        public ActionResult Index( string CompanySlug, int tableId = 0, Guid? tableToken = null)
        {
            ViewBag.TableId = tableId;
            ViewBag.TableToken = tableToken;
            ViewBag.CompanySlug = CompanySlug;
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
        [Route("not-found")]
        public ActionResult NotFound()
        {
            return View();
        }
    }
}