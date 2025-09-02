using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
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
        [Route("upload-images")]
        public IHttpActionResult UploadImages(int productId)
        {
            var httpRequest = HttpContext.Current.Request;
            var server = HttpContext.Current.Server;

            var uploadedFiles = services.UploadImages(productId, httpRequest, server);
            return Json(new { success = true, files = uploadedFiles });
        }
    }
}
