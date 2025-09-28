using Microsoft.AspNet.Identity;
using OrderApp.Areas.Admin.Controllers.AdminTable;
using OrderApp.DataFactory;
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
        private OrderAppEntities db = new OrderAppEntities();
        private string userId = null;
        private int companyId = 0;
        private AdminOrderApiController()
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
        [Route("order-in-process")]
        public IHttpActionResult OrderInProcess(int orderId)
        {
            return Ok(services.OrderInProcess(orderId));
        }
        [HttpPost]
        [Route("order-delivered")]
        public IHttpActionResult OrderDelivered(int orderId)
        {
            return Ok(services.OrderDelivered(orderId));
        }
        [HttpPost]
        [Route("restore-order")]
        public IHttpActionResult RestoreOrder(int orderId)
        {
            return Ok(services.RestoreOrder(orderId));
        }
    }
}
