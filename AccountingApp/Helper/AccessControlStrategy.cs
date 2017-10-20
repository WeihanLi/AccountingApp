using Microsoft.AspNetCore.Mvc;
using System;
using WeihanLi.AspNetMvc.AccessControlHelper;

namespace AccountingApp.Helper
{
    public class AccountingControlAccessStrategy : IControlAccessStrategy
    {
        private static readonly string[] AdminUsers = new[] {"liweihan", "heyafei", "zoushirong"};

        public bool IsControlCanAccess(string accessKey)
        {
            return AdminUsers.IndexOf(accessKey) >= 0;
        }
    }

    public class AccountingActionAccessStrategy : IActionAccessStrategy
    {
        public bool IsActionCanAccess(string areaName, string controllerName, string actionName)
        {
            return true;
        }

        public IActionResult DisallowedCommonResult => new ContentResult { Content = "You have no permission", ContentType = "text/plain" };
        public JsonResult DisallowedAjaxResult => new JsonResult("You have no permission");
    }
}