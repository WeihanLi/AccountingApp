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

        /// <summary>
        /// 登录页面
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult Login()
        {
            if(HttpContext.User.Identity.IsAuthenticated)
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