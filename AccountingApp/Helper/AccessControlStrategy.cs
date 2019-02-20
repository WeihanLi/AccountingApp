using Microsoft.AspNetCore.Mvc;
using WeihanLi.AspNetMvc.AccessControlHelper;

namespace AccountingApp.Helper
{
    public class AccountingControlAccessStrategy : IControlAccessStrategy
    {
        public bool IsControlCanAccess(string accessKey) => Constants.AdminUsers.Contains(accessKey);
    }

    public class AccountingResourceAccessStrategy : IResourceAccessStrategy
    {
        public bool IsCanAccess(string accessKey) => true;

        public IActionResult DisallowedCommonResult => new ContentResult { Content = "You have no permission", ContentType = "text/plain" };

        public IActionResult DisallowedAjaxResult => new JsonResult("You have no permission");
    }
}
