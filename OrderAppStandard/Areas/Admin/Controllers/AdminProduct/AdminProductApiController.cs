using Microsoft.AspNet.Identity;
using OrderApp.DataFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace OrderApp.Areas.Admin.Controllers.AdminProduct
{
    [Authorize]
    [RoutePrefix("api/admin/product")]
    public class AdminProductApiController : ApiController
    {
        private AdminProductService services = new AdminProductService();
        private OrderAppEntities db = new OrderAppEntities();
        private string userId = null;
        private int companyId = 0;
        private AdminProductApiController()
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
        public IHttpActionResult Update(AdminProductDto.UpdateDto dto)
        {
            var result = new Support.ResponsesAPI();

            if (dto.Product.ProductId < 1)
            {
                return Ok(services.Create(companyId, dto));
            }
            else
            {
                var product = db.Product.FirstOrDefault(x => x.ProductId == dto.Product.ProductId);
                if (product == null)
                {
                    result.success = false;
                    result.messageForUser = "Dữ liệu không tồn tại!";
                    return Ok(result);
                }
                //Check company
                if (product.CompanyId != companyId)
                {
                    result.success = false;
                    result.messageForUser = Support.ResponsesAPI.MessageAPI.hacker;
                    return Ok(result);
                }
                return Ok(services.Edit(dto));
            }
        }
        [HttpPost]
        [Route("upload-image")]
        public IHttpActionResult UploadImage(int productId)
        {
            var result = new Support.ResponsesAPI();

            var httpRequest = HttpContext.Current.Request;
            var server = HttpContext.Current.Server;

            var product = db.Product.FirstOrDefault(x => x.ProductId == productId);
            if (product == null) 
            {
                result.success = false;
                result.messageForUser = "Dữ liệu không tồn tại!";
                return Ok(result);
            }
            //Check company
            if (product.CompanyId != companyId)
            {
                result.success = false;
                result.messageForUser = Support.ResponsesAPI.MessageAPI.hacker;
                return Ok(result);
            }
            return Json(services.UploadImage(productId, httpRequest, server));
        }
    }
}
