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
        protected readonly AccountingDbContext _context;

        protected readonly ILogger _logger;

        private readonly object _locker = new object();

        public BaseController(AccountingDbContext context, ILogger<BaseController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private Helper.BusinessHelper businessHelper;

        protected Helper.BusinessHelper BusinessHelper
        {
            get
            {
                if (businessHelper == null)
                {
                    lock (_locker)
                    {
                        if (businessHelper == null)
                        {
                            businessHelper = new Helper.BusinessHelper(_context);
                        }
                    }
                }
                return businessHelper;
            }
        }
    }
}