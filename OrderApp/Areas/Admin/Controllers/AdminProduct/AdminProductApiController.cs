using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OrderApp.Areas.Admin.Controllers.AdminProduct
{
    [RoutePrefix("api/admin/product")]
    public class AdminProductApiController : ApiController
    {
        private AdminProductService services = new AdminProductService();
        [HttpGet]
        [Route("get-all")]
        public IHttpActionResult GetAll()
        {
            return Ok(services.GetAll());
        }
        [HttpPost]
        [Route("update")]
        public IHttpActionResult Update(AdminProductDto.UpdateDto dto)
        {
            return Ok(services.Update(dto));
        }
    }
}
