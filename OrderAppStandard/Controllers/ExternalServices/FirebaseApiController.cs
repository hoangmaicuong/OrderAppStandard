using Microsoft.AspNet.Identity;
using OrderApp.Controllers.Home;
using OrderApp.DataFactory;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace OrderApp.Controllers.ExternalServices
{
    [RoutePrefix("api/firebase")]
    public class FirebaseApiController : ApiController
    {
        private FirebaseService services = new FirebaseService();
        private OrderAppEntities db = new OrderAppEntities();
        private string userId = null;
        private int companyId = 0;
        // Đường dẫn file service account JSON (tải từ Firebase Console)
        string serviceAccountPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content/serviceAccountKey.json");
        FirebaseHelper firebase;
        private FirebaseApiController()
        {
            userId = User.Identity.GetUserId();
            companyId = db.UserExtension.Find(userId)?.CompanyId ?? 0;
            firebase = new FirebaseHelper(serviceAccountPath);
        }
        [Authorize]
        [HttpPost]
        [Route("register-topic")]
        public async Task<IHttpActionResult> RegisterTopic(FirebaseDto.TokenRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
                return BadRequest("Token is required");

            var userExten = await db.UserExtension.FirstOrDefaultAsync(x => x.AspNetUserId == userId);

            if (userExten == null)
            {
                // 🔹 Chưa có record → tạo mới + subscribe token
                userExten = new UserExtension
                {
                    AspNetUserId = userId,
                    FirebaseToken = request.Token
                };
                db.UserExtension.Add(userExten);
            }
            else if (userExten.FirebaseToken != request.Token)
            {
                // Cập nhật token mới
                userExten.FirebaseToken = request.Token;
            }
            else
            {
                // 🔹 Token không thay đổi → return luôn, không gọi API FCM
                return Ok(new { success = true, message = "Token unchanged, no action taken" });
            }

            await db.SaveChangesAsync();

            return Ok(new { success = true, message = "Token registered/updated successfully" });
        }
    }
}
