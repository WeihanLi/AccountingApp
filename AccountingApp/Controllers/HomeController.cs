using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountingApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace AccountingApp.Controllers
{
    public class HomeController : BaseController
    {
        private readonly AccountingDbContext _dbContext;
        public HomeController(AccountingDbContext dbEntity) : base(dbEntity)
        {
            _dbContext = dbEntity;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
