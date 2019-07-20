using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AccountingApp.DataAccess;
using AccountingApp.Models;
using AccountingApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WeihanLi.AspNetMvc.MvcSimplePager;
using WeihanLi.Common.Data;
using WeihanLi.Common.Models;
using WeihanLi.EntityFramework;
using WeihanLi.Extensions;
using WeihanLi.Npoi;

namespace AccountingApp.Controllers
{
    public class BillsController : BaseController
    {
        private readonly DalBill repository;

        public BillsController(DalBill billRepository, ILogger<BillsController> logger) : base(logger)
        {
            repository = billRepository;
        }

        // GET: Bill
        public ActionResult Index() => View("NewIndex");

        public async Task<ActionResult> ExportBillsReport()
        {
            var bills = await repository.GetAsync(queryBuilder => queryBuilder
            .WithOrderBy(query => query.OrderBy(x => x.CreatedTime)), HttpContext.RequestAborted);
            if (bills != null && bills.Any())
            {
                return File(bills.ToExcelBytes(), "application/octet-stream", "Bills.xlsx");
            }
            return Content("没有数据需要导出");
        }

        [HttpGet]
        [ActionName("List")]
        public async Task<ActionResult> ListAsync(int pageIndex = 1, int pageSize = 20)
        {
            var data = await repository.GetPagedListAsync(queryBuilder => queryBuilder
                     .WithInclude(query => query.Include(x => x.AccountBillType)),
                pageIndex,
                pageSize, HttpContext.RequestAborted);
            return Json(data);
        }

        /// <summary>
        /// 账单列表
        /// </summary>
        /// <param name="pageIndex">页码索引</param>
        /// <param name="pageSize">每页数据量</param>
        /// <param name="filerPersonName">只看谁创建的</param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("BillsList")]
        public async Task<ActionResult> BillsListAsync(int pageIndex = 1, int pageSize = 10, string filerPersonName = "")
        {
            Expression<Func<Bill, bool>> predict = b => true;
            if (!string.IsNullOrWhiteSpace(filerPersonName))
            {
                predict = predict.And(b => b.CreatedBy == filerPersonName);
            }
            var data = await repository.GetPagedListAsync(queryBuilder => queryBuilder.WithPredict(predict).WithOrderBy(q => q.OrderByDescending(x => x.CreatedTime))
            , pageIndex, pageSize);
            return View(data.ToPagedList());
        }

        public ActionResult BillPayItemList(string personName)
        {
            Expression<Func<BillPayItem, bool>> billPayItemPredict = item => true;
            if (!string.IsNullOrWhiteSpace(personName))
            {
                billPayItemPredict.And(b => b.PersonName == personName);
            }
            var billPayRepo = HttpContext.RequestServices.GetService<IEFRepository<AccountingDbContext, BillPayItem>>();
            return Json(billPayRepo.Get(queryBuilderAction: builder => builder
            .WithPredict(billPayItemPredict).WithOrderBy(q => q.OrderByDescending(x => x.CreatedTime))));
        }

        // GET: Bill/Create
        [ActionName("Create")]
        public async Task<ActionResult> CreateAsync()
        {
            var billTypeRepo = HttpContext.RequestServices.GetRequiredService<IEFRepository<AccountingDbContext, BillType>>();
            ViewData["BillTypes"] = new BillTypeViewModel(await billTypeRepo.GetAllAsync().ContinueWith(r => r.Result.OrderBy(x => x.TypeName)));
            var userRepo = HttpContext.RequestServices.GetRequiredService<IEFRepository<AccountingDbContext, User>>();
            var userNames = await userRepo.GetResultAsync(u => u.Username, builder => builder.WithPredict(s => s.IsActive)
            .WithOrderBy(x => x.OrderBy(a => a.PKID)));
            ViewData["Users"] = userNames;
            return View();
        }

