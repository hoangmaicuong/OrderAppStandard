using Microsoft.AspNet.Identity;
using OrderApp.Areas.Admin.Controllers.AdminTable;
using OrderApp.DataFactory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace OrderApp.Areas.Admin.Controllers.AdminOrder
{
    [Authorize]
    [RoutePrefix("api/admin/order")]
    public class AdminOrderApiController : ApiController
    {
        private readonly OrderAppEntities db;
        private DapperContext dapperContext = DapperContext.dapperContext;
        private AdminOrderService services;
        private string userId = null;
        private int companyId = 0;
        string serviceAccountPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content/serviceAccountKey.json");
        private AdminOrderApiController()
        {
            db = new OrderAppEntities();
            services = new AdminOrderService(db, dapperContext);

            userId = User.Identity.GetUserId();
            companyId = db.UserExtension.Find(userId)?.CompanyId ?? 0;
        }
        [HttpGet]
        [Route("get-all")]
        public async Task<IHttpActionResult> GetAllAsync()
        {
            var result = await services.GetAllAsync(companyId);
            return Ok(result);
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
            var result = new Support.ResponsesAPI();
            var orderDetail = db.OrderDetail.FirstOrDefault(x => x.OrderDetailId == orderDetailId);
            if (orderDetail == null)
            {
                result.success = false;
                result.messageForUser = "Dữ liệu không tồn tại!";
                return Ok(result);
            }
            var order = db.Order.FirstOrDefault(x => x.OrderId == orderDetail.OrderId);
            if (order == null)
            {
                result.success = false;
                result.messageForUser = "Dữ liệu không tồn tại!";
                return Ok(result);
            }
            // Check Company..
            if (order.CompanyId != companyId)
            {
                result.success = false;
                result.messageForUser = Support.ResponsesAPI.MessageAPI.hacker;
                return Ok(result);
            }
            result = services.RemoveOrderDetail(orderDetail);
            return Ok(result);
        }
        [HttpPost]
        [Route("change-status-to-finish")]
        public IHttpActionResult ChangeStatusToFinish(int orderId)
        {
            var result = new Support.ResponsesAPI();
            var order = db.Order.FirstOrDefault(x => x.OrderId == orderId);
            if (order == null)
            {
                result.success = false;
                result.messageForUser = "Dữ liệu không tồn tại!";
                return Ok(result);
            }
            // Check Company..
            if (order.CompanyId != companyId)
            {
                result.success = false;
                result.messageForUser = Support.ResponsesAPI.MessageAPI.hacker;
                return Ok(result);
            }
            result = services.ChangeStatusToFinish(order);
            return Ok(result);
        }
        [HttpPost]
        [Route("order-confirm")]
        public IHttpActionResult OrderConfirm(int orderId)
        {
            var result = new Support.ResponsesAPI();
            var order = db.Order.FirstOrDefault(x => x.OrderId == orderId);
            if (order == null)
            {
                result.success = false;
                result.messageForUser = "Dữ liệu không tồn tại!";
                return Ok(result);
            }
            // Check Company..
            if (order.CompanyId != companyId)
            {
                result.success = false;
                result.messageForUser = Support.ResponsesAPI.MessageAPI.hacker;
                return Ok(result);
            }
            result = services.OrderConfirm(order, serviceAccountPath);
            return Ok(result);
        }
        [HttpPost]
        [Route("order-in-process")]
        public IHttpActionResult OrderInProcess(int orderId)
        {
            var result = new Support.ResponsesAPI();
            var order = db.Order.FirstOrDefault(x => x.OrderId == orderId);
            if (order == null)
            {
                result.success = false;
                result.messageForUser = "Dữ liệu không tồn tại!";
                return Ok(result);
            }
            // Check Company..
            if (order.CompanyId != companyId)
            {
                result.success = false;
                result.messageForUser = Support.ResponsesAPI.MessageAPI.hacker;
                return Ok(result);
            }
            result = services.OrderInProcess(order, serviceAccountPath);
            return Ok(result);
        }
        [HttpPost]
        [Route("order-delivered")]
        public IHttpActionResult OrderDelivered(int orderId)
        {
            var result = new Support.ResponsesAPI();
            var order = db.Order.FirstOrDefault(x => x.OrderId == orderId);
            if (order == null)
            {
                result.success = false;
                result.messageForUser = "Dữ liệu không tồn tại!";
                return Ok(result);
            }
            // Check Company..
            if (order.CompanyId != companyId)
            {
                result.success = false;
                result.messageForUser = Support.ResponsesAPI.MessageAPI.hacker;
                return Ok(result);
            }
            result = services.OrderDelivered(order);
            return Ok(result);
        }
        [HttpPost]
        [Route("restore-order")]
        public IHttpActionResult RestoreOrder(int orderId)
        {
            var result = new Support.ResponsesAPI();
            var order = db.Order.FirstOrDefault(x => x.OrderId == orderId);
            if (order == null)
            {
                result.success = false;
                result.messageForUser = "Dữ liệu không tồn tại!";
                return Ok(result);
            }
            // Check Company..
            if (order.CompanyId != companyId)
            {
                result.success = false;
                result.messageForUser = Support.ResponsesAPI.MessageAPI.hacker;
                return Ok(result);
            }
            result = services.RestoreOrder(order);
            return Ok(result);
        }
        [HttpPost]
        [Route("delivered-order-detail")]
        public IHttpActionResult DeliveredOrderDetail(int orderDetailId)
        {
            var result = new Support.ResponsesAPI();
            var orderDetail = db.OrderDetail.FirstOrDefault(x => x.OrderDetailId == orderDetailId);
            if (orderDetail == null)
            {
                result.success = false;
                result.messageForUser = "Dữ liệu không tồn tại!";
                return Ok(result);
            }
            var order = db.Order.FirstOrDefault(x => x.OrderId == orderDetail.OrderId);
            if (order == null)
            {
                result.success = false;
                result.messageForUser = "Dữ liệu không tồn tại!";
                return Ok(result);
            }
            // Check Company..
            if (order.CompanyId != companyId)
            {
                result.success = false;
                result.messageForUser = Support.ResponsesAPI.MessageAPI.hacker;
                return Ok(result);
            }
            result = services.DeliveredOrderDetail(orderDetail, serviceAccountPath);
            return Ok(result);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose(); // Giải phóng DbContext
            }
            base.Dispose(disposing); // Cho Web API dọn phần còn lại
        }
    }
}
