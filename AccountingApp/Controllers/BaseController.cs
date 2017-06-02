using AccountingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountingApp.Controllers
{
    [Authorize]
    public class BaseController:Controller
    {
        protected readonly AccountingDbContext _context;

        public BaseController(AccountingDbContext context)
        {
            _context = context;
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
