using Microsoft.AspNet.Identity;
using OrderApp.Areas.Admin.Controllers.AdminTable;
using OrderApp.DataFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static OrderApp.Areas.Admin.Controllers.AdminCompany.AdminCompanyDto;

namespace OrderApp.Areas.Admin.Controllers.AdminCompany
{
    [Authorize(Roles = "admin")]
    [RoutePrefix("api/admin/company")]
    public class AdminCompanyApiController : ApiController
    {
        private AdminCompanyService services;
        private OrderAppEntities db;
        private DapperContext dapperContext = DapperContext.dapperContext;
        private string userId = null;
        private int companyId = 0;
        private AdminCompanyApiController()
        {
            db = new OrderAppEntities();
            services = new AdminCompanyService(db, dapperContext);

            userId = User.Identity.GetUserId();
            companyId = db.UserExtension.Find(userId)?.CompanyId ?? 0;
        }
        [HttpGet]
        [Route("get-detail")]
        public IHttpActionResult GetDetail()
        {
            return Ok(services.GetDetail(companyId));
        }
        [HttpPost]
        [Route("update")]
        public IHttpActionResult Update(AdminCompanyDto.UpdateDto dto)
        {
            var result = new Support.ResponsesAPI();

            return Ok(services.Edit(companyId, userId, dto));
        }
    }
}
