using OrderApp.Areas.Admin.Controllers.AdminTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OrderApp.Areas.Admin.Controllers.AdminOrder
{
    [Authorize]
    [RoutePrefix("api/admin/order")]
    public class AdminOrderApiController : ApiController
    {
        private AdminOrderService services = new AdminOrderService();
        [HttpGet]
        [Route("get-all")]
        public IHttpActionResult GetAll()
        {
            return Ok(services.GetAll());
        }
    }
}
