using Microsoft.AspNet.Identity;
using OrderApp.Areas.Admin.Controllers.AdminProduct;
using OrderApp.DataFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.UI.WebControls;

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
            var result = new Support.ResponsesAPI();

            if (dto.Table.TableId < 1)
            {
                return Ok(services.Create(companyId, dto));
            }
            else
            {
                var table = db.Table.FirstOrDefault(x => x.TableId == dto.Table.TableId);
                if (table == null)
                {
                    result.success = false;
                    result.messageForUser = "Dữ liệu không tồn tại!";
                    return Ok(result);
                }
                if (table.CompanyId != companyId)
                {
                    result.success = false;
                    result.messageForUser = Support.ResponsesAPI.MessageAPI.hacker;
                    return Ok(result);
                }
                return Ok(services.Edit(dto));
            }
        }
        [HttpPost]
        [Route("create-new-token")]
        public IHttpActionResult CreateNewToken(int tableId)
        {
            var result = new Support.ResponsesAPI();
            var table = db.Table.FirstOrDefault(x => x.TableId == tableId);
            if(table == null)
            {
                result.success = false;
                result.messageForUser = "Dữ liệu không tồn tại!";
                return Ok(result);
            }
            if(table.CompanyId != companyId)
            {
                result.success = false;
                result.messageForUser = Support.ResponsesAPI.MessageAPI.hacker;
                return Ok(result);
            }
            return Ok(services.CreateNewToken(tableId));
        }
        [HttpGet]
        [Route("get-order-of-table")]
        public IHttpActionResult GetOrderOfTable(int tableId, Guid tableToken)
        {
            return Ok(services.GetOrderOfTable(tableId, tableToken));
        }
    }
}
