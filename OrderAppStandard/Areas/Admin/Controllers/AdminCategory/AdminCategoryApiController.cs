using Microsoft.AspNet.Identity;
using OrderApp.Areas.Admin.Controllers.AdminTable;
using OrderApp.DataFactory;
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
        private OrderAppEntities db = new OrderAppEntities();
        private string userId = null;
        private int companyId = 0;
        private AdminCategoryApiController()
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
