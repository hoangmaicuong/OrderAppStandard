using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using OrderApp.DataFactory;
using OrderApp.Models;
using System;
using System.Collections.Generic;
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
            bool isSuccess = await UserManager.CheckPasswordAsync(user, model.Password);
            if (!isSuccess)
            {
                ModelState.AddModelError("Password", "Mật khẩu không đúng!");
                return View(model);
            }
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