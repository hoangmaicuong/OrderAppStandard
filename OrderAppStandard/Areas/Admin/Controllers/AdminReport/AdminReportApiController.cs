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
        [Route("get-order-report")]
        public IHttpActionResult GetOrderReport(DateTime startDate, DateTime endDate, string searchKey = null)
        {
            return Ok(services.GetOrderReport(companyId, startDate, endDate,searchKey));
        }
        [HttpGet]
        [Route("get-product-report")]
        public IHttpActionResult GetProductReport(DateTime startDate, DateTime endDate, string searchKey = null)
        {
            if (searchKey == "null")
            {
                searchKey = null;
            }
            return Ok(services.GetProductReport(companyId, startDate, endDate, searchKey));
        }
        [HttpGet]
        [Route("get-day-of-week-report")]
        public IHttpActionResult GetDayOfWeekReport(DateTime startDate, DateTime endDate, string searchKey = null)
        {
            if (searchKey == "null")
            {
                searchKey = null;
            }
            return Ok(services.GetDayOfWeekReport(companyId, startDate, endDate, searchKey));
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose(); // Giải phóng DbContext
            }
            base.Dispose(disposing); // Cho Web API dọn phần còn lại
        }
    }
}
