using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using OrderApp.DataFactory;
using OrderApp.Models;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http;

namespace OrderApp.Areas.Admin.Controllers.AdminAccount
{
    [Authorize(Roles = "admin")]
    [RoutePrefix("api/admin/account")]
    public class AdminAccountApiController : ApiController
    {
        private AdminAccountService services;
        private OrderAppEntities db;
        private DapperContext dapperContext = DapperContext.dapperContext;
        private string userId = null;
        private int companyId = 0;
        private AdminAccountApiController()
        {
            db = new OrderAppEntities();
            services = new AdminAccountService(db, dapperContext);

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
        public async Task<IHttpActionResult> Update(AdminAccountDto.UpdateDto dto)
        {
            var result = new Support.ResponsesAPI();
            if (string.IsNullOrEmpty(dto.AspNetUser.Id))
            {
                return Ok(await services.Create(companyId ,dto));
            }
            else
            {
                var userExten = await db.UserExtension.FirstOrDefaultAsync(x => x.AspNetUserId == dto.AspNetUser.Id);
                if (userExten == null)
                {
                    result.success = false;
                    result.messageForUser = "Dữ liệu không tồn tại!";
                    return Ok(result);
                }
                //Check company
                if (userExten.CompanyId != companyId)
                {
                    result.success = false;
                    result.messageForUser = Support.ResponsesAPI.MessageAPI.hacker;
                    return Ok(result);
                }
                return Ok(await services.Edit(dto));
            }
        }
        [HttpPost]
        [Route("change-password")]
        public async Task<IHttpActionResult> ChangePassword(AdminAccountDto.UpdateDto dto)
        {
            var result = new Support.ResponsesAPI();
            var userExten = await db.UserExtension.FirstOrDefaultAsync(x => x.AspNetUserId == dto.AspNetUser.Id);
            if(userExten == null)
            {
                result.success = false;
                result.messageForUser = "Dữ liệu không tồn tại!";
                return Ok(result);
            }
            //Check company
            if(userExten.CompanyId != companyId)
            {
                result.success = false;
                result.messageForUser = Support.ResponsesAPI.MessageAPI.hacker;
                return Ok(result);
            }
            return Ok(await services.ChangePassword(dto));
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
