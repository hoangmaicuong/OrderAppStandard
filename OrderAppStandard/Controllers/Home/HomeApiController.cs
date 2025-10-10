using OrderApp.Areas.Admin.Controllers.AdminProduct;
using OrderApp.DataFactory;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace OrderApp.Controllers.Home
{
    [RoutePrefix("api/home")]
    public class HomeApiController : ApiController
    {
        private readonly OrderAppEntities db;
        private readonly HomeService services;
        string serviceAccountPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content/serviceAccountKey.json");
        public HomeApiController()
        {
            db = new OrderAppEntities();
            services = new HomeService(db);
        }
        [HttpGet]
        [Route("get-all")]
        public async Task<IHttpActionResult> GetAllAsync(string companySlug)
        {
            var result = await services.GetAllAsync(companySlug);
            return Ok(result);
        }
        //[HttpGet]
        //[Route("get-all")]
        //public IHttpActionResult GetAll(string companySlug)
        //{
        //    return Ok(services.GetAll(companySlug));
        //}
        [HttpPost]
        [Route("create-order")]
        public IHttpActionResult CreateOrder(string companySlug, HomeDto.CreateOrderDto dto)
        {
            return Ok(services.CreateOrder(companySlug, dto, serviceAccountPath));
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
        [HttpPost]
        [Route("call-staff")]
        public IHttpActionResult CallStaff(int tableId, Guid tableToken)
        {
            return Ok(services.CallStaff(tableId, tableToken, serviceAccountPath));
        }
    }
}
