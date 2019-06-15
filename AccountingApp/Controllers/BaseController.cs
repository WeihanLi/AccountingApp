using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WeihanLi.AspNetMvc.AccessControlHelper;

namespace AccountingApp.Controllers
{
    [Authorize]
    [AccessControl]
    public class BaseController : Controller
    {
        protected readonly ILogger Logger;

        public BaseController(ILogger logger)
        {
            Logger = logger;
        }
    }
}
