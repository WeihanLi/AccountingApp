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
        public bool IsCanAccess(string accessKey) => true;

        public IActionResult DisallowedCommonResult => new ContentResult { Content = "You have no permission", ContentType = "text/plain" };
        IActionResult IActionAccessStrategy.DisallowedAjaxResult => DisallowedAjaxResult;

        public JsonResult DisallowedAjaxResult => new JsonResult("You have no permission");
    }
}
