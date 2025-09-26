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
        private string userId = null;
        private int companyId = 0;
        private AdminReportApiController()
        {
            userId = User.Identity.GetUserId();
            companyId = db.UserExtension.Find(userId)?.CompanyId ?? 0;
        }
        [HttpGet]
        [Route("get-filter")]
        public IHttpActionResult GetFilter(DateTime startDate, DateTime endDate, string searchKey = null)
        {
            return Ok(services.GetFilter(companyId, startDate, endDate,searchKey));
        }
    }
}
