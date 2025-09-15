using OrderApp.Areas.Admin.Controllers.AdminOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OrderApp.Areas.Admin.Controllers.AdminReport
{
    [Authorize(Roles = "admin")]
    [RoutePrefix("api/admin/report")]
    public class AdminReportApiController : ApiController
    {
        private AdminReportService services = new AdminReportService();
        [HttpGet]
        [Route("get-filter")]
        public IHttpActionResult GetFilter(DateTime startDate, DateTime endDate, string searchKey = null)
        {
            return Ok(services.GetFilter(startDate, endDate,searchKey));
        }
    }
}
