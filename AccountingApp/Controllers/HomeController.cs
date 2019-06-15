using System;
using System.Linq;
using System.Threading.Tasks;
using AccountingApp.Models;
using AccountingApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WeihanLi.Common.Models;
using WeihanLi.EntityFramework;

namespace AccountingApp.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(ILogger<HomeController> logger) : base(logger)
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
            var billPayItems = (await HttpContext.RequestServices.GetRequiredService<IEFRepository<AccountingDbContext, BillPayItem>>().GetAsync())
                .GroupBy(b => b.PersonName)
                .Select(g => new BasicReportModel { Name = g.Key, Value = g.Sum(b => b.PayMoney) })
                .ToArray();

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
            var billPayItems = (await HttpContext.RequestServices.GetRequiredService<IEFRepository<AccountingDbContext, Bill>>()
                .GetAsync(builder => builder.WithInclude(x => x.Include(_ => _.AccountBillType)), HttpContext.RequestAborted))
                .GroupBy(b => new { b.BillType, TypeName = b.AccountBillType?.TypeName })
                .Select(g => new BasicReportModel
                {
                    Name = g.Key.TypeName,
                    Value = g.Sum(b => b.BillFee)
                })
                .ToArray();

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
