using AccountingApp.Models;
using AccountingApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
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

        /// <summary>
        /// 账单金额总览
        /// </summary>
        /// <returns></returns>
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
                Result = reportModel
            });
        }

        /// <summary>
        /// 账单类型统计
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> BillPayTypeSummary()
        {
            var billPayItems = (await BusinessHelper.BillHelper.SelectWithTypeInfoAsync(b => true, b => b.CreatedTime, true)).GroupBy(b => new { b.BillType, TypeName = b.AccountBillType.TypeName }).Select(g => new BasicReportModel { Name = g.Key.TypeName, Value = g.Sum(b => b.BillFee) }).ToArray();

            var reportModel = new PieReportModel
            {
                Names = billPayItems.Select(b => b.Name),
                Values = billPayItems.Select(b => Math.Round(b.Value, 2)),
                Data = billPayItems
            };

            return Json(new JsonResultModel
            {
                Status = JsonResultStatus.Success,
                Result = reportModel
            });
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}