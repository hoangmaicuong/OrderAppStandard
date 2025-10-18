using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using OrderApp.DataFactory;
using OrderApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OrderApp.Controllers.AccountManage
{
    [Authorize]
    public class AccountManageController : Controller
    {
        // GET: AccountManage
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private OrderAppEntities db = new OrderAppEntities();
        public AccountManageController() { }
        public AccountManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "AdminHome", new { area = "Admin" });
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        // POST: /AccountManager/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.PhoneNumber);
            if (user == null)
            {
                ModelState.AddModelError("PhoneNumber", "Số điện thoại không đúng!");
                return View(model);
            }
            var isOwnerCompany = await db.Company.AnyAsync(x => x.CompanyOwnerId == user.Id);

            // 🔹 Nếu không phải chủ doanh nghiệp, mới áp dụng lockout
            if (!isOwnerCompany)
            {
                user.LockoutEnabled = true;
                // 🔸 Kiểm tra nếu user đang bị khóa
                if (user.LockoutEnabled && user.LockoutEndDateUtc.HasValue && user.LockoutEndDateUtc > DateTime.UtcNow)
                {
                    var remain = user.LockoutEndDateUtc.Value - DateTime.UtcNow;
                    ModelState.AddModelError("", $"Tài khoản bị khóa. Vui lòng thử lại sau {Math.Ceiling(remain.TotalMinutes)} phút.");
                    return View(model);
                }
            }
            bool isSuccess = await UserManager.CheckPasswordAsync(user, model.Password);
            if (!isSuccess)
            {
                // Nếu không phải chủ doanh nghiệp thì áp dụng đếm lỗi
                if (!isOwnerCompany)
                {
                    // Tăng số lần đăng nhập sai
                    await UserManager.AccessFailedAsync(user.Id);
                    // Kiểm tra nếu bị khóa
                    if (user.AccessFailedCount + 1 >= 5)
                    {
                        await UserManager.SetLockoutEndDateAsync(user.Id, DateTimeOffset.UtcNow.AddMinutes(5));
                        ModelState.AddModelError("", "Tài khoản đã bị khóa trong 5 phút do đăng nhập sai quá nhiều lần!");
                    }
                    else
                    {
                        int remainAttempts = 5 - (user.AccessFailedCount + 1);
                        ModelState.AddModelError("Password", $"Mật khẩu không đúng! Bạn còn {remainAttempts} lần thử.");
                    }
                }
                else
                {
                    // Chủ doanh nghiệp thì chỉ báo sai mật khẩu
                    ModelState.AddModelError("Password", "Mật khẩu không đúng!");
                }

                return View(model);
            }
            // 🔸 Nếu đăng nhập đúng, reset số lần sai
            await UserManager.ResetAccessFailedCountAsync(user.Id);

            // ✅ Cập nhật SecurityStamp để vô hiệu hóa các session cũ
            await UserManager.UpdateSecurityStampAsync(user.Id);
            await SignInManager.SignInAsync(user, isPersistent: model.RememberMe, rememberBrowser: false);

            return RedirectToLocal(returnUrl);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "AccountManage");
        }
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.PhoneNumber, PhoneNumber = model.PhoneNumber };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, "admin");
                    Company company = new Company
                    {
                        CompanyOwnerId = user.Id,
                        CompanyName = model.CompanyName
                    };
                    UserExtension userExtension = new UserExtension
                    {
                        AspNetUserId = user.Id,
                        Company = company
                    };
                    db.UserExtension.Add(userExtension);
                    db.SaveChanges();
                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Login", "AccountManage");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "AdminHome", new {  area = "Admin" });
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}