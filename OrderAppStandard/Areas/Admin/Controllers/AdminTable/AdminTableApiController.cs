using Microsoft.AspNet.Identity;
using OrderApp.Areas.Admin.Controllers.AdminProduct;
using OrderApp.DataFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OrderApp.Areas.Admin.Controllers.AdminTable
{
    [Authorize]
    [RoutePrefix("api/admin/table")]
    public class AdminTableApiController : ApiController
    {
        private AdminTableService services = new AdminTableService();
        private OrderAppEntities db = new OrderAppEntities();
        private string userId = null;
        private int companyId = 0;
        private AdminTableApiController()
        {
            userId = User.Identity.GetUserId();
            companyId = db.UserExtension.Find(userId)?.CompanyId ?? 0;
        }
        [HttpGet]
        [Route("get-all")]
        public IHttpActionResult GetAll()
        {
            return Ok(services.GetAll(companyId));
        }
        [HttpPost]
        [Route("update")]
        public IHttpActionResult Update(AdminTableDto.UpdateDto dto)
        {
            if (dto.Table.TableId < 1)
            {
                return Ok(services.Create(dto));
            }
            else
            {
                return Ok(services.Edit(dto));
            }
        }
        [HttpPost]
        [Route("create-new-token")]
        public IHttpActionResult CreateNewToken(int tableId)
        {
            return Ok(services.CreateNewToken(tableId));
        }
    }
}
