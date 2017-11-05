using System;
using System.Linq;
using System.Threading.Tasks;
using AccountingApp.Models;
using AccountingApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WeihanLi.Common.Models;

namespace AccountingApp.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(AccountingDbContext context, ILogger<HomeController> logger) : base(context, logger)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> BillPaySummary()
        {
            var billPayItems = (await BusinessHelper.BillPayItemHelper.SelectAsync(b => true, b => b.BillId)).GroupBy(b => b.PersonName).Select(g => new BasicReportModel { Name = g.Key, Value = g.Sum(b => b.PayMoney) }).ToArray();

            var reportModel = new PieReportModel
            {
                Names = billPayItems.Select(b => b.Name),
                Values = billPayItems.Select(b => Math.Round(b.Value, 2)),
                Data = billPayItems
            };

            return Json(new JsonResultModel
            {
                Status = JsonResultStatus.Success,
                Data = reportModel
            });
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}