using AccountingApp.Models;
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
        protected readonly AccountingDbContext DbContext;

        protected readonly ILogger Logger;

        private readonly object _locker = new object();

        public BaseController(AccountingDbContext context, ILogger logger)
        {
            DbContext = context;
            Logger = logger;
        }

        private Helper.BusinessHelper _businessHelper;

        protected Helper.BusinessHelper BusinessHelper
        {
            get
            {
                if (_businessHelper == null)
                {
                    lock (_locker)
                    {
                        if (_businessHelper == null)
                        {
                            _businessHelper = new Helper.BusinessHelper(DbContext);
                        }
                    }
                }
                return _businessHelper;
            }
        }
    }
}
