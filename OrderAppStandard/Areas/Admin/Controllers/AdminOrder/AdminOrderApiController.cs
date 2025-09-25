using OrderApp.Areas.Admin.Controllers.AdminTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OrderApp.Areas.Admin.Controllers.AdminOrder
{
    [Authorize]
    [RoutePrefix("api/admin/order")]
    public class AdminOrderApiController : ApiController
    {
        private AdminOrderService services = new AdminOrderService();
        [HttpGet]
        [Route("get-all")]
        public IHttpActionResult GetAll(string companySlug)
        {
            return Ok(services.GetAll(companySlug));
        }
        [HttpGet]
        [Route("get-order-details")]
        public IHttpActionResult GetOrderDetails(int orderId)
        {
            return Ok(services.GetOrderDetails(orderId));
        }
        [HttpPost]
        [Route("remove-order-detail")]
        public IHttpActionResult RemoveOrderDetail(int orderDetailId)
        {
            return Ok(services.RemoveOrderDetail(orderDetailId));
        }
        [HttpPost]
        [Route("change-status-to-finish")]
        public IHttpActionResult ChangeStatusToFinish(int orderId)
        {
            return Ok(services.ChangeStatusToFinish(orderId));
        }
        [HttpPost]
        [Route("order-confirm")]
        public IHttpActionResult OrderConfirm(int orderId)
        {
            return Ok(services.OrderConfirm(orderId));
        }
        [HttpPost]
        [Route("restore-order")]
        public IHttpActionResult RestoreOrder(int orderId)
        {
            return Ok(services.RestoreOrder(orderId));
        }
    }
}
