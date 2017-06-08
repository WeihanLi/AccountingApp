using AccountingApp.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AccountingApp.Controllers
{
    public class AccountController : BaseController
    {
        private readonly AccountingDbContext _dbContext;
        public AccountController(AccountingDbContext dbEntity) : base(dbEntity)
        {
            _dbContext = dbEntity;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost, ActionName("UpdatePassword")]
        public async Task<IActionResult> UpdatePasswordAsync(ViewModels.UpdatePasswordViewModel model)
        {
            var result = new HelperModels.JsonResultModel();
            if (ModelState.IsValid)
            {
                var user = await BusinessHelper.UserHelper.FetchAsync(u => u.Username == User.Identity.Name);
                if(user == null)
                {
                    result.Status = HelperModels.JsonResultStatus.ResourceNotFound;
                    result.Msg = "用户名不存在！";
                    return Json(result);
                }
                if(!user.PasswordHash.Equals(Helper.SecurityHelper.SHA256_Encrypt(model.OldPassword)))
                {
                    result.Status = HelperModels.JsonResultStatus.RequestError;
                    result.Msg = "原密码有误，请重试";
                    return Json(result);
                }
                else
                {
                    user.PasswordHash = Helper.SecurityHelper.SHA256_Encrypt(model.NewPassword);
                    var isSuccess = await BusinessHelper.UserHelper.UpdateAsync(user, u => u.PasswordHash);
                    if(isSuccess)
                    {
                        await HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        result.Msg = "更新成功";
                        result.Status = HelperModels.JsonResultStatus.Success;
                    }
                    else
                    {
                        result.Msg = "更新失败";
                        result.Status = HelperModels.JsonResultStatus.ProcessFail;
                    }
                }
            }
            return Json(result);
        }

        [HttpPost,ActionName("ValidateOldPassword")]
        public async Task<IActionResult> ValidateOldPasswordAsync(string password)
        {
            var result = new HelperModels.JsonResultModel<bool>() { Data = false };
            if(string.IsNullOrEmpty(password))
            {
                result.Status = HelperModels.JsonResultStatus.RequestError;
                result.Msg = "原密码不能为空";
                return Json(result);
            }
            var user = await BusinessHelper.UserHelper.FetchAsync(u => u.Username == User.Identity.Name);
            if (user == null)
            {
                result.Status = HelperModels.JsonResultStatus.ResourceNotFound;
                result.Msg = "用户不存在！";
                return Json(result);
            }
            if (!user.PasswordHash.Equals(Helper.SecurityHelper.SHA256_Encrypt(password)))
            {
                result.Status = HelperModels.JsonResultStatus.RequestError;
                result.Msg = "原密码有误";
            }
            else
            {
                result.Data = true;
                result.Msg = "密码正确";
                result.Status = HelperModels.JsonResultStatus.Success;
            }
            return Json(result);
        }

        /// <summary>
        /// 登录页面
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult Login()
        {
            if(User.Identity.IsAuthenticated)
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
        public async Task<IActionResult> LogonAsync(ViewModels.LogonViewModel model)
        {
            var result = new HelperModels.JsonResultModel();
            if (ModelState.IsValid)
            {
                var user = await BusinessHelper.UserHelper.FetchAsync(u => u.Username == model.Username);
                if (user == null)
                {
                    result.Status = HelperModels.JsonResultStatus.ResourceNotFound;
                    result.Msg = "用户不存在";
                }
                else
                {
                    if (user.PasswordHash.Equals(Helper.SecurityHelper.SHA256_Encrypt(model.Password)))
                    {
                        var u = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.Username) }, CookieAuthenticationDefaults.AuthenticationScheme));
                        await HttpContext.Authentication.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, u);
                        //
                        result.Msg = "登录成功";
                        result.Status = HelperModels.JsonResultStatus.Success;
                    }
                    else
                    {
                        result.Status = HelperModels.JsonResultStatus.RequestError;
                        result.Msg = "用户名或密码错误";
                    }
                }
            }
            else
            {
                result.Status = HelperModels.JsonResultStatus.RequestError;
                result.Msg = "请求参数异常！";
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
            await HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("Login");
        }
    }
}