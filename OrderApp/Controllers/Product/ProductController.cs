using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrderApp.Controllers.Product
{
    [RoutePrefix("san-pham")]
    public class ProductController : Controller
    {
        private ProductService services = new ProductService();
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }
    }
}