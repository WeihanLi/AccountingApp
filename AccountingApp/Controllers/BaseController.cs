using AccountingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WeihanLi.AspNetMvc.AccessControlHelper;

namespace AccountingApp.Controllers
{
    [Authorize]
    [AccessControl]
    public class BaseController:Controller
    {
        protected readonly AccountingDbContext _context;

        protected readonly ILogger _logger;

        public BaseController(AccountingDbContext context,ILogger<BaseController> logger)
        {
            _context = context;
            _logger = logger;

            _logger.LogDebug("log info from BaseController");
        }

        private Helper.BusinessHelper businessHelper;
        protected Helper.BusinessHelper BusinessHelper
        {
            get
            {
                if (businessHelper == null)
                {
                    businessHelper = new Helper.BusinessHelper(_context);
                }
                return businessHelper;
            }
        }
    }
}
