using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeihanLi.AspNetMvc.AccessControlHelper;

namespace AccountingApp.Helper
{
    public class AccountingControlAccessStrategy : IControlAccessStrategy
    {
        public bool IsControlCanAccess(string accessKey)
            => Constants.AdminUsers.Contains(accessKey);
    }

    public class AccountingActionAccessStrategy : IActionAccessStrategy
    {
        public bool IsActionCanAccess(HttpContext httpContext, string accessKey) => true;

        public IActionResult DisallowedCommonResult => new ContentResult { Content = "You have no permission", ContentType = "text/plain" };
        public JsonResult DisallowedAjaxResult => new JsonResult("You have no permission");
    }
}