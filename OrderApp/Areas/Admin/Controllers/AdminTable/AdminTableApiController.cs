using OrderApp.Areas.Admin.Controllers.AdminProduct;
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
        [HttpGet]
        [Route("get-all")]
        public IHttpActionResult GetAll()
        {
            return Ok(services.GetAll());
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
    }
}
