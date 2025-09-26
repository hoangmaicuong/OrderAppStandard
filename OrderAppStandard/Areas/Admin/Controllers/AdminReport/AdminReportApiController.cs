using Microsoft.AspNet.Identity;
using OrderApp.Areas.Admin.Controllers.AdminOrder;
using OrderApp.DataFactory;
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
        private OrderAppEntities db = new OrderAppEntities();
        [HttpGet]
        [Route("get-filter")]
        public IHttpActionResult GetFilter(DateTime startDate, DateTime endDate, string searchKey = null)
        {
            string userId = User.Identity.GetUserId();
            int companyId = db.UserExtension.Find(userId)?.CompanyId ?? 0;
            return Ok(services.GetFilter(companyId, startDate, endDate,searchKey));
        }
    }
}
