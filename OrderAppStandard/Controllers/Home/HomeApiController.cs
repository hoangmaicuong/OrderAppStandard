using OrderApp.Areas.Admin.Controllers.AdminProduct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace OrderApp.Controllers.Home
{
    [RoutePrefix("api/home")]
    public class HomeApiController : ApiController
    {
        private HomeService services = new HomeService();
        [HttpGet]
        [Route("get-all")]
        public IHttpActionResult GetAll(string companySlug)
        {
            return Ok(services.GetAll(companySlug));
        }
        [HttpPost]
        [Route("create-order")]
        public IHttpActionResult CreateOrder(HomeDto.CreateOrderDto dto)
        {
            return Ok(services.CreateOrder(dto));
        }
        [HttpGet]
        [Route("get-order-of-table")]
        public IHttpActionResult GetOrderOfTable(int tableId, Guid tableToken)
        {
            return Ok(services.GetOrderOfTable(tableId, tableToken));
        }
        [HttpGet]
        [Route("get-table")]
        public IHttpActionResult GetTable(int tableId, Guid tableToken)
        {
            return Ok(services.GetTable(tableId, tableToken));
        }
    }
}
