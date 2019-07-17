using System.Security.Claims;
using System.Threading.Tasks;
using AccountingApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WeihanLi.AspNetMvc.AccessControlHelper;
using WeihanLi.Common.Helpers;
using WeihanLi.Common.Models;
using WeihanLi.EntityFramework;

namespace AccountingApp.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IEFRepository<AccountingDbContext, User> repository;

        public AccountController(IEFRepository<AccountingDbContext, User> userRepository, ILogger<AccountController> logger) : base(logger)
        {
            repository = userRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost, ActionName("UpdatePassword")]
        public async Task<IActionResult> UpdatePasswordAsync(ViewModels.UpdatePasswordViewModel model)
        {
            var result = new JsonResultModel();
            if (ModelState.IsValid)
            {
                var user = await repository.FetchAsync(u => u.Username == User.Identity.Name);
                if (user == null)
                {
                    result.Status = JsonResultStatus.ResourceNotFound;
                    result.ErrorMsg = "用户名不存在！";
                    return Json(result);
                }
                if (!user.PasswordHash.Equals(SecurityHelper.SHA256(model.OldPassword)))
                {
                    result.Status = JsonResultStatus.RequestError;
                    result.ErrorMsg = "原密码有误，请重试";
                    return Json(result);
                }
                else
                {
                    user.PasswordHash = SecurityHelper.SHA256(model.NewPassword);
                    var isSuccess = await repository.UpdateAsync(user, u => u.PasswordHash).ContinueWith(r => r.Result > 0);
                    if (isSuccess)
                    {
                        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        result.ErrorMsg = "更新成功";
                        result.Status = JsonResultStatus.Success;
                    }
                    else
                    {
                        result.ErrorMsg = "更新失败";
                        result.Status = JsonResultStatus.ProcessFail;
                    }
                }
            }
            return Json(result);
        }

        [HttpPost, ActionName("ValidateOldPassword")]
        public async Task<IActionResult> ValidateOldPasswordAsync(string password)
        {
            var result = new JsonResultModel<bool>();
            if (string.IsNullOrEmpty(password))
            {
                result.Status = JsonResultStatus.RequestError;
                result.ErrorMsg = "原密码不能为空";
                return Json(result);
            }
            var user = await repository.FetchAsync(u => u.Username == User.Identity.Name);
            if (user == null)
            {
                result.Status = JsonResultStatus.ResourceNotFound;
                result.ErrorMsg = "用户不存在！";
                return Json(result);
            }
            if (!user.PasswordHash.Equals(SecurityHelper.SHA256(password)))
            {
                result.Status = JsonResultStatus.RequestError;
                result.ErrorMsg = "原密码有误";
            }
            else
            {
                result.Result = true;
                result.ErrorMsg = "密码正确";
                result.Status = JsonResultStatus.Success;
            }
            return Json(result);
        }

        /// <summary>
        /// 登录页面
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [NoAccessControl]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/");
            }
            return View();
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="model">登录信息</param>
        /// <returns></returns>
        [ActionName("Logon")]
        [HttpPost]
        [AllowAnonymous]
        [NoAccessControl]
        public async Task<IActionResult> LogonAsync(ViewModels.LogonViewModel model)
        {
            var result = new JsonResultModel();
            if (ModelState.IsValid)
            {
                var user = await repository.FetchAsync(u => u.Username == model.Username);
                if (user == null)
                {
                    result.Status = JsonResultStatus.ResourceNotFound;
                    result.ErrorMsg = "用户不存在";
                }
                else
                {
                    if (user.PasswordHash.Equals(SecurityHelper.SHA256(model.Password)))
                    {
                        var u = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.Username) }, CookieAuthenticationDefaults.AuthenticationScheme));
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, u, new AuthenticationProperties { IsPersistent = model.RememberMe, AllowRefresh = true });
                        //
                        result.ErrorMsg = "登录成功";
                        result.Status = JsonResultStatus.Success;

                        Logger.LogInformation($"{user.Username} login success");
                    }
                    else
                    {
                        result.Status = JsonResultStatus.RequestError;
                        result.ErrorMsg = "用户名或密码错误";

                        Logger.LogWarning($"{user.Username} login failed");
                    }
                }
            }
            else
            {
                result.Status = JsonResultStatus.RequestError;
                result.ErrorMsg = "请求参数异常！";
            }
            return Json(result);
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        [ActionName("LogOut")]
        public async Task<IActionResult> LogOutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("Login");
        }
    }
}
