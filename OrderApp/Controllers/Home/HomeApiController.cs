using OrderApp.Areas.Admin.Controllers.AdminProduct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace OrderApp.Controllers.Home
{
    [RoutePrefix("api/home")]
    public class HomeApiController : ApiController
    {
        private HomeService services = new HomeService();
        [HttpGet]
        [Route("get-all")]
        public IHttpActionResult GetAll()
        {
            return Ok(services.GetAll());
        }
    }
}
