using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OrderApp.Controllers.Product
{
    [RoutePrefix("api/product")]
    public class ProductApiController : ApiController
    {
        private ProductService services = new ProductService();
        //[HttpGet]
        //[Route("get-detail")]
        //public IHttpActionResult GetDetail(int id)
        //{
        //    return Ok(services.GetDetail(id));
        //}
    }
}
