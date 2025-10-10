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
        private readonly OrderAppEntities db;
        private AdminOrderService services;
        private string userId = null;
        private int companyId = 0;
        private AdminOrderApiController()
        {
            db = new OrderAppEntities();
            services = new AdminOrderService(db);

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
            var order = db.Order.FirstOrDefault(x => x.OrderId == orderId);
            if(order == null)
            {
                return NotFound();
            }
            // Check Company..
            if(order.CompanyId != companyId)
            {
                return Unauthorized();
            }
            return Ok(services.GetOrderDetails(order));
        }
        [HttpPost]
        [Route("remove-order-detail")]
        public IHttpActionResult RemoveOrderDetail(int orderDetailId)
        {
            var orderDetail = db.OrderDetail.FirstOrDefault(x => x.OrderDetailId == orderDetailId);
            if (orderDetail == null)
            {
                return NotFound();
            }
            var order = db.Order.FirstOrDefault(x => x.OrderId == orderDetail.OrderId);
            if (order == null)
            {
                return NotFound();
            }
            // Check Company..
            if (order.CompanyId != companyId)
            {
                return Unauthorized();
            }
            return Ok(services.RemoveOrderDetail(orderDetail));
        }
        [HttpPost]
        [Route("change-status-to-finish")]
        public IHttpActionResult ChangeStatusToFinish(int orderId)
        {
            var order = db.Order.FirstOrDefault(x => x.OrderId == orderId);
            if (order == null)
            {
                return NotFound();
            }
            // Check Company..
            if (order.CompanyId != companyId)
            {
                return Unauthorized();
            }
            return Ok(services.ChangeStatusToFinish(order));
        }
        [HttpPost]
        [Route("order-confirm")]
        public IHttpActionResult OrderConfirm(int orderId)
        {
            var order = db.Order.FirstOrDefault(x => x.OrderId == orderId);
            if (order == null)
            {
                return NotFound();
            }
            // Check Company..
            if (order.CompanyId != companyId)
            {
                return Unauthorized();
            }
            return Ok(services.OrderConfirm(order));
        }
        [HttpPost]
        [Route("order-in-process")]
        public IHttpActionResult OrderInProcess(int orderId)
        {
            var order = db.Order.FirstOrDefault(x => x.OrderId == orderId);
            if (order == null)
            {
                return NotFound();
            }
            // Check Company..
            if (order.CompanyId != companyId)
            {
                return Unauthorized();
            }
            return Ok(services.OrderInProcess(order));
        }
        [HttpPost]
        [Route("order-delivered")]
        public IHttpActionResult OrderDelivered(int orderId)
        {
            var order = db.Order.FirstOrDefault(x => x.OrderId == orderId);
            if (order == null)
            {
                return NotFound();
            }
            // Check Company..
            if (order.CompanyId != companyId)
            {
                return Unauthorized();
            }
            return Ok(services.OrderDelivered(order));
        }
        [HttpPost]
        [Route("restore-order")]
        public IHttpActionResult RestoreOrder(int orderId)
        {
            var order = db.Order.FirstOrDefault(x => x.OrderId == orderId);
            if (order == null)
            {
                return NotFound();
            }
            // Check Company..
            if (order.CompanyId != companyId)
            {
                return Unauthorized();
            }
            return Ok(services.RestoreOrder(order));
        }
        [HttpPost]
        [Route("delivered-order-detail")]
        public IHttpActionResult DeliveredOrderDetail(int orderDetailId)
        {
            return Ok(services.DeliveredOrderDetail(orderDetailId));
        }
    }
}
