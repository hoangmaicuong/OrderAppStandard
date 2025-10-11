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
            var result = new Support.ResponsesAPI();
            if (dto.Category.CategoryId < 1)
            {
                return Ok(services.Create(companyId ,dto));
            }
            else
            {
                var category = db.Category.FirstOrDefault(x => x.CategoryId == dto.Category.CategoryId);
                if (category == null)
                {
                    result.success = false;
                    result.messageForUser = "Dữ liệu không tồn tại!";
                    return Ok(result);
                }
                // Check Company..
                if (category.CompanyId != companyId)
                {
                    result.success = false;
                    result.messageForUser = Support.ResponsesAPI.MessageAPI.hacker;
                    return Ok(result);
                }
                return Ok(services.Edit(dto));
            }
        }
        [HttpDelete]
        [Route("delete")]
        public IHttpActionResult Delete(int categoryId)
        {
            var result = new Support.ResponsesAPI();
            var category = db.Category.FirstOrDefault(x => x.CategoryId == categoryId);
            if (category == null)
            {
                result.success = false;
                result.messageForUser = "Dữ liệu không tồn tại!";
                return Ok(result);
            }
            // Check Company..
            if (category.CompanyId != companyId)
            {
                result.success = false;
                result.messageForUser = Support.ResponsesAPI.MessageAPI.hacker;
                return Ok(result);
            }
            return Ok(services.Delete(categoryId));
        }
    }
}