        // POST: Bill/Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync([Bind("BillTitle,BillDescription,BillDetails,BillType,BillFee")]Bill bill)
        {
            var billTypeRepo = HttpContext.RequestServices.GetRequiredService<IEFRepository<AccountingDbContext, BillType>>();
            ViewData["BillTypes"] = new BillTypeViewModel(await billTypeRepo.GetAllAsync().ContinueWith(r => r.Result.OrderBy(x => x.TypeName)));
            var userRepo = HttpContext.RequestServices.GetRequiredService<IEFRepository<AccountingDbContext, User>>();
            var userNames = await userRepo.GetResultAsync(u => u.Username, builder => builder.WithPredict(s => s.IsActive)
    .WithOrderBy(x => x.OrderBy(a => a.PKID)));
            ViewData["Users"] = userNames;

            try
            {
                if (ModelState.IsValid)
                {
                    var items = bill.BillDetails.JsonToType<List<BillPayItemViewModel>>();
                    if (items.Sum(t => t.PayMoney) != bill.BillFee)
                    {
                        ModelState.AddModelError("BillFee", "每个人实付金额与总金额不符，请检查");
                        ViewData["ErrorMsg"] = "每个人实付金额与总金额不符，请检查";
                        return View();
                    }
                    bill.CreatedBy = User.Identity.Name;
                    var res = await repository.AddAsync(bill);
                    if (res != null)
                    {
                        var billItems = items.Select(t => new BillPayItem { BillId = res.PKID, CreatedBy = User.Identity.Name, PayMoney = t.PayMoney, PersonName = t.PersonName }).Where(b => b.PayMoney > 0).ToList();
                        //保存到数据库
                        var billPayItemRepo = HttpContext.RequestServices.GetRequiredService<IEFRepository<AccountingDbContext, BillPayItem>>();
                        await billPayItemRepo.InsertAsync(billItems);
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewData["ErrorMsg"] = "请求参数不合法";
                    return View();
                }
            }
            catch (Exception)
            {
                return View();
            }
        }

        // GET: Bill/Edit/5
        [HttpGet, ActionName("Edit")]
        public async Task<ActionResult> EditAsync(int id)
        {
            var billTypeRepo = HttpContext.RequestServices.GetRequiredService<IEFRepository<AccountingDbContext, BillType>>();
            ViewData["BillTypes"] = new BillTypeViewModel(await billTypeRepo.GetAllAsync().ContinueWith(r => r.Result.OrderBy(x => x.TypeName)));
            return View(await repository.FindAsync(id));
        }

        // POST: Bill/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync([Bind("BillTitle,BillDescription,BillType,PKID")]Bill bill)
        {
            try
            {
                bill.UpdatedBy = User.Identity.Name;
                await repository.UpdateAsync(bill, "BillTitle", "BillType", "BillDescription", "UpdatedBy", "UpdatedTime");
                return RedirectToAction("Index");
            }
            catch
            {
                var billTypeRepo = HttpContext.RequestServices.GetRequiredService<IEFRepository<AccountingDbContext, BillType>>();
                ViewData["BillTypes"] = new BillTypeViewModel(await billTypeRepo.GetAllAsync().ContinueWith(r => r.Result.OrderBy(x => x.TypeName)));
                return View();
            }
        }

        /// <summary>
        /// 更新账单状态
        /// </summary>
        /// <param name="id">账单id</param>
        /// <param name="status">账单状态</param>
        /// <returns></returns>
        [HttpPost, ActionName("UpdateBillStatus")]
        public async Task<IActionResult> UpdateBillStatusAsync(int id, int status)
        {
            var result = new JsonResultModel();
            if (id <= 0 || status <= 0)
            {
                result.Status = JsonResultStatus.RequestError;
                result.ErrorMsg = "请求参数异常";
                return Json(result);
            }
            var bill = new Bill { PKID = id, BillStatus = status };
            bill.UpdatedBy = User.Identity.Name;
            await repository.UpdateAsync(bill, b => b.BillStatus);
            result.Status = JsonResultStatus.Success;
            result.ErrorMsg = "";
            return Json(result);
        }

        // GET: Bill/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }
            var bill = await repository.FindAsync(id);
            if (bill == null)
            {
                return NotFound();
            }
            return View(bill);
        }

        // POST: Bill/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedAsync(int id)
        {
            await Task.WhenAll(
                repository.DeleteAsync(m => m.PKID == id, User.Identity.Name),
                HttpContext.RequestServices.GetRequiredService<AccountingRepository<BillPayItem>>()
                .DeleteAsync(t => t.BillId == id, User.Identity.Name));
            return RedirectToAction("Index");
        }
    }
}
