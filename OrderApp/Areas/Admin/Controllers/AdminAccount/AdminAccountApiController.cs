using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using OrderApp.Models;
using System.Threading.Tasks;
using System.Web.Http;

namespace OrderApp.Areas.Admin.Controllers.AdminAccount
{
    [Authorize]
    [RoutePrefix("api/admin/account")]
    public class AdminAccountApiController : ApiController
    {
        private AdminAccountService services = new AdminAccountService();
        [HttpGet]
        [Route("get-all")]
        public IHttpActionResult GetAll()
        {
            return Ok(services.GetAll());
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
