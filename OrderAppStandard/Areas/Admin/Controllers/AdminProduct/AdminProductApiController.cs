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
            if (dto.Product.ProductId < 1)
            {
                return Ok(services.Create(dto));
            }
            else
            {
                return Ok(services.Edit(dto));
            }
        }
        [HttpPost]
        [Route("upload-image")]
        public IHttpActionResult UploadImage(int productId)
        {
            var httpRequest = HttpContext.Current.Request;
            var server = HttpContext.Current.Server;

            return Json(services.UploadImage(productId, httpRequest, server));
        }
    }
}
