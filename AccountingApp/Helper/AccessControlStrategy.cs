using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
        public bool IsActionCanAccess(string areaName, string controllerName, string actionName)
            => true;

        public IActionResult DisallowedCommonResult => new ContentResult { Content = "You have no permission", ContentType = "text/plain" };
        public JsonResult DisallowedAjaxResult => new JsonResult("You have no permission");
    }
}