using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using OrderApp.DataFactory;
using OrderApp.Models;
using System.Threading.Tasks;
using System.Web.Http;

namespace OrderApp.Areas.Admin.Controllers.AdminAccount
{
    [Authorize(Roles = "admin")]
    [RoutePrefix("api/admin/account")]
    public class AdminAccountApiController : ApiController
    {
        private AdminAccountService services = new AdminAccountService();
        private OrderAppEntities db = new OrderAppEntities();
        [HttpGet]
        [Route("get-all")]
        public IHttpActionResult GetAll()
        {
            string userId = User.Identity.GetUserId();
            int companyId = db.UserExtension.Find(userId)?.CompanyId ?? 0;
            return Ok(services.GetAll(companyId));
        }

        [HttpPost]
        [Route("update")]
        public async Task<IHttpActionResult> Update(AdminAccountDto.UpdateDto dto)
        {
            if (string.IsNullOrEmpty(dto.AspNetUser.Id))
            {
                return Ok(await services.Create(dto));
            }
            else
            {
                return Ok(await services.Edit(dto));
            }
        }
        [HttpPost]
        [Route("change-password")]
        public async Task<IHttpActionResult> ChangePassword(AdminAccountDto.UpdateDto dto)
        {
            return Ok(await services.ChangePassword(dto));
        }
    }
}
