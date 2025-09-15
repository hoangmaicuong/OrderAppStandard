using OrderApp.Areas.Admin.Controllers.AdminTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OrderApp.Areas.Admin.Controllers.AdminCategory
{
    [Authorize]
    [RoutePrefix("api/admin/category")]
    public class AdminCategoryApiController : ApiController
    {
        private AdminCategoryService services = new AdminCategoryService();
        [HttpGet]
        [Route("get-all")]
        public IHttpActionResult GetAll()
        {
            return Ok(services.GetAll());
        }
        [HttpPost]
        [Route("update")]
        public IHttpActionResult Update(AdminCategoryDto.UpdateDto dto)
        {
            if (dto.Category.CategoryId < 1)
            {
                return Ok(services.Create(dto));
            }
            else
            {
                return Ok(services.Edit(dto));
            }
        }
        [HttpDelete]
        [Route("delete")]
        public IHttpActionResult Delete(int categoryId)
        {
            return Ok(services.Delete(categoryId));
        }
    }
}
